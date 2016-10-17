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

namespace Testes
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var cronos = new Stopwatch();
            Arquivo l = new ArquivoDAO().Buscar(4);

            cronos.Start();
            new ImportadorElo().Importar(l);
            new ImportadorElo().Conciliar(l);
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
            StreamWriter sw = new StreamWriter("c:\\arquivos\\elo\\carga10000k.txt");

            sw.WriteLine(Header);
            for (int i = 0; i < 10000000; i++)
            {
                sw.WriteLine(Detail);
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
                    inf.Valor = StringUtil.Zip(Detail) + i.ToString().PadLeft(10, '0');

                    infos.Add(inf);
                    inf = null;
                }



                new InformacaoRegistroDAO().Salvar(infos);
            }

        }

        #region Arquivo
        private string Header    = "BO10201610070001    20161006150000000000000000001111                                                                                                                0071";
        private string Detail    = "0500###################L#1***********************0000614220161010###########000000000010986CONDUCTOR      TECNOLOGIAJOAO   PESSOABR 123400001  0071000000011X 0120161010";
        private string Trailer   = "BZ10000100000001000000000000001000000010000000000000010000000100000000000000100000001000000000000001                                                                   1";
        #endregion
    }

}
