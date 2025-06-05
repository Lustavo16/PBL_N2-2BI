using PBL_N2_1BI.Crypto;
using PBL_N2_1BI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PBL_N2_1BI.DAO
{
    public class UsuarioDAO : Hash
    {
        #region Sql

        public static SqlParameter[] CriaParametros(UsuarioViewModel usuario)
        {
            SqlParameter[] parametros = new SqlParameter[8];
            parametros[0] = new SqlParameter("Id", usuario.Id);
            parametros[1] = new SqlParameter("Login", usuario.Login);

            if (string.IsNullOrEmpty(usuario.Senha))
                parametros[2] = new SqlParameter("Senha", DBNull.Value);
            else
                parametros[2] = new SqlParameter("Senha", HashPassword(usuario.Senha));

            parametros[3] = new SqlParameter("Nome", usuario.Nome);
            parametros[4] = new SqlParameter("Email", usuario.Email);
            parametros[5] = new SqlParameter("PrimeiroAcesso", usuario.IsPrimeiroAcesso);

            if (usuario.Foto != null)
            {
                parametros[6] = new SqlParameter("Foto", SqlDbType.Image);
                parametros[6].Value = usuario.Foto;
            }
            else
            {
                parametros[6] = new SqlParameter("Foto", SqlDbType.Image);
                parametros[6].Value = DBNull.Value;
            }

            if (!usuario.IdPerfil.HasValue)
                parametros[7] = new SqlParameter("IdPerfil", DBNull.Value);
            else
                parametros[7] = new SqlParameter("IdPerfil", usuario.IdPerfil);

            return parametros;
        }

        public static SqlParameter[] CriaParametrosAlteracao(UsuarioViewModel usuario)
        {
            SqlParameter[] parametros = new SqlParameter[6];
            parametros[0] = new SqlParameter("Id", usuario.Id);
            parametros[1] = new SqlParameter("Login", usuario.Login);

            parametros[2] = new SqlParameter("Nome", usuario.Nome);
            parametros[3] = new SqlParameter("Email", usuario.Email);

            if (usuario.Foto != null)
            {
                parametros[4] = new SqlParameter("Foto", SqlDbType.Image);
                parametros[4].Value = usuario.Foto;
            }
            else
            {
                parametros[4] = new SqlParameter("Foto", SqlDbType.Image);
                parametros[4].Value = DBNull.Value;
            }

            if (!usuario.IdPerfil.HasValue)
                parametros[5] = new SqlParameter("IdPerfil", DBNull.Value);
            else
                parametros[5] = new SqlParameter("IdPerfil", usuario.IdPerfil);

            return parametros;
        }

        public static SqlParameter[] CriaParametrosUsuarioNovo(UsuarioViewModel usuario)
        {

            SqlParameter[] parametros = new SqlParameter[6];
            parametros[0] = new SqlParameter("Id", usuario.Id);
            parametros[1] = new SqlParameter("Login", usuario.Login);

            if (string.IsNullOrEmpty(usuario.Senha))
                parametros[2] = new SqlParameter("Senha", DBNull.Value);
            else
                parametros[2] = new SqlParameter("Senha", HashPassword(usuario.Senha));

            parametros[3] = new SqlParameter("Nome", usuario.Nome);
            parametros[4] = new SqlParameter("Email", usuario.Email);
            parametros[5] = new SqlParameter("PrimeiroAcesso", usuario.IsPrimeiroAcesso);

            return parametros;
        }

        #endregion

        #region CRUD

        public void Inserir(UsuarioViewModel usuario)
        {
            usuario.Id = GerarId();

            string sql =
                "INSERT INTO dbo.Usuarios (Id, Login, Senha, Nome, Email, PrimeiroAcesso, Foto, IdPerfil) " +
                $"VALUES (@Id, @Login, @Senha, @Nome, @Email, @PrimeiroAcesso, @Foto, @IdPerfil)";

            HelperDAO.ExecutaSQL(sql, CriaParametros(usuario));
        }

        public void Alterar(UsuarioViewModel usuario)
        {
            string sql = "UPDATE dbo.Usuarios SET Login=@Login, Senha=@Senha, Nome=@Nome, Email=@Email, PrimeiroAcesso=@PrimeiroAcesso where Id=@Id";

            HelperDAO.ExecutaSQL(sql, CriaParametrosUsuarioNovo(usuario));
        }

        public void AlterarCadastro(UsuarioViewModel usuario)
        {
            string sql = "UPDATE dbo.Usuarios SET Login=@Login, Nome=@Nome, Email=@Email, Foto=@Foto, IdPerfil=@IdPerfil where Id=@Id";

            HelperDAO.ExecutaSQL(sql, CriaParametrosAlteracao(usuario));
        }

        public void Excluir(int Id)
        {
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("Id", Id) };

            new SimulacaoDAO().ExcluirPorIdUsuario(Id);

            string sql = "delete from dbo.Usuarios where Id= @Id";

            HelperDAO.ExecutaSQL(sql, parametros);
        }

        #endregion

        #region Consulta

        public List<UsuarioViewModel> ListarUsuarios(UsuarioViewModel usuarioConsulta)
        {
            List<UsuarioViewModel> listaUsuarios = new List<UsuarioViewModel>();

            string sql = "select * from dbo.Usuarios";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, null);

            if (tabela.Rows.Count == 0)
                return new List<UsuarioViewModel>();
            else
            {
                foreach (DataRow row in tabela.Rows)
                {
                    listaUsuarios.Add(MontaModelConsulta(row));
                }
            }

            if (!string.IsNullOrEmpty(usuarioConsulta.Nome))
                listaUsuarios = listaUsuarios.Where(xs => xs.Nome.ToLower().Trim().Contains(usuarioConsulta.Nome.ToLower().Trim())).ToList();
            if (!string.IsNullOrEmpty(usuarioConsulta.Login))
                listaUsuarios = listaUsuarios.Where(xs => xs.Login.ToLower().Trim().Contains(usuarioConsulta.Login.ToLower().Trim())).ToList();

            return listaUsuarios;
        }

        public UsuarioViewModel PesquisarPorId(int Id)
        {
            UsuarioViewModel usuario = new UsuarioViewModel();
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("Id", Id) };

            string sql = "select * from dbo.Usuarios where Id= @Id";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, parametros);

            if (tabela.Rows.Count == 0)
                return null;
            else
            {
                usuario = MontaModelConsulta(tabela.Rows[0]);
            }

            return usuario;
        }

        public static UsuarioViewModel MontaModelConsulta(DataRow registro)
        {
            UsuarioViewModel usuario = new UsuarioViewModel();

            usuario.Id = Convert.ToInt32(registro["id"]);
            usuario.Login = registro["login"].ToString();
            usuario.Nome = registro["nome"].ToString();
            usuario.Email = registro["email"].ToString();
            usuario.IsPrimeiroAcesso = Convert.ToBoolean(registro["PrimeiroAcesso"]);
            usuario.Foto = registro["Foto"] as byte[];

            if (registro["IdPerfil"] != DBNull.Value)
                usuario.IdPerfil = Convert.ToInt32(registro["IdPerfil"]);

            return usuario;
        }

        public static UsuarioViewModel MontaModelLogin(DataRow registro)
        {
            UsuarioViewModel usuario = new UsuarioViewModel();

            usuario.Login = registro["login"].ToString();
            usuario.Senha = registro["senha"].ToString();
            usuario.IsPrimeiroAcesso = Convert.ToBoolean(registro["PrimeiroAcesso"]);

            return usuario;
        }

        public UsuarioViewModel PesquisarPorLogin(string login)
        {
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("Login", login) };
            string sql = "select * from Usuarios where Login=@Login";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, parametros);

            if (tabela.Rows != null && tabela.Rows.Count > 0)
            {
                UsuarioViewModel usuario = MontaModelConsulta(tabela.Rows[0]);
                usuario.Email = null;

                return usuario;
            }
            else
                return new UsuarioViewModel();
        }

        #endregion

        #region Gerar Id

        public int GerarId()
        {
            string sql = "select MAX(Id) + 1 from dbo.Usuarios";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, null);

            int idRetorno = 1;
            var retornoConsulta = tabela.Rows[0][0];

            if (retornoConsulta != DBNull.Value)
                idRetorno = Convert.ToInt32(retornoConsulta);

            return idRetorno;
        }

        #endregion

        #region Validações

        public bool ValidarLogin(LoginViewModel login)
        {
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("Login", login.Login) };
            string sql = "select * from dbo.Usuarios where Login=@Login";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, parametros);

            if (tabela.Rows != null && tabela.Rows.Count > 0)
            {
                UsuarioViewModel usuario = MontaModelLogin(tabela.Rows[0]);

                if (usuario.Login == login.Login && usuario.Senha != null && VerificarSenha(login.Senha, usuario.Senha))
                {
                    return true;
                }
            }
            else
                return false;

            return false;

        }

        public bool ValidarEmail(UsuarioViewModel usuario)
        {
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("Login", usuario.Login), new SqlParameter("Email", usuario.Email) };

            string sql = "select * from dbo.Usuarios where Login=@Login AND Email=@Email";

            DataTable tabela = HelperDAO.ExecutaSelect(sql, parametros);

            if (tabela.Rows != null && tabela.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public bool VerificarPrimeiroAcesso(string login)
        {
            if (string.IsNullOrEmpty(login))
                return false;

            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("Login", login) };
            string sql = "select * from Usuarios where Login=@Login";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, parametros);

            if (tabela.Rows != null && tabela.Rows.Count > 0)
            {
                UsuarioViewModel usuario = MontaModelLogin(tabela.Rows[0]);

                if (usuario.IsPrimeiroAcesso)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public bool VerificarLoginCadastro(string login)
        {
            if (string.IsNullOrEmpty(login))
                return false;

            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("Login", login) };
            string sql = "select * from Usuarios where Login=@Login";
            DataTable tabela = HelperDAO.ExecutaSelect(sql, parametros);

            if (tabela.Rows != null && tabela.Rows.Count > 0)
            {
                return true;
            }
            else
                return false;
        }

        #endregion

        #region Insert Inicial

        public void InsertInicial()
        {
            UsuarioViewModel usuario = new UsuarioViewModel()
            {
                Id = GerarId(),
                Nome = "Admin",
                Login = "Admin",
                Email = "Admin",
                IsPrimeiroAcesso = false,
                Senha = "senha",
                IdPerfil = new PerfilDAO().InsertInicial()
            };
            string sql =
                "INSERT INTO dbo.Usuarios (Id, Login, Senha, Nome, Email, PrimeiroAcesso, Foto, IdPerfil) " +
                $"VALUES (@Id, @Login, @Senha, @Nome, @Email, @PrimeiroAcesso, @Foto, @IdPerfil)";

            HelperDAO.ExecutaSQL(sql, CriaParametros(usuario));
        }

        #endregion
    }
}
