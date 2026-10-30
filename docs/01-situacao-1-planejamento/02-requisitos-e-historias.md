# Requisitos e Histórias de Usuário – PedidoFácil

Este documento reúne os requisitos funcionais (RF), os requisitos não funcionais (RNF), as regras de negócio (RN) e as histórias de usuário (HU) do sistema. Cada item tem um código para poder ser ligado aos diagramas, ao código e aos testes (rastreabilidade).

---

## 1. Requisitos funcionais (RF)

| Código | Requisito | Prioridade | Caso de uso | História |
|---|---|---|---|---|
| RF01 | O sistema deve permitir criar conta, entrar (login) e sair (logout) | Alta | UC01 | HU01 |
| RF02 | O sistema deve permitir incluir, listar, buscar por nome, editar e excluir clientes | Alta | UC02 | HU02 |
| RF03 | O sistema deve permitir incluir, listar, editar e excluir produtos | Alta | UC03 | HU03 |
| RF04 | O sistema deve permitir registrar um pedido para um cliente cadastrado | Alta | UC04 | HU04 |
| RF05 | O sistema deve permitir adicionar e remover itens (produto + quantidade) de um pedido | Alta | UC05 | HU05 |
| RF06 | O sistema deve calcular automaticamente o total do pedido, aplicando o desconto | Alta | UC04, UC05 | HU04, HU05 |
| RF07 | O sistema deve permitir alterar o status do pedido (aprovar, faturar, cancelar) | Alta | UC06 | HU06 |
| RF08 | O sistema deve listar os pedidos, permitindo filtrar por status | Média | UC07 | HU07 |

## 2. Requisitos não funcionais (RNF)

| Código | Categoria | Requisito |
|---|---|---|
| RNF01 | Usabilidade | A aplicação deve ser responsiva e funcionar em telas de celular (Bootstrap 5) |
| RNF02 | Tecnologia | O back-end deve ser desenvolvido em C# com ASP.NET Core MVC (.NET 10), seguindo o padrão MVC |
| RNF03 | Dados | Os dados devem ser armazenados em SQLite, via Entity Framework Core, com integridade referencial |
| RNF04 | Segurança | As senhas devem ser guardadas com hash (ASP.NET Core Identity) e todas as telas de negócio devem exigir login |
| RNF05 | Usabilidade | Todas as mensagens devem estar em português e ser claras; o usuário nunca deve ver uma mensagem de erro técnica |
| RNF06 | Desempenho | As telas devem responder em até 2 segundos com até 1.000 registros por tabela |
| RNF07 | Manutenibilidade | O código deve ser versionado no GitHub e ter testes automatizados (xUnit) |
| RNF08 | Confiabilidade | Operações que violem regras de negócio devem ser bloqueadas sem corromper os dados |

## 3. Regras de negócio (RN)

| Código | Regra | Onde é aplicada |
|---|---|---|
| RN01 | O CNPJ/CPF do cliente é obrigatório e não pode se repetir | Cliente (validação + índice único no banco) |
| RN02 | O preço do produto deve ser maior que zero e a quantidade do item deve ser no mínimo 1 | Produto e ItemPedido |
| RN03 | O desconto do pedido deve estar entre 0% e 30% | Pedido |
| RN04 | Total do pedido = soma de (quantidade × preço unitário) de cada item × (1 − desconto ÷ 100) | Pedido.CalcularTotal() |
| RN05 | Só pedidos com status **Aberto** podem ter itens ou desconto alterados | Pedido.PodeEditar() |
| RN06 | Mudanças de status permitidas: Aberto → Aprovado → Faturado; Aberto → Cancelado; Aprovado → Cancelado | Pedido.AlterarStatus() |
| RN07 | Um pedido só pode ser aprovado se tiver pelo menos 1 item | Pedido.AlterarStatus() |
| RN08 | Um cliente que tem pedidos não pode ser excluído | Controller de Clientes + chave estrangeira com restrição |
| RN09 | O preço unitário do item é copiado do produto no momento em que o item é adicionado (preço histórico) | PedidosController.AdicionarItem |
| RN10 | Todo pedido novo nasce com status **Aberto**, desconto 0% e a data do dia | Pedido (valores padrão) |

---

## 4. Histórias de usuário (HU)

Formato: **Como** [usuário], **quero** [ação], **para** [benefício].
Critérios de aceitação no formato **Dado / Quando / Então**.

### HU01 – Acessar o sistema
**Como** vendedor, **quero** criar minha conta e entrar com e-mail e senha, **para** que só eu acesse os meus dados.

Critérios de aceitação:
- **Dado** que não tenho conta, **quando** eu me cadastrar com e-mail e senha válidos, **então** a conta é criada e eu entro no sistema.
- **Dado** que tenho conta, **quando** eu informar a senha errada, **então** vejo a mensagem "E-mail ou senha inválidos." (MSG10).
- **Dado** que não estou logado, **quando** eu tentar abrir a tela de Clientes, **então** sou levado para a tela de login.

