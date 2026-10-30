# Diagrama de Classes – PedidoFácil

## 1. Classes de domínio (persistência)

Estas classes ficam na pasta `Models` do projeto (o **M** do MVC) e viram tabelas no banco de dados.

```mermaid
classDiagram
    direction LR

    class Cliente {
        +int Id
        +string Nome
        +string CnpjCpf
        +string Email
        +string Telefone
        +string Cidade
        +string UF
        +DateTime DataCadastro
        +ICollection~Pedido~ Pedidos
    }

    class Produto {
        +int Id
        +string Codigo
        +string Descricao
        +decimal PrecoUnitario
        +bool Ativo
    }

    class Pedido {
        +int Id
        +int ClienteId
        +DateTime DataPedido
        +StatusPedido Status
        +decimal DescontoPercentual
        +string Observacao
        +ICollection~ItemPedido~ Itens
        +CalcularSubtotal() decimal
        +CalcularTotal() decimal
        +PodeEditar() bool
        +AlterarStatus(StatusPedido novoStatus) void
    }

    class ItemPedido {
        +int Id
        +int PedidoId
        +int ProdutoId
        +int Quantidade
        +decimal PrecoUnitario
        +CalcularSubtotal() decimal
    }

    class StatusPedido {
        <<enumeration>>
        Aberto
        Aprovado
        Faturado
        Cancelado
    }

    Cliente "1" --> "0..*" Pedido : faz
    Pedido "1" *-- "0..*" ItemPedido : contém
    ItemPedido "0..*" --> "1" Produto : refere-se a
    Pedido ..> StatusPedido : usa
```

### Explicação

| Elemento | Significado |
|---|---|
| Cliente 1 → 0..* Pedido | Um cliente pode ter nenhum ou vários pedidos; cada pedido pertence a um único cliente |
| Pedido 1 ◆ 0..* ItemPedido | **Composição**: o item só existe dentro do pedido. Se o pedido for excluído, os itens também são |
| ItemPedido 0..* → 1 Produto | Cada item aponta para um produto; um produto pode aparecer em vários itens |
| StatusPedido | Enumeração com os valores Aberto (0), Aprovado (1), Faturado (2) e Cancelado (3) |

### Métodos (regras de negócio dentro do Model)

| Método | Regra | O que faz |
|---|---|---|
| `ItemPedido.CalcularSubtotal()` | RN04 | Quantidade × PrecoUnitario |
| `Pedido.CalcularSubtotal()` | RN04 | Soma dos subtotais de todos os itens |
| `Pedido.CalcularTotal()` | RN04 | Subtotal × (1 − DescontoPercentual ÷ 100), arredondado em 2 casas |
| `Pedido.PodeEditar()` | RN05 | Retorna verdadeiro somente se o status for Aberto |
| `Pedido.AlterarStatus(novo)` | RN06, RN07 | Valida a transição e o número de itens; se for inválida, gera um erro com a mensagem adequada |

Colocar as regras dentro da classe `Pedido` deixa o Controller mais simples e permite testar as regras com testes unitários, sem precisar de banco de dados nem de tela.

**Campos opcionais:** `Email`, `Telefone`, `Cidade`, `UF` (Cliente) e `Observacao` (Pedido). Os demais são obrigatórios. Os tamanhos e domínios de cada campo estão no [dicionário de dados](../05-dados/dicionario-de-dados.md).

---

## 2. Classes da arquitetura MVC

Visão de como as camadas se relacionam. Os **Controllers** recebem as requisições, usam o **DbContext** para ler e gravar os **Models** e devolvem as **Views** (telas Razor).

```mermaid
classDiagram
    direction TB

    class Controller {
        <<ASP.NET Core>>
    }
    class ClientesController {
        +Index(string busca)
        +Create()
        +Edit(int id)
        +Delete(int id)
    }
    class ProdutosController {
        +Index()
        +Create()
        +Edit(int id)
        +Delete(int id)
    }
    class PedidosController {
        +Index(StatusPedido status)
        +Create()
        +Details(int id)
        +AtualizarDesconto(int id, decimal desconto)
        +AdicionarItem(int pedidoId, int produtoId, int quantidade)
        +RemoverItem(int itemId)
        +AlterarStatus(int id, StatusPedido novoStatus)
    }
    class ApplicationDbContext {
        +DbSet~Cliente~ Clientes
        +DbSet~Produto~ Produtos
        +DbSet~Pedido~ Pedidos
        +DbSet~ItemPedido~ ItensPedido
        +SaveChanges() int
    }

    Controller <|-- ClientesController
    Controller <|-- ProdutosController
    Controller <|-- PedidosController
    ClientesController --> ApplicationDbContext
    ProdutosController --> ApplicationDbContext
    PedidosController --> ApplicationDbContext
```

Todos os Controllers têm o atributo `[Authorize]`: só usuários logados acessam (RNF04). A autenticação (criar conta, login e logout) usa as telas prontas do **ASP.NET Core Identity**.
