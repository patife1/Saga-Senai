using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaEstoque.Models
{
    public class Funcionario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(150, ErrorMessage = "Nome deve ter no máximo 150 caracteres")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "CPF é obrigatório")]
        [StringLength(14, ErrorMessage = "CPF deve ter no máximo 14 caracteres")]
        [Display(Name = "CPF")]
        public string CPF { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(15, ErrorMessage = "Telefone deve ter no máximo 15 caracteres")]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Cargo é obrigatório")]
        [StringLength(100, ErrorMessage = "Cargo deve ter no máximo 100 caracteres")]
        [Display(Name = "Cargo")]
        public string Cargo { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Salário deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Salário")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal? Salario { get; set; }

        [Required(ErrorMessage = "Data de admissão é obrigatória")]
        [Display(Name = "Data de Admissão")]
        [DataType(DataType.Date)]
        public DateTime DataAdmissao { get; set; } = DateTime.Now;

        [Display(Name = "Data de Demissão")]
        [DataType(DataType.Date)]
        public DateTime? DataDemissao { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        [Display(Name = "Usuário do Sistema")]
        public string? UserId { get; set; }

        // Relacionamentos
        public virtual ICollection<Venda> Vendas { get; set; } = new List<Venda>();
        public virtual ICollection<Servico> Servicos { get; set; } = new List<Servico>();
    }
}
