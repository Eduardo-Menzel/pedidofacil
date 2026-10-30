# Diagrama de Atividades – Fluxo do Pedido

Mostra o caminho de um pedido desde a criação até o faturamento ou cancelamento, com os pontos de decisão e as regras de negócio aplicadas.

```mermaid
flowchart TD
    I(("Início")) --> A["Vendedor cria pedido para um cliente"]
    A --> A1["Sistema grava pedido: status Aberto, desconto 0%, data de hoje (RN10)"]
    A1 --> B["Vendedor escolhe produto e quantidade"]
    B --> C{"Quantidade maior ou igual a 1? (RN02)"}
    C -- Não --> C1["Mostra MSG13"] --> B
    C -- Sim --> D["Sistema grava item com o preço atual do produto (RN09)"]
    D --> E["Sistema recalcula o total com desconto (RN04)"]
    E --> F{"Adicionar mais itens?"}
    F -- Sim --> B
    F -- Não --> G{"Próxima ação"}

    G -- "Alterar desconto" --> G1{"Desconto entre 0% e 30%? (RN03)"}
    G1 -- Não --> G2["Mostra MSG04"] --> G
    G1 -- Sim --> E

    G -- Aprovar --> H{"Pedido tem pelo menos 1 item? (RN07)"}
    H -- Não --> H1["Mostra MSG05"] --> B
    H -- Sim --> J["Status = Aprovado"]

    G -- Cancelar --> K["Sistema pede confirmação (MSG14)"]

    J --> L{"Faturar ou cancelar?"}
    L -- Faturar --> M["Status = Faturado"]
    L -- Cancelar --> K

    K --> K1{"Vendedor confirmou?"}
    K1 -- Não --> R["Pedido continua com o status atual"]
    K1 -- Sim --> N["Status = Cancelado"]

    M --> Z(("Fim"))
    N --> Z
    R --> Z
```

**Observações**

- Depois de **Aprovado**, o pedido não aceita mais alterações de itens ou desconto (RN05).
- **Faturado** e **Cancelado** são estados finais: nenhum botão de ação aparece na tela.
- Se o vendedor desistir do cancelamento a partir de um pedido Aprovado, o pedido continua Aprovado.
