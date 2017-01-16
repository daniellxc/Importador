using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model.Emissores;
using LAB5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Business
{
    public class AutorizacoesBO
    {
        private AutorizacoesDAO _autDAO;


        public AutorizacoesBO(int idEmissor)
        {
            _autDAO = new AutorizacoesDAO(idEmissor);
        }

        public bool AutorizacaoExiste(string numeroCartao, string codigoAutorizacao)
        {
            long cartaoHash = BitConverter.ToInt64(LAB5Utils.CriptografiaUtils.GetMD5(numeroCartao), 0);
            return _autDAO.LocalizaAutorizacao(cartaoHash, codigoAutorizacao).Count == 1;
        }

        public Autorizacoes LocalizarAutorizacao(string numeroCartao, string codigoAutorizacao)
        {
            try
            {
                long cartaoHash = BitConverter.ToInt64(LAB5Utils.CriptografiaUtils.GetMD5(numeroCartao), 0);
                return _autDAO.LocalizaAutorizacao(cartaoHash, codigoAutorizacao).First();
            }
            catch
            {
                return null;
            }
           
        }

        public AutorizacaoEvtExternoCompraNaoProcessado LocalizarAutorizacaoEvtExternoCompraNaoProcessado(string numeroCartao, string codigoAutorizacao)
        {
            try
            {
                long cartaoHash = BitConverter.ToInt64(LAB5Utils.CriptografiaUtils.GetMD5(numeroCartao), 0);
                return _autDAO.LocalizaAutorizacaoEventoExternoCompraNaoProcessado(cartaoHash, codigoAutorizacao).First();
            }
            catch
            {
                return null;
            }

        }
    }
}
