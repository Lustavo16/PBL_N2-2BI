using System;
using System.Data.SqlClient;

namespace PBL_N2_1BI.DAO
{
    public class ConexaoBD
    {
        public static SqlConnection? GetConexao()
        {
            string strCon = "Data Source=DESKTOP-AKJIQLH\\SQLEXPRESS; Initial Catalog=AulaDB; Integrated Security=True";
            //string strCon = "Data Source=LOCALHOST; user id=SA; password=123456; Initial Catalog=teste1;";
            //string strCon = "Data Source=LOCALHOST; Integrated Security=True Initial Catalog=AulaDB;";

            try
            {
                SqlConnection conexao = new SqlConnection(strCon);
                conexao.Open();
                return conexao;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Erro na conexão: " + ex.Message);
                return null;
            }
        }
    }
}
