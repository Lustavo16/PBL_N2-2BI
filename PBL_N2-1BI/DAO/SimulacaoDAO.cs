using PBL_N2_1BI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PBL_N2_1BI.DAO
{
    public class SimulacaoDAO
    {
        #region Sql

        public static SqlParameter[] CriaParametros(SimulacaoViewModel Simulacao)
        {
            SqlParameter[] parametros = new SqlParameter[10];
            parametros[0] = new SqlParameter("Id", Simulacao.Id);
            parametros[1] = new SqlParameter("IdUsuario", Simulacao.IdUsuario);
            parametros[2] = new SqlParameter("IdMotor", Simulacao.IdMotor);
            parametros[3] = new SqlParameter("Nome", Simulacao.Nome);
            parametros[4] = new SqlParameter("DataCriacaoAlteracao", Simulacao.DataCriacaoAlteracao);

            parametros[5] = new SqlParameter("Media", (object?)Simulacao.Media ?? DBNull.Value);
            parametros[6] = new SqlParameter("Min", (object?)Simulacao.Min ?? DBNull.Value);
            parametros[7] = new SqlParameter("Max", (object?)Simulacao.Max ?? DBNull.Value);

            parametros[8] = new SqlParameter("DataInicio", (object?)Simulacao.DataInicio ?? DBNull.Value);
            parametros[9] = new SqlParameter("DataFim", (object?)Simulacao.DataFim ?? DBNull.Value);

            return parametros;
        }

        public void Inserir(SimulacaoViewModel Simulacao)
        {
            Simulacao.Id = GerarId();
            Simulacao.DataCriacaoAlteracao = DateTime.Now;

            string sql =
                "INSERT INTO Simulacao (Id, IdUsuario, IdMotor, Nome, DataCriacaoAlteracao, Media, Min, Max, DataInicio, DataFim) " +
                $"VALUES (@Id, @IdUsuario, @IdMotor, @Nome, @DataCriacaoAlteracao, @Media, @Min, @Max, @DataInicio, @DataFim)";

            HelperDAO.ExecutaSQL(sql, CriaParametros(Simulacao));
        }

        public void Alterar(SimulacaoViewModel Simulacao)
        {
            Simulacao.DataCriacaoAlteracao = DateTime.Now;

            string sql = "UPDATE Simulacao SET IdUsuario=@IdUsuario, IdMotor=@IdMotor, Nome=@Nome, DataCriacaoAlteracao=@DataCriacaoAlteracao, Media=@Media, Min=@Min, Max=@Max, DataInicio=@DataInicio, DataFim=@DataFim where Id=@Id";

            HelperDAO.ExecutaSQL(sql, CriaParametros(Simulacao));
        }

        public void Excluir(int Id)
        {
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("Id", Id) };

            string sql = "delete from Simulacao where Id= @Id";

            HelperDAO.ExecutaSQL(sql, parametros);
        }

        public List<SimulacaoViewModel> ListarSimulacao(SimulacaoViewModel simulacaoConsulta)
        {
            List<SimulacaoViewModel> listaSimulacao = new List<SimulacaoViewModel>();

            string sql = "select * from Simulacao";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, null);

            if (tabela.Rows.Count == 0)
                return new List<SimulacaoViewModel>();
            else
            {
                foreach (DataRow row in tabela.Rows)
                {
                    listaSimulacao.Add(MontaModelConsulta(row));
                }
            }

            if (!string.IsNullOrEmpty(simulacaoConsulta.Nome))
                listaSimulacao = listaSimulacao.Where(xs => xs.Nome.ToLower().Trim().Contains(simulacaoConsulta.Nome.ToLower().Trim())).ToList();
            if (simulacaoConsulta.IdMotor.HasValue)
                listaSimulacao = listaSimulacao.Where(xs => xs.IdMotor.Equals(simulacaoConsulta.IdMotor)).ToList();
            if (simulacaoConsulta.IdUsuario.HasValue)
                listaSimulacao = listaSimulacao.Where(xs => xs.IdUsuario.Equals(simulacaoConsulta.IdUsuario)).ToList();

            return listaSimulacao;
        }

        public SimulacaoViewModel PesquisarPorId(int Id)
        {
            SimulacaoViewModel Simulacao = new SimulacaoViewModel();
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("Id", Id) };

            string sql = "select * from Simulacao where Id= @Id";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, parametros);

            if (tabela.Rows.Count == 0)
                return null;
            else
            {
                Simulacao = MontaModelConsulta(tabela.Rows[0]);
            }

            return Simulacao;
        }

        public void ExcluirPorIdMotor(int Id)
        {
            List<int> listaSimulacao = new List<int>();

            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("IdMotor", Id) };

            string sql = "delete from Simulacao where IdMotor=@IdMotor";

            DataTable tabela = HelperDAO.ExecutaSelect(sql, parametros);
        }

        public void ExcluirPorIdUsuario(int Id)
        {
            List<int> listaSimulacao = new List<int>();

            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("IdUsuario", Id) };

            string sql = "delete from Simulacao where IdUsuario=@IdUsuario";

            DataTable tabela = HelperDAO.ExecutaSelect(sql, parametros);
        }

        public static SimulacaoViewModel MontaModelConsulta(DataRow registro)
        {
            SimulacaoViewModel Simulacao = new SimulacaoViewModel();

            Simulacao.Id = Convert.ToInt32(registro["id"]);
            Simulacao.Nome = registro["nome"].ToString();
            Simulacao.DataCriacaoAlteracao = Convert.ToDateTime(registro["dataCriacaoAlteracao"].ToString());

            if (registro["dataInicio"] != DBNull.Value)
                Simulacao.DataInicio = Convert.ToDateTime(registro["dataInicio"].ToString());
            if (registro["dataFim"] != DBNull.Value)
                Simulacao.DataFim = Convert.ToDateTime(registro["dataFim"].ToString());

            Simulacao.Motor = new MotorDAO().PesquisarPorId(Convert.ToInt32(registro["idMotor"]));
            Simulacao.Usuario = new UsuarioDAO().PesquisarPorId(Convert.ToInt32(registro["idUsuario"]));

            Simulacao.IdUsuario = Convert.ToInt32(registro["idUsuario"]);
            Simulacao.IdMotor = Convert.ToInt32(registro["idMotor"]);

            if (registro["media"] != DBNull.Value)
                Simulacao.Media = Convert.ToDecimal(registro["media"]);
            if (registro["max"] != DBNull.Value)
                Simulacao.Max = Convert.ToDecimal(registro["max"]);
            if (registro["min"] != DBNull.Value)
                Simulacao.Min = Convert.ToDecimal(registro["min"]);

            return Simulacao;
        }

        #endregion Sql

        #region Gerar Id

        public int GerarId()
        {
            string sql = "select MAX(Id) + 1 from Simulacao";
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
