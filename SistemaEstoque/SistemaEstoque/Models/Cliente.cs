using System.ComponentModel.DataAnnotations;

namespace SistemaEstoque.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(150, ErrorMessage = "Nome deve ter no máximo 150 caracteres")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(14, ErrorMessage = "CPF deve ter no máximo 14 caracteres")]
        [Display(Name = "CPF")]
        public string? CPF { get; set; }

        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(15, ErrorMessage = "Telefone deve ter no máximo 15 caracteres")]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        [StringLength(200, ErrorMessage = "Endereço deve ter no máximo 200 caracteres")]
        [Display(Name = "Endereço")]
        public string? Endereco { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        // Relacionamentos
        public virtual ICollection<Venda> Vendas { get; set; } = new List<Venda>();
        public virtual ICollection<Servico> Servicos { get; set; } = new List<Servico>();
    }
}
