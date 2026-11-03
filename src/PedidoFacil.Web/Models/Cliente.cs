using System.ComponentModel.DataAnnotations;

namespace PedidoFacil.Web.Models;

/// <summary>Cliente para o qual o vendedor emite pedidos (tabela Clientes).</summary>
public class Cliente
{
    public int Id { get; set; }

    [Display(Name = "Nome")]
    [Required(ErrorMessage = Mensagens.CampoObrigatorio)]
    [StringLength(100, MinimumLength = 3, ErrorMessage = Mensagens.TamanhoTexto)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>RN01: obrigatório e único.</summary>
    [Display(Name = "CNPJ/CPF")]
    [Required(ErrorMessage = Mensagens.CampoObrigatorio)]
    [StringLength(18, MinimumLength = 11, ErrorMessage = Mensagens.TamanhoTexto)]
    [RegularExpression(@"^(\d{3}\.?\d{3}\.?\d{3}-?\d{2}|\d{2}\.?\d{3}\.?\d{3}/?\d{4}-?\d{2})$",
        ErrorMessage = Mensagens.DocumentoInvalido)]
    public string CnpjCpf { get; set; } = string.Empty;

    [Display(Name = "E-mail")]
    [EmailAddress(ErrorMessage = Mensagens.EmailInvalido)]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? Email { get; set; }

    [Display(Name = "Telefone")]
    [StringLength(20, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? Telefone { get; set; }

    [Display(Name = "Cidade")]
    [StringLength(60, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? Cidade { get; set; }

    [Display(Name = "UF")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "Informe a sigla do estado com 2 letras.")]
    public string? UF { get; set; }

    [Display(Name = "Data de cadastro")]
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    /// <summary>Texto "Cidade/UF" usado nas listas.</summary>
    public string CidadeUf =>
        string.IsNullOrWhiteSpace(Cidade) ? (UF ?? string.Empty)
        : string.IsNullOrWhiteSpace(UF) ? Cidade
        : $"{Cidade}/{UF}";
}
