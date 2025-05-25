using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;

namespace PBL_N2_1BI.Models
{
    public class SimulacaoViewModel
    {
        public int? Id { get; set; }

        [StringLength(50, ErrorMessage = "O Nome deve ter no máximo 100 caracteres.")]
        [Required(ErrorMessage = "O Nome é um campo obrigatório!")]
        public string Nome { get; set; }
        public DateTime? DataCriacaoAlteracao { get; set; }
        public MotorViewModel Motor { get; set; }
        public int? IdMotor { get; set; }
        public UsuarioViewModel Usuario { get; set; }
        public int? IdUsuario { get; set; }
    }
}
