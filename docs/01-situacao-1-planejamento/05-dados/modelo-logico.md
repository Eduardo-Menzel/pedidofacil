# Modelo Lógico Normalizado – PedidoFácil

Caminho seguido na modelagem: **diagrama de classes → modelo conceitual (DER) → modelo lógico normalizado → modelo físico (script SQL)**.

## 1. Tabelas

Notação: <ins>sublinhado</ins> = chave primária · *itálico* = chave estrangeira.

- **Clientes** (<ins>Id</ins>, Nome, CnpjCpf, Email, Telefone, Cidade, UF, DataCadastro)
  - CnpjCpf: único
- **Produtos** (<ins>Id</ins>, Codigo, Descricao, PrecoUnitario, Ativo)
  - Codigo: único
- **Pedidos** (<ins>Id</ins>, *ClienteId*, DataPedido, Status, DescontoPercentual, Observacao)
  - ClienteId → Clientes.Id
- **ItensPedido** (<ins>Id</ins>, *PedidoId*, *ProdutoId*, Quantidade, PrecoUnitario)
  - PedidoId → Pedidos.Id
  - ProdutoId → Produtos.Id

Além dessas, o ASP.NET Core Identity cria automaticamente as tabelas de usuários e senhas (AspNetUsers, AspNetRoles, AspNetUserClaims, AspNetUserLogins, AspNetUserRoles, AspNetUserTokens e AspNetRoleClaims). Elas pertencem ao framework e não foram modeladas aqui.

## 2. Normalização

| Forma normal | Exigência | Como o modelo atende |
|---|---|---|
| **1FN** | Todos os campos têm valores atômicos (um valor só) e não há grupos repetidos | Os itens do pedido não ficam em colunas repetidas (Produto1, Produto2...) dentro de Pedidos: viraram a tabela ItensPedido, com uma linha por item. Telefone é um único valor. |
| **2FN** | Estar na 1FN e todo campo não chave depender da chave primária inteira | Todas as tabelas usam chave primária simples (Id), então não existe dependência parcial. Em ItensPedido, Quantidade e PrecoUnitario dependem do item, e não só do pedido ou só do produto. |
| **3FN** | Estar na 2FN e nenhum campo não chave depender de outro campo não chave | O nome e o CNPJ do cliente não são copiados para Pedidos (ficam só em Clientes, ligados pela FK). A descrição do produto não é copiada para ItensPedido. O valor total não é gravado. |

### Decisões que podem parecer redundância (e não são)

1. **ValorTotal não é gravado.** O total do pedido depende dos itens e do desconto; gravá-lo criaria um campo derivado de outros campos (violaria a 3FN) e poderia ficar desatualizado. Ele é calculado pelo método `Pedido.CalcularTotal()` e, no banco, pela visão `vw_TotalPedidos`.
2. **PrecoUnitario aparece em Produtos e em ItensPedido.** Não é redundância: em Produtos é o **preço atual**; em ItensPedido é o **preço no dia da venda** (RN09). Se o preço do produto mudar amanhã, os pedidos antigos continuam com o valor correto.
3. **Cidade e UF ficam em Clientes.** Uma tabela separada de cidades seria a normalização máxima, mas não traz ganho para um sistema deste tamanho; fica registrada como melhoria futura.

## 3. Quadro de correspondência: Diagrama de Classes × Modelo Relacional

| Diagrama de classes (UML) | Modelo relacional (banco) | No PedidoFácil |
|---|---|---|
| Classe | Tabela | Cliente → Clientes, Pedido → Pedidos |
| Atributo | Coluna | Cliente.Nome → coluna Nome |
| Identidade do objeto | Chave primária | Id em todas as tabelas |
| Associação 1 para N | Chave estrangeira no lado N | Pedidos.ClienteId |
| Associação N para N | Tabela associativa | ItensPedido (entre Pedidos e Produtos) |
| Composição | Chave estrangeira com exclusão em cascata | ItensPedido.PedidoId (ON DELETE CASCADE) |
| Enumeração | Coluna inteira com domínio restrito (CHECK) | Status (0 a 3) |
| Método | Stored procedure / visão (ou fica no código) | CalcularTotal() → visão vw_TotalPedidos; as demais regras ficam no Model em C# |
| Herança e realização (interface) | Sem equivalente direto | Não foram usadas no domínio |

## 4. Projeto físico e teste do script

O projeto físico está em [script-fisico.sql](script-fisico.sql). O script foi executado em um banco SQLite vazio e as regras foram testadas com dados fictícios:

| Teste | Resultado esperado | Resultado obtido |
|---|---|---|
| Total do pedido de exemplo (3 itens, 10% de desconto) | Subtotal 936,00 · Total 842,40 | 936,00 · 842,40 ✔ |
| Excluir cliente que tem pedido (RN08) | Bloqueado | Bloqueado (FOREIGN KEY) ✔ |
| Cadastrar CNPJ repetido (RN01) | Bloqueado | Bloqueado (UNIQUE) ✔ |
| Produto com preço negativo (RN02) | Bloqueado | Bloqueado (CHECK) ✔ |
| Item com quantidade 0 (RN02) | Bloqueado | Bloqueado (CHECK) ✔ |
| Pedido com desconto de 35% (RN03) | Bloqueado | Bloqueado (CHECK) ✔ |
| Status fora da lista (7) | Bloqueado | Bloqueado (CHECK) ✔ |
| Excluir produto que já foi vendido | Bloqueado | Bloqueado (FOREIGN KEY) ✔ |
| Excluir pedido | Itens excluídos junto | 0 itens restantes (CASCADE) ✔ |

O banco garante as regras mesmo que alguém tente gravar dados direto na tabela; o sistema também valida antes, para mostrar mensagens amigáveis ao usuário (camada Model).
