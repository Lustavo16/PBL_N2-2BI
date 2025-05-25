namespace PBL_N2_1BI.Crypto
{
    public class Hash
    {
        public static string HashPassword(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        public static bool VerificarSenha(string senhaDigitada, string hashSalvo)
        {
            return BCrypt.Net.BCrypt.Verify(senhaDigitada, hashSalvo);
        }
    }
}
