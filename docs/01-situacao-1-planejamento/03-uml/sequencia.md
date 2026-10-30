# Diagramas de Sequência – PedidoFácil

Os diagramas mostram a troca de mensagens entre as camadas do padrão MVC: **View** (tela) → **Controller** → **Model** / **DbContext** (banco) → de volta para a **View**.

## 1. Adicionar item ao pedido (UC05)

```mermaid
sequenceDiagram
    autonumber
    actor V as Vendedor
    participant T as View Detalhes do Pedido
    participant C as PedidosController
    participant DB as ApplicationDbContext
    participant P as Pedido (Model)

    V->>T: Escolhe o produto, digita a quantidade e clica em "Adicionar"
    T->>C: POST /Pedidos/AdicionarItem (pedidoId, produtoId, quantidade)
    C->>DB: Busca o pedido (com itens) e o produto
    DB-->>C: Pedido e Produto

    alt Pedido ou produto não encontrado
        C-->>T: NotFound (página 404 – MSG09)
    else Pedido encontrado
        C->>P: PodeEditar()
        P-->>C: verdadeiro ou falso
        alt Pedido não está Aberto (RN05)
            C-->>T: Redireciona para Detalhes com erro MSG06
        else Quantidade menor que 1 (RN02)
            C-->>T: Redireciona para Detalhes com erro MSG13
        else Dados válidos
            C->>P: Itens.Add(novo ItemPedido com o preço atual do produto) (RN09)
            C->>DB: SaveChanges()
            DB-->>C: Item gravado
            C-->>T: Redireciona para Detalhes com sucesso MSG19
        end
    end

    T->>P: CalcularTotal() (RN04)
    P-->>T: Total com desconto
    T-->>V: Mostra a lista de itens, a mensagem e o total atualizado
```

## 2. Aprovar pedido (UC06)

```mermaid
sequenceDiagram
    autonumber
    actor V as Vendedor
    participant T as View Detalhes do Pedido
    participant C as PedidosController
    participant DB as ApplicationDbContext
    participant P as Pedido (Model)

    V->>T: Clica em "Aprovar"
    T->>C: POST /Pedidos/AlterarStatus (id, Aprovado)
    C->>DB: Busca o pedido com os itens
    DB-->>C: Pedido
    C->>P: AlterarStatus(Aprovado)

    alt Pedido sem itens (RN07)
        P-->>C: Erro "Adicione ao menos um item antes de aprovar o pedido."
        C-->>T: Redireciona para Detalhes com erro MSG05
    else Transição não permitida (RN06)
        P-->>C: Erro "Mudança de status não permitida."
        C-->>T: Redireciona para Detalhes com erro MSG21
    else Transição válida
        P-->>C: Status = Aprovado
        C->>DB: SaveChanges()
        DB-->>C: Pedido gravado
        C-->>T: Redireciona para Detalhes com sucesso MSG20
    end

    T-->>V: Mostra o novo status e somente os botões permitidos (Faturar, Cancelar)
```

As mensagens (MSGxx) estão no [catálogo de mensagens](../04-ihc/mensagens.md).
