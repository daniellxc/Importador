using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Business
{
    public class ExecucaoAgendamentoBO
    {
        private ExecucaoAgendamentoDAO _dao;

        public ExecucaoAgendamentoBO()
        {
            _dao = new ExecucaoAgendamentoDAO();
        }
        
        public void SalvarExecucaoAgendamento(int idAgendamento, DateTime dataExecucao, string message, bool sucesso)
        {
            try
            {
                ExecucaoAgendamento execAgd = new ExecucaoAgendamento();
                execAgd.IdAgendamento = idAgendamento;
                execAgd.DataExecucao = DateTime.Now;
                execAgd.Resultado = message;
                execAgd.Sucesso = sucesso;
                _dao.Salvar(execAgd);
            }
            catch(Exception ex) { throw ex; }
            

        }
    }
}
