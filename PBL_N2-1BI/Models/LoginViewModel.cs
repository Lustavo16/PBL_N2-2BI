using System.ComponentModel.DataAnnotations;

namespace PBL_N2_1BI.Models
{
    public class LoginViewModel
    {
        [StringLength(50, ErrorMessage = "O Usuário deve ter no máximo 50 caracteres.")]
        public string Login { get; set; }

        [StringLength(12, ErrorMessage = "O Usuário deve ter no máximo 12 caracteres.")]
        public string Senha { get; set; }

        public string IsUsuarioValido { get; set; }

        public string FotoBase64 { get; set; }
    }
}
