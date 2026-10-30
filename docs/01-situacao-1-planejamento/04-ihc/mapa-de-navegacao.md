# Mapa de Navegação – PedidoFácil

Mostra como o usuário se movimenta entre as telas. As telas de negócio só podem ser acessadas depois do login.

```mermaid
flowchart TD
    L["Login"] -->|"Criar conta"| R["Criar conta"]
    R -->|"Conta criada"| HOME
    L -->|"Entrar"| HOME["Início"]

    HOME --> MC["Clientes - lista e busca"]
    HOME --> MP["Produtos - lista"]
    HOME --> MPE["Pedidos - lista e filtro por status"]

    MC --> CN["Novo cliente"]
    MC --> CE["Editar cliente"]
    MC --> CD["Detalhes do cliente"]
    MC --> CX["Confirmar exclusão"]
    CN -->|"Salvar ou Voltar"| MC
    CE -->|"Salvar ou Voltar"| MC
    CX -->|"Excluir ou Cancelar"| MC

    MP --> PN["Novo produto"]
    MP --> PE["Editar produto"]
    MP --> PX["Confirmar exclusão"]
    PN -->|"Salvar ou Voltar"| MP
    PE -->|"Salvar ou Voltar"| MP
    PX -->|"Excluir ou Cancelar"| MP

    MPE --> PDN["Novo pedido"]
    PDN -->|"Salvar"| PDD["Detalhes do pedido"]
    MPE -->|"Detalhes"| PDD
    PDD -->|"Adicionar ou remover item, aplicar desconto, mudar status"| PDD
    PDD -->|"Voltar"| MPE

    HOME -->|"Sair"| L
```

**Menu superior** (presente em todas as telas após o login): Início · Clientes · Produtos · Pedidos · e-mail do usuário · Sair.

| Tela | Endereço (rota MVC) | Controller / Ação |
|---|---|---|
| Login | /Identity/Account/Login | ASP.NET Core Identity |
| Criar conta | /Identity/Account/Register | ASP.NET Core Identity |
| Início | / | HomeController.Index |
| Clientes | /Clientes | ClientesController.Index |
| Novo / editar cliente | /Clientes/Create · /Clientes/Edit/{id} | ClientesController.Create · Edit |
| Produtos | /Produtos | ProdutosController.Index |
| Pedidos | /Pedidos?status={status} | PedidosController.Index |
| Novo pedido | /Pedidos/Create | PedidosController.Create |
| Detalhes do pedido | /Pedidos/Details/{id} | PedidosController.Details |
| Erro / página não encontrada | /Home/Error · /Home/StatusCode/404 | HomeController |
