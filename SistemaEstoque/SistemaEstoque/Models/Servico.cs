using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaEstoque.Models
{
    public class Servico
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cliente é obrigatório")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Funcionário é obrigatório")]
        [Display(Name = "Funcionário Responsável")]
        public int FuncionarioId { get; set; }

        [Required(ErrorMessage = "Tipo de serviço é obrigatório")]
        [StringLength(100, ErrorMessage = "Tipo de serviço deve ter no máximo 100 caracteres")]
        [Display(Name = "Tipo de Serviço")]
        public string TipoServico { get; set; } = string.Empty;

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        [Display(Name = "Descrição do Serviço")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data do serviço é obrigatória")]
        [Display(Name = "Data do Serviço")]
        [DataType(DataType.DateTime)]
        public DateTime DataServico { get; set; } = DateTime.Now;

        [Display(Name = "Data de Conclusão")]
        [DataType(DataType.DateTime)]
        public DateTime? DataConclusao { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Valor do serviço deve ser maior que zero")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Valor do Serviço")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ValorServico { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        [StringLength(30, ErrorMessage = "Status deve ter no máximo 30 caracteres")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Agendado";

        [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        // Propriedades calculadas
        [NotMapped]
        [Display(Name = "Status Class")]
        public string StatusClass => Status switch
        {
            "Agendado" => "warning",
            "Em Andamento" => "info",
            "Concluído" => "success",
            "Cancelado" => "danger",
            _ => "secondary"
        };

        // Relacionamentos
        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; } = null!;

        [ForeignKey("FuncionarioId")]
        public virtual Funcionario Funcionario { get; set; } = null!;
    }
}
