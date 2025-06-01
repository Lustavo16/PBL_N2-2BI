using System;
using System.ComponentModel.DataAnnotations;

namespace PBL_N2_1BI.Models
{
    public class MotorViewModel : PadraoViewModel
    {
        [StringLength(50, ErrorMessage = "O Modelo deve ter no máximo 50 caracteres.")]
        [Required(ErrorMessage = "O Modelo é um campo obrigatório!")]
        public string Modelo { get; set; }

        [StringLength(50, ErrorMessage = "A Fabricante deve ter no máximo 50 caracteres.")]
        [Required(ErrorMessage = "A Fabricante é um campo obrigatório!")]
        public string Fabricante { get; set; }

        public double? TemperaturaSecagem { get; set; }

        [StringLength(50, ErrorMessage = "O Número de Série deve ter no máximo 50 caracteres.")]
        [Required(ErrorMessage = "O Número de Série é um campo obrigatório!")]
        public string NumeroDeSerie { get; set; }
    }
}
