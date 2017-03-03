using CDT.Importacao.Data.Business;
using CDT.Importacao.Data.Business.Import;
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using LAB5;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDT.Importacao.Web.Controllers
{
    [Authorize]
    public class ArquivoController : BaseController
    {
        ArquivoDAO _dao = new ArquivoDAO();
        // GET: Arquivo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastro()
        {
            return View();
        }

        public ActionResult Salvar(Arquivo arquivo)
        {
            if (!ModelState.IsValid) return View("Cadastro", arquivo);

            string acao = arquivo.IdArquivo == 0 ? "Salvar arquivo" : "Editar arquivo";
            try
            {
                arquivo.DataImportacao = DateTime.Parse("01/01/1900");
                arquivo.IdStatusArquivo = 1;
                LogINFO(this.ToString(), acao + LAB5Utils.ReflectionUtils.GetObjectDescription(arquivo));
                _dao.Salvar(arquivo);
                return View("Index");
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                LogWARN(this.ToString(), acao + LAB5Utils.ReflectionUtils.GetObjectDescription(arquivo) + ex.Message);
                return View("Cadastro", arquivo);
            }
        }

        public ActionResult Editar(int IdArquivo)
        {
            return View("Cadastro", _dao.Buscar(IdArquivo));
        }

        public ActionResult Importar(int IdArquivo)
        {
            Arquivo arq = _dao.Buscar(IdArquivo);
            try
            {
                
                new ArquivoBO(arq).Importar();
                Alert("Arquivo importado com sucesso.");
                LogINFO(this.ToString(), "Importar arquivo:" + LAB5Utils.ReflectionUtils.GetObjectDescription(arq));
                return View("Index");

            }catch(Exception ex)
            {
                Alert(ex.Message);
                LogWARN(this.ToString(), "Erro ao importar arquivo: " + LAB5Utils.ReflectionUtils.GetObjectDescription(arq) + ex.Message);
                return View("Index");
            }
        }

        /// <summary>
        /// UTILIZAR APENAS PARA A ELO. OS ARQUIVOS DE RETORNO SERÃO GERADOS PELOS JOBS QUARTZ
        /// </summary>
        /// <param name="idArquivo"></param>
        /// <returns></returns>
        public ActionResult MontarArquivoRetorno(int idArquivo)
        {
            Arquivo arquivoBase = _dao.Buscar(idArquivo);
            try
            {
                
                DirectoryInfo di = LAB5Utils.DirectoryUtils.CreateDirectory(@"\\10.1.1.139\Arquivos_Clientes\Cielo\Entrada\Liquidacao_Elo");
                
                new ArquivoRetornoElo(arquivoBase).MontarArquivoRetorno("C:\\app", "MBRCV.IO.RX.IO36D.M07063CI.RET(+1)");
                Alert("Arquivo gerado com sucesso.");
                LogINFO(this.ToString(), "Gerar arquivo retorno Elo:" + LAB5Utils.ReflectionUtils.GetObjectDescription(arquivoBase));
                return View("Index");

            }
            catch(Exception ex)
            {
                Alert(ex.Message);
                LogWARN(this.ToString(), "Erro ao gerar aqruivo retorno Elo: " + LAB5Utils.ReflectionUtils.GetObjectDescription(arquivoBase) + ex.Message);
                return View("Index");
            }
        }
    }
}