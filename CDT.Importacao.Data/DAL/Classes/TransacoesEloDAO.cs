﻿using CDT.Importacao.Data.Business;
using CDT.Importacao.Data.Model;
using CDT.Importacao.Data.Model.Emissores;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using EntityFramework.Extensions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using CDT.Importacao.Data.Utils.Log;

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
                    _dao.Update(transacao);
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

        public List<string> GerarReportFraude(string data)
        {
            List<SqlParameter> parms = new List<SqlParameter>();
            SqlParameter parm1 = new SqlParameter("@DATA", System.Data.SqlDbType.VarChar);
            parm1.Value = data;
            parms.Add(parm1);
           
            try
            {
                List<string> result = _dao.CallStoredProcedure(parm1, "REPORT_FRAUDE_ELO @DATA").ToList();
                if(result != null)
                {
                    StreamWriter sw = new StreamWriter(@"\\10.1.1.139\Arquivos_Clientes\Cielo\Entrada\Registrofraude_Elo\" + "MBRCV.OE.RX.BRPOERAD.CBSS.FRAUDES(+1)"); //MBRCV.OE.RX.BRPOERAD.CBSS.FRAUDES(+1) //MBRCV.OE.RX.BRPOER7D.CBSS.FRAUDES(+1)
                    foreach (string s in result)
                        sw.WriteLine(s);
                    sw.Flush();
                    sw.Close();

                    return result;
                }

                return null;

            }catch
            {
                throw;
            }
           

        }

        public void Salvar(List<TransacaoElo> transacoes)
        {

            try
            {
                
                _dao.InsertData(transacoes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void NegarTransacoes(List<int> ids)
        {
            _dao.GetContext().Set<TransacaoElo>().Where(u=> ids.Contains(u.Id_TransacaoElo)).Update(u => new TransacaoElo { FlagProblemaTratamento = true });
        }



        public List<TransacaoElo> ListarTodos()
        {
            return _dao.GetAll();
        }


        public TransacaoElo Buscar(int id)
        {
            return _dao.Get(id);
        }

        
        public List<int> TransacoesPorArquivo(int idArquivo)
        {
            var query = from trans in _dao.GetContext().Set<TransacaoElo>()
                        where trans.IdArquivo  == idArquivo
                        select trans.Id_TransacaoElo;
            return query.ToList();
        }

        /// <summary>
        /// Transacoes lidas do arquivo
        /// </summary>
        /// <param name="nomeArquivo"></param>
        /// <param name="codMoeda"></param>
        /// <returns></returns>
        public List<TransacaoElo> TransacoesProcessadasPorCodigoMoeda(string nomeArquivo, int codMoeda)
        {
            return _dao.Find(x => x.CodigoMoeda == codMoeda && x.NomeArquivo == nomeArquivo);
        }

        /// <summary>
        /// Transacoes lidas do arquivo e localizadas na base do emissor
        /// </summary>
        /// <param name="nomeArquivo"></param>
        /// <param name="codMoeda"></param>
        /// <returns></returns>
        public List<TransacaoElo> TransacoesConciliadasPorCodigoMoeda(string nomeArquivo, int codMoeda)
        {
            return _dao.Find(x => x.CodigoMoeda == codMoeda && x.NomeArquivo == nomeArquivo && x.FlagProblemaTratamento == false);
        }

        public List<TransacaoElo> TransacoesNaoConciliadasPorCodigoMoeda(string nomeArquivo, int codMoeda)
        {
            return _dao.Find(x => x.CodigoMoeda == codMoeda && x.NomeArquivo == nomeArquivo && x.FlagProblemaTratamento == true);
        }

        public List<TransacaoElo> TransacoesPorCodigoTransacao(string nomeArquivo, string codTransacao)
        {
            return _dao.Find(x => x.NomeArquivo.Equals(nomeArquivo) && x.TE.Equals(codTransacao));
        }

        public List<TransacaoElo> TransacoesCredito(string nomeArquivo)
        {
            return _dao.Find(x => x.NomeArquivo.Equals(nomeArquivo) && (x.TE.Equals("06") || x.TE.Equals("20")));
        }

        public List<TransacaoElo> TransacoesDebito(string nomeArquivo)
        {
            return _dao.Find(x => x.NomeArquivo.Equals(nomeArquivo) && (x.TE.Equals("05") || x.TE.Equals("10") || x.TE.Equals("15") ));
        }

        public List<TransacaoElo> TransacoesProcessadas(string nomeArquivo)
        {
            return _dao.Find(x => x.NomeArquivo.Equals(nomeArquivo));
        }

        public List<TransacaoElo> TransacoesConciliadas(string nomeArquivo)
        {
            return _dao.Find(x => x.NomeArquivo.Equals(nomeArquivo) && x.FlagProblemaTratamento == false);
        }

        public List<TransacaoElo> TransacoesNacionaisProcessadas(string nomeArquivo)
        {
            return _dao.Find(x => nomeArquivo.Equals(nomeArquivo)  && x.FlagTransacaoInternacional == false);
        }



    }
}
