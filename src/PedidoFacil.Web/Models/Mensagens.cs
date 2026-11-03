namespace PedidoFacil.Web.Models;

/// <summary>
/// Catálogo de mensagens do sistema. Cada constante corresponde a um código
/// do documento docs/01-situacao-1-planejamento/04-ihc/mensagens.md.
/// </summary>
public static class Mensagens
{
    public const string ClienteCadastrado = "Cliente cadastrado com sucesso.";                                  // MSG01
    public const string CnpjCpfDuplicado = "Já existe um cliente com este CNPJ/CPF.";                            // MSG02
    public const string ClienteComPedidos = "Este cliente possui pedidos e não pode ser excluído.";              // MSG03
    public const string DescontoInvalido = "O desconto deve estar entre 0% e 30%.";                              // MSG04
    public const string PedidoSemItens = "Adicione ao menos um item antes de aprovar o pedido.";                 // MSG05
    public const string PedidoNaoEditavel = "Somente pedidos abertos podem ser alterados.";                      // MSG06
    public const string ConfirmarExclusao = "Tem certeza que deseja excluir? Esta ação não pode ser desfeita.";   // MSG07
    public const string ErroInesperado = "Ocorreu um erro inesperado. Tente novamente em instantes.";            // MSG08
    public const string PaginaNaoEncontrada = "Página não encontrada. Verifique o endereço ou volte para o início."; // MSG09
    public const string LoginInvalido = "E-mail ou senha inválidos.";                                             // MSG10
    public const string PrecoInvalido = "O preço deve ser maior que zero.";                                       // MSG11
    public const string CodigoDuplicado = "Já existe um produto com este código.";                               // MSG12
    public const string QuantidadeInvalida = "A quantidade deve ser maior que zero.";                            // MSG13
    public const string ConfirmarCancelamento = "Tem certeza que deseja cancelar este pedido?";                  // MSG14
    public const string NenhumPedido = "Nenhum pedido encontrado.";                                               // MSG15
    public const string CampoObrigatorio = "O campo {0} é obrigatório.";                                          // MSG16
    public const string DadosAtualizados = "Dados atualizados com sucesso.";                                      // MSG17
    public const string RegistroExcluido = "Registro excluído com sucesso.";                                      // MSG18
    public const string ItemAdicionado = "Item adicionado ao pedido.";                                            // MSG19
    public const string StatusAlterado = "Status do pedido alterado para {0}.";                                   // MSG20
    public const string TransicaoInvalida = "Mudança de status não permitida.";                                   // MSG21
    public const string PedidoCriado = "Pedido criado com sucesso. Agora adicione os itens.";                     // MSG22
    public const string ProdutoCadastrado = "Produto cadastrado com sucesso.";                                    // MSG23
    public const string NenhumCliente = "Nenhum cliente encontrado.";                                             // MSG24
    public const string EmailInvalido = "Informe um e-mail válido.";                                              // MSG25
    public const string ProdutoEmUso = "Este produto já foi usado em pedidos e não pode ser excluído. Marque-o como inativo."; // MSG26
    public const string DocumentoInvalido = "Informe um CPF (11 dígitos) ou CNPJ (14 dígitos) válido.";          // MSG27
    public const string SelecioneProduto = "Selecione um produto.";                                               // MSG28
    public const string ProdutoInativo = "Este produto está inativo e não pode ser adicionado ao pedido.";       // MSG29
    public const string ContaCriada = "Conta criada com sucesso. Bem-vindo ao PedidoFácil!";                     // MSG30
    public const string TamanhoTexto = "O campo {0} deve ter entre {2} e {1} caracteres.";                       // MSG31
    public const string ItemRemovido = "Item removido do pedido.";                                                // MSG32
    public const string DescontoAtualizado = "Desconto atualizado.";                                              // MSG33
    public const string SenhasDiferentes = "As senhas não conferem.";                                             // MSG34
    public const string NenhumProduto = "Nenhum produto cadastrado.";                                             // MSG35
    public const string SelecioneCliente = "Selecione um cliente.";                                               // MSG36
}
