using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;

namespace CDT.Importacao.Data.DAL.Classes
{
    public  class InformacaoRegistroDAO
    {
        private class DAO : AbstractCrudDao<InformacaoRegistro>
        {
            public DAO(Contexto ctx) : base(ctx)
            {

            }
        }

        DAO _dao = new DAO(new Contexto());



        public void Salvar(InformacaoRegistro informacaoRegistro)
        {
            try
            {
                if (informacaoRegistro.IdInformacaoRegistro == 0)
                {
                    _dao.Add(informacaoRegistro);
                    _dao.CommitChanges();
                }
                else
                {
                    _dao.Update(informacaoRegistro, informacaoRegistro.IdInformacaoRegistro);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Salvar(List<InformacaoRegistro> InformacaoRegistro)
        {
            
            try
            {
                _dao.BulkInsert(InformacaoRegistro);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public List<InformacaoRegistro> ListarTodos()
        {
            return _dao.GetAll();
        }

        public long[] GetIds(int idArquivo)
        {
            return _dao.ZZZFind(x => x.IdArquivo == idArquivo).Select(x => x.IdInformacaoRegistro).ToArray();

        }

        public IQueryable<InformacaoRegistro> ListarInformacoesDoArquivo(int idArquivo)
        {
            
            return _dao.ZZZFind(x => x.IdArquivo == idArquivo);
        }

        public IQueryable<InformacaoRegistro> ListarPorChave(string idChave)
        {
            return _dao.ZZZFind(x => x.Chave.Equals(idChave));
        }

        public string[] BUscarDetlhesArquivo(int idArquivo)
        {
            var chaves = from transacoes in _dao.GetContext().Set<InformacaoRegistro>()
                         join registros in _dao.GetContext().Set<Registro>()
                         on transacoes.IdRegistro equals registros.IdRegistro
                         join tipoRegistro in _dao.GetContext().Set<TipoRegistro>()
                         on registros.IdTipoRegistro equals tipoRegistro.IdTipoRegistro
                         where tipoRegistro.NomeTipoRegistro.ToUpper().Equals("DETAIL")
                         group transacoes by transacoes.Chave
                         into grp
                         select grp.Key;
            return chaves.ToArray();
        }


        public List<InformacaoRegistro> BuscarDetalhesComprimidosArquivo(int idArquivo)
        {
            
            var linha = (from transacoes in _dao.GetContext().Set<InformacaoRegistro>()
                        where transacoes.IdArquivo.Equals(idArquivo)
                        select transacoes);

         
            return linha.ToList();
            
        }

        

        public InformacaoRegistro Buscar(int id)
        {
            return _dao.Get(id);
        }
    }
}
