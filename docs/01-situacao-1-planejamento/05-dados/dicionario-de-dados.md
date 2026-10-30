# Dicionário de Dados – PedidoFácil

**SGBD:** SQLite 3 · **Acesso:** Entity Framework Core · **Script:** [script-fisico.sql](script-fisico.sql)

Colunas da tabela: **Tipo lógico** (como o dado é tratado no sistema), **Tipo físico** (como fica gravado no SQLite), **Tam.** (tamanho máximo ou precisão), **Nulo** (se pode ficar vazio), **Chave** e **Domínio / regra** (valores permitidos).

> No SQLite, o tipo `decimal` do C# é gravado como TEXT pelo Entity Framework Core para não perder precisão nos centavos. Datas são gravadas como TEXT no formato `AAAA-MM-DD HH:MM:SS`.

---

## Tabela: Clientes

Guarda os clientes para os quais o vendedor emite pedidos.

| Campo | Tipo lógico | Tipo físico | Tam. | Nulo | Chave | Domínio / regra | Descrição |
|---|---|---|---|---|---|---|---|
| Id | Inteiro | INTEGER | – | Não | PK | Autoincremento | Identificador do cliente |
| Nome | Texto | TEXT | 100 | Não | | 3 a 100 caracteres | Nome ou razão social |
| CnpjCpf | Texto | TEXT | 18 | Não | UK | CPF (000.000.000-00) ou CNPJ (00.000.000/0000-00); único (RN01) | Documento do cliente |
| Email | Texto | TEXT | 100 | Sim | | Formato de e-mail válido | E-mail de contato |
| Telefone | Texto | TEXT | 20 | Sim | | Formato (00) 00000-0000 | Telefone de contato |
| Cidade | Texto | TEXT | 60 | Sim | | Livre | Cidade do cliente |
| UF | Texto | TEXT | 2 | Sim | | Sigla de estado brasileiro (SC, PR, RS...) | Estado do cliente |
| DataCadastro | Data e hora | TEXT | – | Não | | Preenchida automaticamente com a data atual | Quando o cliente foi cadastrado |

Índices: `IX_Clientes_CnpjCpf` (único) · `IX_Clientes_Nome` (busca por nome).

---

## Tabela: Produtos

Guarda os produtos que podem ser vendidos.

| Campo | Tipo lógico | Tipo físico | Tam. | Nulo | Chave | Domínio / regra | Descrição |
|---|---|---|---|---|---|---|---|
| Id | Inteiro | INTEGER | – | Não | PK | Autoincremento | Identificador do produto |
| Codigo | Texto | TEXT | 30 | Não | UK | 1 a 30 caracteres; único | Código interno do produto (ex.: PRD-001) |
| Descricao | Texto | TEXT | 150 | Não | | 3 a 150 caracteres | Descrição do produto |
| PrecoUnitario | Decimal | TEXT | 10,2 | Não | | Maior que zero (RN02) | Preço de venda atual, em reais |
| Ativo | Lógico | INTEGER | – | Não | | 1 = sim, 0 = não; padrão 1 | Produtos inativos não aparecem para adicionar em pedidos |

Índice: `IX_Produtos_Codigo` (único).

---

## Tabela: Pedidos

Guarda o cabeçalho de cada pedido de venda.

| Campo | Tipo lógico | Tipo físico | Tam. | Nulo | Chave | Domínio / regra | Descrição |
|---|---|---|---|---|---|---|---|
| Id | Inteiro | INTEGER | – | Não | PK | Autoincremento | Número do pedido |
| ClienteId | Inteiro | INTEGER | – | Não | FK → Clientes.Id | O cliente deve existir; exclusão do cliente bloqueada (RN08) | Cliente do pedido |
| DataPedido | Data e hora | TEXT | – | Não | | Padrão: data atual (RN10) | Data de emissão |
| Status | Inteiro (enumeração) | INTEGER | – | Não | | 0 = Aberto, 1 = Aprovado, 2 = Faturado, 3 = Cancelado; padrão 0 (RN06) | Situação do pedido |
| DescontoPercentual | Decimal | TEXT | 5,2 | Não | | 0 a 30; padrão 0 (RN03) | Desconto sobre o subtotal, em % |
| Observacao | Texto | TEXT | 500 | Sim | | Livre | Observações do vendedor |

Índices: `IX_Pedidos_ClienteId` · `IX_Pedidos_Status` (filtro por status).

**Campo calculado (não gravado):** Total = soma(Quantidade × PrecoUnitario dos itens) × (1 − DescontoPercentual ÷ 100), arredondado em 2 casas (RN04). Disponível pelo método `Pedido.CalcularTotal()` e pela visão `vw_TotalPedidos`.

---

## Tabela: ItensPedido

Guarda os produtos de cada pedido (entidade associativa entre Pedidos e Produtos).

| Campo | Tipo lógico | Tipo físico | Tam. | Nulo | Chave | Domínio / regra | Descrição |
|---|---|---|---|---|---|---|---|
| Id | Inteiro | INTEGER | – | Não | PK | Autoincremento | Identificador do item |
| PedidoId | Inteiro | INTEGER | – | Não | FK → Pedidos.Id | Exclusão em cascata (excluir o pedido exclui os itens) | Pedido ao qual o item pertence |
| ProdutoId | Inteiro | INTEGER | – | Não | FK → Produtos.Id | O produto deve existir; exclusão do produto bloqueada | Produto vendido |
| Quantidade | Inteiro | INTEGER | – | Não | | Maior ou igual a 1 (RN02) | Quantidade vendida |
| PrecoUnitario | Decimal | TEXT | 10,2 | Não | | Maior que zero; copiado do produto ao adicionar o item (RN09) | Preço unitário no dia da venda |

Índices: `IX_ItensPedido_PedidoId` · `IX_ItensPedido_ProdutoId`.

**Campo calculado (não gravado):** Subtotal = Quantidade × PrecoUnitario.

---

## Visão: vw_TotalPedidos

Consulta pronta que mostra, para cada pedido: número, nome do cliente, data, status, subtotal, desconto e total. Não guarda dados, apenas calcula a partir das tabelas.

## Tabelas do ASP.NET Core Identity

Criadas automaticamente pelo framework para cadastro de usuários e login: `AspNetUsers` (usuários, e-mail e senha com hash), `AspNetRoles`, `AspNetUserRoles`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserTokens` e `AspNetRoleClaims`. Não são alteradas pelo projeto.
