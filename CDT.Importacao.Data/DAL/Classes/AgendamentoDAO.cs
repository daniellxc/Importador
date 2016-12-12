using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class AgendamentoDAO
    {
        private class DAO : AbstractCrudDao<Agendamento>
        {
            public DAO(Contexto ctx) : base(ctx)
            {

            }
        }

        DAO _dao = new DAO(new Contexto());

        public void Salvar(Agendamento agendamento)
        {
            try
            {
                if (agendamento.IdAgendamento == 0)
                {
                    _dao.Add(agendamento);
                    _dao.CommitChanges();
                }
                else
                {
                    _dao.Update(agendamento, agendamento.IdAgendamento);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Agendamento Buscar(int id)
        {
            return _dao.Get(id);
        }


        public List<Agendamento> ListarTodos()
        {
            return _dao.GetAll() ;
        }

        public List<Agendamento> ListarAtivos()
        {
            return _dao.Find(a => a.Ativo == true);
        }

        public void Excluir(int IdAgendamento)
        {
            try
            {
                _dao.Delete(_dao.Get(IdAgendamento));
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


    }
}