### HU02 – Cadastrar clientes
**Como** vendedor, **quero** cadastrar e buscar meus clientes, **para** não precisar digitar os dados deles a cada pedido.

Critérios de aceitação:
- **Quando** eu salvar um cliente com todos os campos obrigatórios, **então** vejo "Cliente cadastrado com sucesso." (MSG01).
- **Quando** eu tentar salvar um CNPJ/CPF que já existe, **então** vejo "Já existe um cliente com este CNPJ/CPF." (MSG02).
- **Quando** eu digitar parte do nome na busca, **então** a lista mostra apenas os clientes que contêm esse texto.
- **Quando** eu tentar excluir um cliente que tem pedidos, **então** vejo "Este cliente possui pedidos e não pode ser excluído." (MSG03).

### HU03 – Cadastrar produtos
**Como** vendedor, **quero** cadastrar os produtos com código, descrição e preço, **para** usá-los nos pedidos.

Critérios de aceitação:
- **Quando** eu informar preço zero ou negativo, **então** vejo "O preço deve ser maior que zero." (MSG11).
- **Quando** eu informar um código que já existe, **então** vejo "Já existe um produto com este código." (MSG12).
- **Quando** eu marcar um produto como inativo, **então** ele deixa de aparecer na lista de produtos para adicionar ao pedido.

### HU04 – Registrar pedido
**Como** vendedor, **quero** registrar um pedido para um cliente, **para** controlar minhas vendas.

Critérios de aceitação:
- **Dado** um cliente cadastrado, **quando** eu criar um pedido, **então** ele nasce com status "Aberto", desconto 0% e data de hoje.
- **Quando** eu informar desconto de 10%, **então** o total é recalculado com o desconto.
- **Quando** eu informar desconto de 35%, **então** vejo "O desconto deve estar entre 0% e 30%." (MSG04).

### HU05 – Adicionar e remover itens
**Como** vendedor, **quero** adicionar e remover produtos do pedido, **para** montar o pedido do cliente.

Critérios de aceitação:
- **Quando** eu adicionar um produto com quantidade 2, **então** o item aparece na tabela com o subtotal e o total do pedido é atualizado.
- **Quando** eu adicionar um item com quantidade 0, **então** vejo "A quantidade deve ser maior que zero." (MSG13).
- **Dado** um pedido Aprovado, Faturado ou Cancelado, **quando** eu tentar alterar os itens, **então** vejo "Somente pedidos abertos podem ser alterados." (MSG06).

### HU06 – Alterar status do pedido
**Como** vendedor, **quero** aprovar, faturar ou cancelar um pedido, **para** saber em que etapa cada venda está.

Critérios de aceitação:
- **Dado** um pedido Aberto sem itens, **quando** eu clicar em Aprovar, **então** vejo "Adicione ao menos um item antes de aprovar o pedido." (MSG05).
- **Dado** um pedido Aberto com itens, **quando** eu clicar em Aprovar, **então** o status passa para "Aprovado".
- **Dado** um pedido Faturado, **então** os botões Aprovar, Faturar e Cancelar não aparecem.
- **Quando** eu clicar em Cancelar, **então** o sistema pede confirmação antes de cancelar (MSG14).

### HU07 – Consultar pedidos
**Como** vendedor, **quero** ver a lista de pedidos e filtrar por status, **para** encontrar rapidamente o que preciso acompanhar.

Critérios de aceitação:
- **Quando** eu abrir a lista de pedidos, **então** vejo número, cliente, data, status e total de cada pedido, do mais recente para o mais antigo.
- **Quando** eu escolher o filtro "Aprovado", **então** a lista mostra somente os pedidos aprovados.
- **Quando** não houver pedidos no filtro escolhido, **então** vejo "Nenhum pedido encontrado." (MSG15).

---

## 5. Backlog priorizado (visão ágil)

| Ordem | História | Estimativa (pontos) | Sprint planejada |
|---|---|---|---|
| 1 | HU01 – Acessar o sistema | 1 (já vem pronto no template) | 1 |
| 2 | HU02 – Cadastrar clientes | 3 | 1 |
| 3 | HU03 – Cadastrar produtos | 2 | 1 |
| 4 | HU04 – Registrar pedido | 3 | 2 |
| 5 | HU05 – Adicionar e remover itens | 5 | 2 |
| 6 | HU06 – Alterar status do pedido | 3 | 2 |
| 7 | HU07 – Consultar pedidos | 2 | 2 |

As mensagens citadas (MSGxx) estão no [catálogo de mensagens](04-ihc/mensagens.md).
