using System.Data.SqlClient;

namespace PBL_N2_1BI.DAO
{
    public class ConexaoBD
    {
        public static SqlConnection GetConexao()
        {
            //string strCon = "Data Source=tmdc1dbdv01;Initial Catalog=db_demonstracao; Integrated Security=True";
            //string strCon = "Data Source=DESKTOP-AKJIQLH\\SQLEXPRESS; Initial Catalog=AulaDB; Integrated Security=True";
            string strCon = "Data Source=LOCALHOST; user id=SA; password=123456; Initial Catalog=AulaDB;";
            SqlConnection conexao = new SqlConnection(strCon);
            conexao.Open();
            return conexao;
        }
    }
}
