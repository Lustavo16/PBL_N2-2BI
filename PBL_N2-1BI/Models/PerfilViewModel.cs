using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PBL_N2_1BI.Models
{
    public class PerfilViewModel : PadraoViewModel
    {
        [StringLength(100, ErrorMessage = "O Nome deve ter no máximo 100 caracteres.")]
        [Required(ErrorMessage = "O Nome é um campo obrigatório!")]
        public string Nome { get; set; }

        public string Permissoes { get; set; }
    }
}
