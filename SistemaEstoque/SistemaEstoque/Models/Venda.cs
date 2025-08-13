using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaEstoque.Models
{
    public class Venda
    {
        public int Id { get; set; }

        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }

        [Required(ErrorMessage = "Funcionário é obrigatório")]
        [Display(Name = "Funcionário")]
        public int FuncionarioId { get; set; }

        [Required(ErrorMessage = "Data da venda é obrigatória")]
        [Display(Name = "Data da Venda")]
        [DataType(DataType.DateTime)]
        public DateTime DataVenda { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Valor Total")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ValorTotal { get; set; }

        [Required(ErrorMessage = "Forma de pagamento é obrigatória")]
        [StringLength(50, ErrorMessage = "Forma de pagamento deve ter no máximo 50 caracteres")]
        [Display(Name = "Forma de Pagamento")]
        public string FormaPagamento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status é obrigatório")]
        [StringLength(30, ErrorMessage = "Status deve ter no máximo 30 caracteres")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Finalizada";

        [StringLength(500, ErrorMessage = "Observações devem ter no máximo 500 caracteres")]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        // Relacionamentos
        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey("FuncionarioId")]
        public virtual Funcionario Funcionario { get; set; } = null!;

        public virtual ICollection<ItemVenda> ItemVendas { get; set; } = new List<ItemVenda>();
    }
}
