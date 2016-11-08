using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class ArquivoDAO
    {
        private class DAO : AbstractCrudDao<Arquivo>
        {
            public DAO(Contexto ctx) : base(ctx)
            {

            }
        }

        DAO _dao = new DAO(new Contexto());


        public void Salvar(Arquivo arquivo)
        {
            try
            {
                if(arquivo.IdArquivo == 0)
                {
                    _dao.Add(arquivo);
                    _dao.CommitChanges();
                }else
                {
                    _dao.Update(arquivo, arquivo.IdArquivo);
                }

            }catch(Exception ex)
            {
                throw ex;
            }
        }


        public List<Arquivo> ListarTodos()
        {
            return _dao.GetAll();
        }

        public Arquivo Buscar(string nome)
        {
            return _dao.Find(x => x.NomeArquivo.Equals(nome)).FirstOrDefault();
        }

        public Arquivo Buscar(int id)
        {
            return _dao.Get(id);
        }
    }
}
