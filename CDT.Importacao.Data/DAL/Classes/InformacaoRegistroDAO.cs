using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using System.Linq.Expressions;
using EntityFramework.Extensions;

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
                    _dao.Update(informacaoRegistro);
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
                _dao.InsertData(InformacaoRegistro);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void Update(List<InformacaoRegistro> inforacoesRegistro)
        {
            _dao.Update(inforacoesRegistro);
        }

        public void Update(InformacaoRegistro inforacaoRegistro)
        {
            _dao.Update(inforacaoRegistro);
        }


        public InformacaoRegistro BuscarHeaderArquivo(int idArquivo)
        {
            var query = from inf in _dao.GetContext().Set<InformacaoRegistro>()
                        join reg in _dao.GetContext().Set<Registro>()
                        on inf.IdRegistro equals reg.IdRegistro
                        join lay in _dao.GetContext().Set<Layout>()
                        on reg.IdLayout equals lay.IdLayout
                        join tre in _dao.GetContext().Set<TipoRegistro>()
                        on reg.IdTipoRegistro equals tre.IdTipoRegistro
                        where inf.IdArquivo == idArquivo && tre.NomeTipoRegistro.ToLower().Equals("header")
                        select inf;
            return query.FirstOrDefault();
        }

        public InformacaoRegistro BuscarTrailerArquivo(int idArquivo)
        {
            var query = from inf in _dao.GetContext().Set<InformacaoRegistro>()
                        join reg in _dao.GetContext().Set<Registro>()
                        on inf.IdRegistro equals reg.IdRegistro
                        join lay in _dao.GetContext().Set<Layout>()
                        on reg.IdLayout equals lay.IdLayout
                        join tre in _dao.GetContext().Set<TipoRegistro>()
                        on reg.IdTipoRegistro equals tre.IdTipoRegistro
                        where inf.IdArquivo == idArquivo && tre.NomeTipoRegistro.ToLower().Equals("trailer")
                        select inf;
            return query.FirstOrDefault();
        }

        public InformacaoRegistro BuscarUltimoHeaderEnviado(int idArquivo)
        {
            var query = from inf in _dao.GetContext().Set<InformacaoRegistro>()
                        join reg in _dao.GetContext().Set<Registro>()
                        on inf.IdRegistro equals reg.IdRegistro
                        join lay in _dao.GetContext().Set<Layout>()
                        on reg.IdLayout equals lay.IdLayout
                        join tre in _dao.GetContext().Set<TipoRegistro>()
                        on reg.IdTipoRegistro equals tre.IdTipoRegistro
                        where reg.ChaveRegistro.Equals("B0-OUTGOING") && tre.NomeTipoRegistro.ToLower().Equals("header") && inf.IdArquivo == idArquivo
                        select inf;
            return query.FirstOrDefault();
        }

        public List<InformacaoRegistro> ListarTodos()
        {
            return _dao.GetAll();
        }

        public long[] GetIds(int idArquivo)
        {
            return _dao.ZZZFind(x => x.IdArquivo == idArquivo).Select(x => x.IdInformacaoRegistro).ToArray();

        }



        public List<InformacaoRegistro> ListarRegistrosRejeitados(int idArquivo)
        {
            var query = from info in _dao.GetContext().Set<InformacaoRegistro>().ToList()
                        from negadas in _dao.GetContext().Set<ErroValidacaoArquivo>().ToList()
                        .Where(x=>x.IdInformacaoRegistro == info.IdInformacaoRegistro)
                        .OrderBy(x=>x.IdErroValidacaoArquivo)
                        .Take(1)
                        where info.IdArquivo == idArquivo  
                        select new InformacaoRegistro
                        {
                            Erro = negadas.erro.Substring(0, 3),
                            FlagErro = info.FlagErro,
                            IdArquivo = info.IdArquivo,
                            Chave = info.Chave,
                            IdInformacaoRegistro = info.IdInformacaoRegistro,
                            Valor = info.Valor,
                            IdRegistro = info.IdRegistro

                        };

            return query.ToList();
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

        public List<InformacaoRegistro> TransacoesAceitas(int idArquivo)
        {
            return _dao.Find(x => x.IdArquivo == idArquivo && x.FlagErro == false).ToList();
        }
        
        public void Delete(List<Registro> registros)
        {
            int[] ids = registros.Select(x => x.IdRegistro).ToArray();
            _dao.GetContext().Set<InformacaoRegistro>().Where(x=>ids.Contains(x.IdRegistro)).Delete();
        }

        public InformacaoRegistro Buscar(int id)
        {
            return _dao.Get(id);
        }

       
    }
}
