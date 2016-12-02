using CDT.Importacao.Data.Business;
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
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

            try
            {
                arquivo.DataImportacao = DateTime.Parse("01/01/1900");
                arquivo.IdStatusArquivo = 1;
                _dao.Salvar(arquivo);
                return View("Index");
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                return View("Cadastro", arquivo);
            }
        }

        public ActionResult Editar(int IdArquivo)
        {
            return View("Cadastro", _dao.Buscar(IdArquivo));
        }

        public ActionResult Importar(int IdArquivo)
        {
            try
            {
                 new ArquivoBO(_dao.Buscar(IdArquivo)).Importar();
                Alert("Arquivo importado com sucesso.");
                return View("Index");

            }catch(Exception ex)
            {
                Alert(ex.Message);
                return View("Index");
            }
        }
    }
}