using PBL_N2_1BI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PBL_N2_1BI.DAO
{
    public class RegistroDAO
    {
        public static SqlParameter[] CriaParametros(RegistroViewModel registro)
        {
            SqlParameter[] parametros = new SqlParameter[5];
            parametros[0] = new SqlParameter("Id", registro.Id);
            parametros[1] = new SqlParameter("DataRegistro", registro.DataRegistro);
            parametros[3] = new SqlParameter("ValorTemperatura", registro.ValorTemperatura);

            return parametros;
        }

        public void InserirRegistro(RegistroViewModel registro)
        {
            registro.Id = GerarId();

            string sql =
                "INSERT INTO Registros " +
                $"VALUES (@Id, @DataRegistro, @ValorTemperatura)";

            HelperDAO.ExecutaSQL(sql, CriaParametros(registro));
        }

        public int GerarId()
        {
            string sql = "select MAX(Id) + 1 from Registros";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, null);

            int idRetorno = 1;
            var retornoConsulta = tabela.Rows[0][0];

            if (retornoConsulta != DBNull.Value)
                idRetorno = Convert.ToInt32(retornoConsulta);

            return idRetorno;
        }

        public List<RegistroViewModel> ListarRegistros()
        {
            List<RegistroViewModel> listaRegistros = new List<RegistroViewModel>();

            string sql = "select * from Registros";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, null);

            if (tabela.Rows.Count == 0)
                return new List<RegistroViewModel>();
            else
            {
                foreach (DataRow row in tabela.Rows)
                {
                    listaRegistros.Add(MontaModelConsulta(row));
                }
            }

            return listaRegistros;
        }

        public static RegistroViewModel MontaModelConsulta(DataRow registro)
        {
            RegistroViewModel registroRetorno = new RegistroViewModel();

            registroRetorno.Id = Convert.ToInt32(registro["id"]);
            registroRetorno.DataRegistro = Convert.ToDateTime(registro["Data_Registro"]);
            registroRetorno.ValorTemperatura = Convert.ToDouble(registro["Valor_Temperatura"]);

            return registroRetorno;
        }


        public void SalvaDados()
        {
            Random random = new Random();
            List<RegistroViewModel> listaRegistros = new List<RegistroViewModel>();

            for (int i = 1; i <= 4; i++)
            {
                for (int j = 1; j <= 31; j++)
                {
                    if ((i == 1 || i == 3 || i == 4) && j == 31)
                    {
                        break;
                    }
                    else if (i == 2 && j == 29)
                    {
                        break;
                    }

                    listaRegistros.Add(new RegistroViewModel()
                    {
                        DataRegistro = new DateTime(2025, i, j),
                        ValorTemperatura = random.Next(41, 51),
                    });
                }
            }

            foreach (RegistroViewModel item in listaRegistros)
            {
                InserirRegistro(item);
            }

        }
    }
}
