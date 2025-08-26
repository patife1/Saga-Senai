using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaEstoque.Models
{
    public class ItemVenda
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Venda é obrigatória")]
        [Display(Name = "Venda")]
        public int VendaId { get; set; }

        [Required(ErrorMessage = "Produto é obrigatório")]
        [Display(Name = "Produto")]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "Preço unitário é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço unitário deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Preço Unitário")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PrecoUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Preço de Compra")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PrecoCompra { get; set; }

        // Relacionamentos
        public virtual Venda Venda { get; set; } = null!;

        public virtual Produto Produto { get; set; } = null!;
    }
}
