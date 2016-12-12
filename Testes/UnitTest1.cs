using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CDT.Importacao.Data.Model;
using CDT.Importacao.Data.Business.Import;
using System.IO;
using System.Reflection;
using System.Linq;
using CDT.Importacao.Data.Model;
using System.Collections.Generic;
using CDT.Importacao.Data.DAL.Classes;
using System.Diagnostics;
using CDT.Importacao.Data.Utils;
using CDT.Importacao.Data.Model.Emissores;
using PagedList;
using CDT.Importacao.Data.Business;
using CDT.Importacao.Data.Utils.Quartz.Schedulers;
using CDT.Importacao.Data.Utils.Quartz.Jobs;

namespace Testes
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TesteGeral()
        {
            var cronos = new Stopwatch();
            Arquivo l = new ArquivoDAO().Buscar(10);

            cronos.Start();
            new ImportadorElo().Importar(l);
            new ImportadorElo().GerarTransacoesEmissor(l);
            cronos.Stop();
             var tempoProcessamento = cronos.ElapsedMilliseconds; 
        }

        [TestMethod]
        public void TesteImportar()
        {
            var cronos = new Stopwatch();
            Arquivo l = new ArquivoDAO().Buscar(10);

            cronos.Start();
            new ImportadorElo().Importar(l);
            // new ImportadorElo().Conciliar(l);
            cronos.Stop();
            var tempoProcessamento = cronos.ElapsedMilliseconds;
        }

        [TestMethod]
        public void LeTransacoes()
        {
            string linha = "";
            StreamReader sr = new StreamReader("c:\\arquivos\\elo\\transacoes.txt");
            while((linha = sr.ReadLine()) != null)
            {

                Tratar(ref sr, linha);
            }
        }

        private void Tratar(ref StreamReader reader, string linha)
        {
            string aux = "";
            if (linha.Substring(0, 2).Equals("t1"))
            {
                for(int i = 1; i<= 4; i++)
                {
                    aux = reader.ReadLine();
                }
            }
            else if (linha.Substring(0, 2).Equals("t2"))
            {
                for (int i = 1; i <= 3; i++)
                {
                    aux = reader.ReadLine();
                }
            }
        }

        [TestMethod]
        public void Carga()
        {
            StreamWriter sw = new StreamWriter("c:\\arquivos\\elo\\cargaTransacoes1000k.txt");

            sw.WriteLine(Header);
            for (int i = 0; i < 1000000; i++)
            {

                sw.WriteLine(MontarRegistro0(i.ToString()));
                sw.WriteLine(Detail1);
                sw.WriteLine(Detail2);
                sw.WriteLine(Detail5);
                sw.WriteLine(Detail7);
            }
            sw.Write(Trailer);
            sw.Close();
        }


        [TestMethod]
        public void GenerateInstance()
        {
            
            Assembly asm = Assembly.Load("CDT.Importacao.Data");
            Type importadorElo = asm.GetType("CDT.Importacao.Data.Business.Import.ImportadorElo");
            Object imp = Activator.CreateInstance(importadorElo);
            ((ImportadorElo)imp).Importar(null);
        }

        [TestMethod]
        public  void InserirMil()
        {
            using (Contexto ctx = new Contexto())
            {
                List<InformacaoRegistro> infos = new List<InformacaoRegistro>();
                for (int i = 1; i <= 1000; i++)
                {
                    InformacaoRegistro inf = new InformacaoRegistro();
                    inf.IdArquivo = 0;
                    inf.IdRegistro = 0;
                    inf.Valor = StringUtil.Zip(Detail0 + i.ToString().PadLeft(10, '0')) ;

                    infos.Add(inf);
                    inf = null;
                }



                new InformacaoRegistroDAO().Salvar(infos);
            }

        }

        [TestMethod]
        public void TestarContextoEmissor()
        {
            TransacaoElo t = new TransacaoElo();
            t.TE = "10";
            t.FlagProblemaTratamento = false;
            t.DataProcessamento = DateTime.Now;
            t.DataTransacao = DateTime.Now;
            new TransacoesEloDAO(86).Salvar(t);

        }

        [TestMethod]
        public void TestarConciliar()
        {
            Arquivo arq = new Arquivo();
            arq.NomeArquivo = "Carga10.txt";
            arq.IdEmissor = 85;
            arq.IdArquivo = 10;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            new ImportadorElo().GerarTransacoesEmissor(arq);
            sw.Stop();
            var tempo = sw.ElapsedMilliseconds;
        }

        private string MontarRegistro0(string chave)
        {
            return "0500###################L#1"+ chave.PadLeft(23,'0') +"0000614220161010###########000000000010986CONDUCTOR      TECNOLOGIAJOAO   PESSOABR 123400001  0071000000011X 0120161010";
        }

       

        [TestMethod]
        public void TestarConversao()
        {
            Arquivo arq = new ArquivoDAO().Buscar(10);
            new ArquivoBO(arq).Importar();
        }

        [TestMethod]
        public void TestarBulkUpdate()
        {

            Agendamento a = new AgendamentoDAO().Buscar(2);

            new AgendamentoBO().IniciarAgendamento(a);

            CDTScheduler.NextExecutionTime("CDT.Importacao.Data.Utils.Quartz.Jobs.LiquidacaoNacionalEloJob", "grp_CDT.Importacao.Data.Utils.Quartz.Jobs.LiquidacaoNacionalEloJob");

            CDTScheduler.DeleteJob("CDT.Importacao.Data.Utils.Quartz.Jobs.LiquidacaoNacionalEloJob", "grp_CDT.Importacao.Data.Utils.Quartz.Jobs.LiquidacaoNacionalEloJob");

        }

        #region Arquivo
        private string Header    = "BO10201610070001    20161006150000000000000000001111                                                                                                                0071";
        private string Detail0   = "0500###################L#1***********************0000614220161010###########000000000010986CONDUCTOR      TECNOLOGIAJOAO   PESSOABR 123400001  0071000000011X 0120161010";
        private string Detail1   = "0501            000000 TEXTO_LIVRE_PARA_JUSTIFICATIVA_DE_CHARGEBACK      071    00000000000001500000008000000000010 000000000010 00000000000               00000000000  ";
        private string Detail2   = "0502            BR    030000000001019000101 990                                                                                                                         ";
        private string Detail5   = "0505000000000000000000000000010986Y10000    0000 000000000000                                                                                                           ";
        private string Detail7   = "050700123161010******BR 123456781234567800040000****************0202000000001000000008000000000010                                                            ##########";
        private string Trailer   = "BZ10000100000001000000000000001000000010000000000000010000000100000000000000100000001000000000000001                                                                   1";
        #endregion
    }

}
