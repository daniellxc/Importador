using CDT.Importacao.Data.Business;
using CDT.Importacao.Data.Model;
using CDT.Importacao.Data.Model.Emissores;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public  class TransacoesEloDAO
    {
        protected EmissorBO emissorBO;
        private class DAO : AbstractCrudDao<TransacaoElo>
        {
            public DAO(ContextoEmissor ctx):base(ctx)
            {

            }
        }

        DAO _dao;

        public TransacoesEloDAO(int idEmissor)
        {
            emissorBO = new EmissorBO(idEmissor);
            _dao = new DAO(new ContextoEmissor(emissorBO.GetConnectionString()));
        }

        public void Salvar(TransacaoElo transacao)
        {
            try
            {
                if (transacao.Id_TransacaoElo == 0)
                {
                    _dao.Add(transacao);
                    
                    _dao.CommitChanges();
                }
                else
                {
                    _dao.Update(transacao, transacao.Id_TransacaoElo);
                }

            }
            catch(DbUpdateException dbex)
            {
                throw dbex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Salvar(List<TransacaoElo> transacoes)
        {

            try
            {
               _dao.ZZZBulkInsert(transacoes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public List<TransacaoElo> ListarTodos()
        {
            return _dao.GetAll();
        }


        public TransacaoElo Buscar(int id)
        {
            return _dao.Get(id);
        }



    }
}
