# Catálogo de Mensagens – PedidoFácil

Todas as mensagens que o sistema mostra ao usuário. Seguem os princípios de IHC de **dar resposta** (feedback após cada ação), **clareza** (linguagem simples, em português) e **empatia** (dizer o que aconteceu e o que fazer).

**Como aparecem na tela**

| Tipo | Aparência | Comportamento |
|---|---|---|
| Sucesso | Alerta verde no topo da página | Some sozinho após 4 segundos |
| Erro | Alerta vermelho no topo ou texto vermelho abaixo do campo | Fica na tela até o usuário corrigir |
| Confirmação | Janela de confirmação (OK / Cancelar) | Só executa a ação se o usuário clicar em OK |
| Informação | Texto cinza no lugar da lista vazia | Fixo |

## Mensagens

| Código | Tipo | Texto | Quando aparece | Regra |
|---|---|---|---|---|
| MSG01 | Sucesso | Cliente cadastrado com sucesso. | Ao salvar um novo cliente | RF02 |
| MSG02 | Erro | Já existe um cliente com este CNPJ/CPF. | CNPJ/CPF repetido | RN01 |
| MSG03 | Erro | Este cliente possui pedidos e não pode ser excluído. | Excluir cliente com pedidos | RN08 |
| MSG04 | Erro | O desconto deve estar entre 0% e 30%. | Desconto fora do limite | RN03 |
| MSG05 | Erro | Adicione ao menos um item antes de aprovar o pedido. | Aprovar pedido sem itens | RN07 |
| MSG06 | Erro | Somente pedidos abertos podem ser alterados. | Alterar itens/desconto de pedido não aberto | RN05 |
| MSG07 | Confirmação | Tem certeza que deseja excluir? Esta ação não pode ser desfeita. | Antes de excluir cliente, produto ou item | IHC: reversível |
| MSG08 | Erro geral | Ocorreu um erro inesperado. Tente novamente em instantes. | Qualquer falha não prevista (página de erro) | RNF05 |
| MSG09 | Erro | Página não encontrada. Verifique o endereço ou volte para o início. | Endereço ou registro inexistente (erro 404) | RNF05 |
| MSG10 | Erro | E-mail ou senha inválidos. | Login com dados errados | RF01 |
| MSG11 | Erro | O preço deve ser maior que zero. | Produto com preço ≤ 0 | RN02 |
| MSG12 | Erro | Já existe um produto com este código. | Código de produto repetido | RF03 |
| MSG13 | Erro | A quantidade deve ser maior que zero. | Item com quantidade < 1 | RN02 |
| MSG14 | Confirmação | Tem certeza que deseja cancelar este pedido? | Antes de cancelar um pedido | IHC: reversível |
| MSG15 | Informação | Nenhum pedido encontrado. | Lista de pedidos vazia | RF08 |
| MSG16 | Erro | O campo {nome do campo} é obrigatório. | Campo obrigatório vazio | RNF05 |
| MSG17 | Sucesso | Dados atualizados com sucesso. | Ao salvar uma edição | RF02, RF03 |
| MSG18 | Sucesso | Registro excluído com sucesso. | Após excluir cliente, produto ou item | RF02, RF03, RF05 |
| MSG19 | Sucesso | Item adicionado ao pedido. | Após adicionar item | RF05 |
| MSG20 | Sucesso | Status do pedido alterado para {novo status}. | Após aprovar, faturar ou cancelar | RF07 |
| MSG21 | Erro | Mudança de status não permitida. | Transição de status inválida | RN06 |
| MSG22 | Sucesso | Pedido criado com sucesso. Agora adicione os itens. | Após criar o pedido | RF04 |
| MSG23 | Sucesso | Produto cadastrado com sucesso. | Ao salvar um novo produto | RF03 |
| MSG24 | Informação | Nenhum cliente encontrado. | Busca de clientes sem resultado | RF02 |
| MSG25 | Erro | Informe um e-mail válido. | E-mail em formato inválido | RF02 |
