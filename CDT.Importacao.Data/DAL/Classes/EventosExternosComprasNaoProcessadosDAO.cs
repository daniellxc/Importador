using CDT.Importacao.Data.Business;
using CDT.Importacao.Data.Model;
using CDT.Importacao.Data.Model.Emissores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class EventosExternosComprasNaoProcessadosDAO
    {
        EmissorBO emissorBO;
        private class DAO : AbstractCrudDao<EventosExternosComprasNaoProcessados>
        {

            public DAO(ContextoEmissor ctx) : base(ctx)
            {

            }
        }

        DAO _dao;

        public EventosExternosComprasNaoProcessadosDAO(int idEmissor)
        {
            emissorBO = new EmissorBO(idEmissor);
            _dao = new DAO(new ContextoEmissor(emissorBO.GetConnectionString()));
        }


        public EventosExternosComprasNaoProcessados BuscarEventoExternoCompraNaoProcessado(int idAutorizao)
        {
            return _dao.Find(x => x.IdAutorizacao == idAutorizao).First();
        }
    }
}
