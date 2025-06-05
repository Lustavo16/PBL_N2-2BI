using PBL_N2_1BI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PBL_N2_1BI.DAO
{
    public class PerfilDAO
    {
        #region SQL

        public static SqlParameter[] CriaParametros(PerfilViewModel perfil)
        {
            SqlParameter[] parametros = new SqlParameter[3];
            parametros[0] = new SqlParameter("Id", perfil.Id);
            parametros[1] = new SqlParameter("Nome", perfil.Nome);

            if (perfil.Permissoes != null)
                parametros[2] = new SqlParameter("Permissoes", perfil.Permissoes);
            else
                parametros[2] = new SqlParameter("Permissoes", DBNull.Value);

            return parametros;
        }

        #endregion

        #region CRUD

        public void Inserir(PerfilViewModel perfil)
        {
            perfil.Id = GerarId();

            string sql =
                "INSERT INTO Perfil (Id, Nome, Permissoes) " +
                $"VALUES (@Id, @Nome, @Permissoes)";

            HelperDAO.ExecutaSQL(sql, CriaParametros(perfil));
        }

        public void Alterar(PerfilViewModel perfil)
        {
            string sql = "UPDATE Perfil SET Nome=@Nome, Permissoes=@Permissoes where Id=@Id";

            HelperDAO.ExecutaSQL(sql, CriaParametros(perfil));
        }

        public void Excluir(int Id)
        {
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("Id", Id) };

            string sql = "delete from Perfil where Id= @Id";

            HelperDAO.ExecutaSQL(sql, parametros);
        }

        #endregion

        #region Consulta

        public List<PerfilViewModel> ListarPerfis(PerfilViewModel perfilConsulta)
        {
            List<PerfilViewModel> listaPerfis = new List<PerfilViewModel>();

            string sql = "select * from perfil";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, null);

            if (tabela.Rows.Count == 0)
                return new List<PerfilViewModel>();
            else
            {
                foreach (DataRow row in tabela.Rows)
                {
                    listaPerfis.Add(MontaModelConsulta(row));
                }
            }

            if (!string.IsNullOrEmpty(perfilConsulta.Nome))
                listaPerfis = listaPerfis.Where(xs => xs.Nome.ToLower().Trim().Contains(perfilConsulta.Nome.ToLower().Trim())).ToList();

            return listaPerfis;
        }

        public static PerfilViewModel MontaModelConsulta(DataRow registro)
        {
            PerfilViewModel perfil = new PerfilViewModel();

            perfil.Id = Convert.ToInt32(registro["id"]);
            perfil.Nome = registro["nome"].ToString();
            perfil.Permissoes = registro["permissoes"].ToString();

            return perfil;
        }

        public PerfilViewModel PesquisarPorId(int Id)
        {
            PerfilViewModel perfil = new PerfilViewModel();
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("Id", Id) };

            string sql = "select * from perfil where Id= @Id";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, parametros);

            if (tabela.Rows.Count == 0)
                return null;
            else
            {
                perfil = MontaModelConsulta(tabela.Rows[0]);
            }

            return perfil;
        }

        #endregion

        #region GerarID

        public int GerarId()
        {
            string sql = "select MAX(Id) + 1 from perfil";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, null);

            int idRetorno = 1;
            var retornoConsulta = tabela.Rows[0][0];

            if (retornoConsulta != DBNull.Value)
                idRetorno = Convert.ToInt32(retornoConsulta);

            return idRetorno;

        }

        #endregion

        #region Insert Inicial

        public int? InsertInicial()
        {
            PerfilViewModel perfil = new PerfilViewModel()
            {
                Id = GerarId(),
                Nome = "Perfil Admin",
                Permissoes =
                "[{\"NomeController\":\"Usuario\",\"NomeAction\":\"Consulta\"}," +
                "{\"NomeController\":\"Usuario\",\"NomeAction\":\"Adicionar\"}," +
                "{\"NomeController\":\"Usuario\",\"NomeAction\":\"Editar\"}," +
                "{\"NomeController\":\"Usuario\",\"NomeAction\":\"Excluir\"}," +
                "{\"NomeController\":\"Motor\",\"NomeAction\":\"Excluir\"}," +
                "{\"NomeController\":\"Motor\",\"NomeAction\":\"Editar\"}," +
                "{\"NomeController\":\"Motor\",\"NomeAction\":\"Adicionar\"}," +
                "{\"NomeController\":\"Motor\",\"NomeAction\":\"Consulta\"}," +
                "{\"NomeController\":\"Perfil\",\"NomeAction\":\"Consulta\"}," +
                "{\"NomeController\":\"Simulacao\",\"NomeAction\":\"Consulta\"}," +
                "{\"NomeController\":\"Simulacao\",\"NomeAction\":\"Adicionar\"}," +
                "{\"NomeController\":\"Perfil\",\"NomeAction\":\"Adicionar\"}," +
                "{\"NomeController\":\"Perfil\",\"NomeAction\":\"Editar\"}," +
                "{\"NomeController\":\"Simulacao\",\"NomeAction\":\"Editar\"}," +
                "{\"NomeController\":\"Simulacao\",\"NomeAction\":\"Excluir\"}," +
                "{\"NomeController\":\"Perfil\",\"NomeAction\":\"Excluir\"}]"
            };

            string sql =
                "INSERT INTO Perfil (Id, Nome, Permissoes) " +
                $"VALUES (@Id, @Nome, @Permissoes)";

            HelperDAO.ExecutaSQL(sql, CriaParametros(perfil));

            return perfil.Id;
        }

        #endregion
    }
}
