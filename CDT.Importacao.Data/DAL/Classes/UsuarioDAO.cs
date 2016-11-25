using CDT.Importacao.Data.Model;
using LAB5;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class UsuarioDAO
    {
        private class DAO : AbstractCrudDao<Usuario>
        {
            public DAO(Contexto ctx) : base(ctx)
            {

            }
        }

        DAO _dao = new DAO(new Contexto());

        public void Salvar(Usuario usuario)
        {
            try
            {
                usuario.Senha = LAB5Utils.CriptografiaUtils.TripleDESEncrypt(usuario.Senha, true);
                if (usuario.IdUsuario == 0)
                {
                
                    _dao.Add(usuario);
                    _dao.CommitChanges();
                }
                else
                {
                    _dao.Update(usuario);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void Excluir(int idUsuario)
        {
            try
            {
                _dao.Delete(_dao.Get(idUsuario));
            }
            catch (DbUpdateException dbex)
            {
                throw new Exception("Erro ao excluir." + dbex.Message);
            }
            catch
            {
                throw;
            }
        }

        public List<Usuario> ListarTodos()
        {
            return _dao.GetAll();
        }

        public Usuario Buscar(string login, string senha)
        {
            senha = LAB5Utils.CriptografiaUtils.TripleDESEncrypt(senha, true);
            return _dao.Find(x => x.Login.Equals(login) && x.Senha.Equals(senha)).FirstOrDefault();
        }

        public Usuario Buscar(int id)
        {
            return _dao.Get(id);
        }
    }
}
