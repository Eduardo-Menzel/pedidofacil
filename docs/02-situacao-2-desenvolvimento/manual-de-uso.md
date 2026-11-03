# Manual de Uso – PedidoFácil

Guia rápido para usar o sistema. Os dados de exemplo são fictícios.

> Os prints das telas serão adicionados nesta página na próxima etapa.

## 1. Criar conta e entrar

1. Abra o endereço do sistema e clique em **Criar conta**.
2. Informe seu e-mail e uma senha com pelo menos 6 caracteres, repita a senha e clique em **Criar conta**.
3. Você já entra no sistema. Nas próximas vezes, use **Entrar** com o mesmo e-mail e senha.
4. Para sair, clique em **Sair**, no canto superior direito.

Sem login, as telas de Clientes, Produtos e Pedidos não abrem: o sistema leva você para a tela de login.

## 2. Página inicial

Mostra quantos clientes, produtos ativos, pedidos abertos e pedidos aprovados existem. Clique em um dos quadros para ir direto à lista correspondente.

## 3. Clientes

| Ação | Como fazer |
|---|---|
| Cadastrar | **Clientes → + Novo cliente**, preencha os dados e clique em **Salvar**. O CNPJ/CPF e o telefone ganham formatação enquanto você digita. |
| Buscar | Digite parte do nome no campo de busca e clique em **Buscar**. |
| Editar | Clique em **Editar** na linha do cliente. |
| Ver pedidos do cliente | Clique em **Detalhes**. |
| Excluir | Clique em **Excluir** e confirme. Clientes que já têm pedidos não podem ser excluídos. |

Campos obrigatórios: **Nome** e **CNPJ/CPF**. O CNPJ/CPF não pode se repetir.

## 4. Produtos

| Ação | Como fazer |
|---|---|
| Cadastrar | **Produtos → + Novo produto**, informe código, descrição e preço (ex.: `45,90`) e clique em **Salvar**. |
| Editar | Clique em **Editar**. Mudar o preço não altera os pedidos já feitos. |
| Desativar | Em **Editar**, desligue a opção **Produto ativo**. Produtos inativos não aparecem para adicionar em pedidos. |
| Excluir | Clique em **Excluir** e confirme. Produtos que já foram vendidos não podem ser excluídos; desative-os. |

## 5. Pedidos

### Criar um pedido
1. **Pedidos → + Novo pedido**.
2. Escolha o cliente, informe o desconto se houver (de 0% a 30%) e clique em **Salvar e adicionar itens**.
3. O pedido é criado com status **Aberto** e a tela de detalhes abre.

### Adicionar e remover itens
1. Na área **Adicionar item**, escolha o produto e a quantidade. O subtotal aparece na hora.
2. Clique em **Adicionar**. Se o produto já estiver no pedido, a quantidade é somada.
3. Para tirar um item, clique em **Remover** e confirme.

### Desconto e total
No quadro **Resumo**, altere o desconto e clique em **Aplicar**. O total é recalculado:
**Total = subtotal × (1 − desconto ÷ 100)**.

### Mudar o status

| Status atual | Botões disponíveis |
|---|---|
| Aberto | Aprovar · Cancelar pedido |
| Aprovado | Faturar · Cancelar pedido |
| Faturado | nenhum (pedido concluído) |
| Cancelado | nenhum |

- Só é possível aprovar um pedido que tenha pelo menos 1 item.
- Depois de aprovado, o pedido não aceita mais mudanças nos itens nem no desconto.
- O cancelamento pede confirmação.

### Consultar pedidos
Em **Pedidos**, escolha um status no filtro e clique em **Filtrar**. A lista mostra número, cliente, data, status e total, do mais recente para o mais antigo.

## 6. Mensagens comuns

| Mensagem | O que significa / o que fazer |
|---|---|
| E-mail ou senha inválidos. | Confira o e-mail e a senha digitados. |
| Já existe um cliente com este CNPJ/CPF. | O cliente já está cadastrado; use a busca. |
| Este cliente possui pedidos e não pode ser excluído. | O histórico de vendas é preservado. |
| O desconto deve estar entre 0% e 30%. | Informe um valor de 0 a 30. |
| Adicione ao menos um item antes de aprovar o pedido. | Inclua um produto antes de aprovar. |
| Somente pedidos abertos podem ser alterados. | Pedidos aprovados, faturados ou cancelados ficam bloqueados. |
| Página não encontrada. | O endereço não existe; clique em **Voltar para o início**. |

A lista completa está no [catálogo de mensagens](../01-situacao-1-planejamento/04-ihc/mensagens.md).
