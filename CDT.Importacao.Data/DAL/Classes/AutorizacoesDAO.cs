using CDT.Importacao.Data.Business;
using CDT.Importacao.Data.Model;
using CDT.Importacao.Data.Model.Emissores;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class AutorizacoesDAO
    {
        EmissorBO emissorBO;
        private class DAO : AbstractCrudDao<Autorizacoes>
        {
            
            public DAO(ContextoEmissor ctx) : base(ctx)
            {

            }
        }

        DAO _dao;

   

        public AutorizacoesDAO(int idEmissor)
        {
            emissorBO = new EmissorBO(idEmissor);
            _dao = new DAO(new ContextoEmissor(emissorBO.GetConnectionString()));
        }


        public List<Autorizacoes> LocalizaAutorizacao(long cartaoHash, string codigoAutorizacao)
        {


            var consulta = from cartoes in _dao.GetContext().Set<Cartoes>()
                           join
                           autorizacoes in _dao.GetContext().Set<Autorizacoes>()
                           on cartoes.Cartao equals autorizacoes.Cartao
                           join 
                           evt in _dao.GetContext().Set<EventosExternosComprasNaoProcessados>()
                           on autorizacoes.IdAutorizacao equals evt.IdAutorizacao
                           where cartoes.CartaoHash == cartaoHash && autorizacoes.CodigoAutorizacao == codigoAutorizacao
                          // &&  evt.DataCompra.Date.Equals(dataCompra)
                           select autorizacoes;

            return consulta.ToList();

        }


        public List<AutorizacaoEvtExternoCompraNaoProcessado> LocalizaAutorizacaoEventoExternoCompraNaoProcessado(long cartaoHash, string codigoAutorizacao)
        {


            var consulta = from cartoes in _dao.GetContext().Set<Cartoes>()
                           join
                           autorizacoes in _dao.GetContext().Set<Autorizacoes>()
                           on cartoes.Cartao equals autorizacoes.Cartao
                           join
                           evt in _dao.GetContext().Set<EventosExternosComprasNaoProcessados>()
                           on autorizacoes.IdAutorizacao equals evt.IdAutorizacao
                           where cartoes.CartaoHash == cartaoHash && autorizacoes.CodigoAutorizacao == codigoAutorizacao
                           select new AutorizacaoEvtExternoCompraNaoProcessado
                           {
                               Cartao = autorizacoes.Cartao,
                               DataAutorizacao = autorizacoes.DataAutorizacao,
                               IdAutorizacao = autorizacoes.IdAutorizacao,
                               CodigoAutorizacao = autorizacoes.CodigoAutorizacao,
                               MCC = evt.MCC,
                               NSUOrigem = autorizacoes.NSUOrigem,
                               NomeEstabelecimento = evt.NomeEstabelecimento,
                               NumeroEstabelecimento = autorizacoes.NumeroEstabelecimento,
                               ReferenceNumber = autorizacoes.ReferenceNumber,
                               Valor = autorizacoes.Valor
                           };

            return consulta.ToList();

        }


        public List<AutorizacaoEvtExternoCompraNaoProcessado> LocalizaAutorizacaoEventoExternoCompraNaoProcessado(string codigoAutorizacao)
        {


            var consulta = from cartoes in _dao.GetContext().Set<Cartoes>()
                           join
                           autorizacoes in _dao.GetContext().Set<Autorizacoes>()
                           on cartoes.Cartao equals autorizacoes.Cartao
                           join
                           evt in _dao.GetContext().Set<EventosExternosComprasNaoProcessados>()
                           on autorizacoes.IdAutorizacao equals evt.IdAutorizacao
                           where  autorizacoes.CodigoAutorizacao == codigoAutorizacao
                           select new AutorizacaoEvtExternoCompraNaoProcessado
                           {
                               Cartao = autorizacoes.Cartao,
                               DataAutorizacao = autorizacoes.DataAutorizacao,
                               IdAutorizacao = autorizacoes.IdAutorizacao,
                               CodigoAutorizacao = autorizacoes.CodigoAutorizacao,
                               MCC = evt.MCC,
                               NSUOrigem = autorizacoes.NSUOrigem,
                               NomeEstabelecimento = evt.NomeEstabelecimento,
                               NumeroEstabelecimento = autorizacoes.NumeroEstabelecimento,
                               ReferenceNumber = autorizacoes.ReferenceNumber,
                               Valor = autorizacoes.Valor
                           };

            return consulta.ToList();

        }


        public List<AutorizacaoEvtExternoCompraNaoProcessado> LocalizaAutorizacaoEventoExternoCompraNaoProcessado2(List<string> referenceNumbers)
        {


            var consulta = from cartoes in _dao.GetContext().Set<Cartoes>()
                           join
                           autorizacoes in _dao.GetContext().Set<Autorizacoes>()
                           on cartoes.Cartao equals autorizacoes.Cartao
                           join
                           evt in _dao.GetContext().Set<EventosExternosComprasNaoProcessados>()
                           on autorizacoes.IdAutorizacao equals evt.IdAutorizacao
                           where referenceNumbers.Contains(autorizacoes.ReferenceNumber) && SqlFunctions.DateDiff("DAY",evt.DataCompra, DateTime.Now) <= 30
                           select new AutorizacaoEvtExternoCompraNaoProcessado
                           {
                               Cartao = autorizacoes.Cartao,
                               DataAutorizacao = autorizacoes.DataAutorizacao,
                               IdAutorizacao = autorizacoes.IdAutorizacao,
                               MCC = evt.MCC,
                               NSUOrigem = autorizacoes.NSUOrigem,
                               NomeEstabelecimento = evt.NomeEstabelecimento,
                               NumeroEstabelecimento = autorizacoes.NumeroEstabelecimento,
                               ReferenceNumber = autorizacoes.ReferenceNumber,
                               Valor = autorizacoes.Valor
                           };

            return consulta.ToList();

        }



        


    }
}
