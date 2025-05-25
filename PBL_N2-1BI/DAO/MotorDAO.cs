using PBL_N2_1BI.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Linq;

namespace PBL_N2_1BI.DAO
{
    public class MotorDAO
    {
        #region Sql

        public static SqlParameter[] CriaParametros(MotorViewModel motor)
        {
            SqlParameter[] parametros = new SqlParameter[5];
            parametros[0] = new SqlParameter("Id", motor.Id);
            parametros[1] = new SqlParameter("Modelo", motor.Modelo);
            parametros[2] = new SqlParameter("Fabricante", motor.Fabricante);
            if (motor.TemperaturaSecagem == null)
                parametros[3] = new SqlParameter("TemperaturaSecagem", DBNull.Value);
            else
                parametros[3] = new SqlParameter("TemperaturaSecagem", motor.TemperaturaSecagem);

            parametros[4] = new SqlParameter("NumeroDeSerie", motor.NumeroDeSerie);

            return parametros;
        }

       public void Inserir(MotorViewModel motor)
        {
            motor.Id = GerarId();

            string sql =
                "INSERT INTO Motor (Id, Modelo, Fabricante, TemperaturaSecagem, NumeroDeSerie) " +
                $"VALUES (@Id, @Modelo, @Fabricante, @TemperaturaSecagem, @NumeroDeSerie)";

            HelperDAO.ExecutaSQL(sql, CriaParametros(motor));
        }

        public void Alterar(MotorViewModel motor)
        {
            string sql = "UPDATE Motor SET Modelo=@Modelo, Fabricante=@Fabricante, TemperaturaSecagem=@TemperaturaSecagem, NumeroDeSerie=@NumeroDeSerie where Id=@Id";

            HelperDAO.ExecutaSQL(sql, CriaParametros(motor));
        }

        public void Excluir(int Id)
        {
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("Id", Id) };

            new SimulacaoDAO().ExcluirPorIdMotor(Id);
            string sql = "delete from Motor where Id= @Id";

            HelperDAO.ExecutaSQL(sql, parametros);
        }

        public List<MotorViewModel> ListarMotores(MotorViewModel motorConsulta)
        {
            List<MotorViewModel> listaMotores = new List<MotorViewModel>();

            string sql = "select * from motor";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, null);

            if (tabela.Rows.Count == 0)
                return new List<MotorViewModel>();
            else
            {
                foreach (DataRow row in tabela.Rows)
                {
                    listaMotores.Add(MontaModelConsulta(row));
                }
            }

            if (!string.IsNullOrEmpty(motorConsulta.Modelo))
                listaMotores = listaMotores.Where(xs => xs.Modelo.ToLower().Trim().Contains(motorConsulta.Modelo.ToLower().Trim())).ToList();
            if (!string.IsNullOrEmpty(motorConsulta.Fabricante))
                listaMotores = listaMotores.Where(xs => xs.Fabricante.ToLower().Trim().Contains(motorConsulta.Fabricante.ToLower().Trim())).ToList();
            if (!string.IsNullOrEmpty(motorConsulta.NumeroDeSerie))
                listaMotores = listaMotores.Where(xs => xs.NumeroDeSerie.ToLower().Trim().Contains(motorConsulta.NumeroDeSerie.ToLower().Trim())).ToList();

            return listaMotores;
        }

        public MotorViewModel PesquisarPorId(int Id)
        {
            MotorViewModel motor = new MotorViewModel();
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("Id", Id) };

            string sql = "select * from motor where Id= @Id";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, parametros);

            if (tabela.Rows.Count == 0)
                return null;
            else
            {
                motor = MontaModelConsulta(tabela.Rows[0]);
            }

            return motor;
        }

        public static MotorViewModel MontaModelConsulta(DataRow registro)
        {
            MotorViewModel motor = new MotorViewModel();

            motor.Id = Convert.ToInt32(registro["id"]);
            motor.Modelo = registro["modelo"].ToString();
            motor.Fabricante = registro["fabricante"].ToString();
            motor.NumeroDeSerie = registro["numeroDeSerie"].ToString();
            motor.TemperaturaSecagem = Convert.ToDouble(registro["temperaturaSecagem"]);

            return motor;
        }

        #endregion Sql

        #region Gerar Id

        public int GerarId()
        {
            string sql = "select MAX(Id) + 1 from motor";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, null);

            int idRetorno = 1;
            var retornoConsulta = tabela.Rows[0][0];

            if (retornoConsulta != DBNull.Value)
                idRetorno = Convert.ToInt32(retornoConsulta);

            return idRetorno;
        }

        #endregion
    }
}
