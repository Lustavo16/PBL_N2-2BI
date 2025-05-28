using System.ComponentModel.DataAnnotations;

namespace PBL_N2_1BI.Models
{
    public class UsuarioViewModel
    {
        public int? Id { get; set; }

        [StringLength(50, ErrorMessage = "O Login deve ter no máximo 50 caracteres.")]
        [Required(ErrorMessage = "O Login é um campo obrigatório!")]
        public string Login { get; set; }

        [StringLength(100, ErrorMessage = "O Nome deve ter no máximo 100 caracteres.")]
        [Required(ErrorMessage = "O Nome é um campo obrigatório!")]
        public string Nome { get; set; }

        [StringLength(100, ErrorMessage = "O E-mail deve ter no máximo 100 caracteres.")]
        [Required(ErrorMessage = "O E-mail é um campo obrigatório!")]
        [EmailAddress(ErrorMessage = "O formato do E-mail é inválido.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "A Senha é um campo obrigatório!")]
        public string Senha { get; set; }

        public bool IsPrimeiroAcesso { get; set; }

        public string FotoBase64 { get; set; }

        public byte[] Foto { get; set; }
    }
}
