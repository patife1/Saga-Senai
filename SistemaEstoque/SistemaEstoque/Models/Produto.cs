using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaEstoque.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(150, ErrorMessage = "Nome deve ter no máximo 150 caracteres")]
        [Display(Name = "Nome do Produto")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "Categoria é obrigatória")]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantidade deve ser positiva")]
        [Display(Name = "Quantidade em Estoque")]
        public int QuantidadeEstoque { get; set; }

        [Required(ErrorMessage = "Preço de compra é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço de compra deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Preço de Compra")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PrecoCompra { get; set; }

        [Required(ErrorMessage = "Preço de venda é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço de venda deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Preço de Venda")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PrecoVenda { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Estoque mínimo deve ser positivo")]
        [Display(Name = "Estoque Mínimo")]
        public int EstoqueMinimo { get; set; } = 0;

        [StringLength(50, ErrorMessage = "Código de barras deve ter no máximo 50 caracteres")]
        [Display(Name = "Código de Barras")]
        public string? CodigoBarras { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Propriedades calculadas
        [NotMapped]
        [Display(Name = "Margem de Lucro")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public decimal MargemLucro => PrecoCompra > 0 ? ((PrecoVenda - PrecoCompra) / PrecoCompra) : 0;

        [NotMapped]
        [Display(Name = "Estoque Baixo")]
        public bool EstoqueBaixo => QuantidadeEstoque <= EstoqueMinimo;

        [NotMapped]
        [Display(Name = "Valor Total Estoque")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ValorTotalEstoque => QuantidadeEstoque * PrecoVenda;

        // Relacionamentos
        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; } = null!;
        
        public virtual ICollection<ItemVenda> ItemVendas { get; set; } = new List<ItemVenda>();
    }
}
