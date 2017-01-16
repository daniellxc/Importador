using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model.Emissores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Business
{
    public class EventosExternosComprasNaoProcessadosBO
    {
        EventosExternosComprasNaoProcessadosDAO _dao;

        public EventosExternosComprasNaoProcessadosBO(int idEmissor)
        {
            _dao = new EventosExternosComprasNaoProcessadosDAO(idEmissor);
        }

       public EventosExternosComprasNaoProcessados BuscarEventoExternoCompraNaoProcessado(int idAutorizacao)
        {
            return _dao.BuscarEventoExternoCompraNaoProcessado(idAutorizacao);
        }
    }
}
