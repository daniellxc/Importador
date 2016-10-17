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
    public class ArquivoController : Controller
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
                _dao.Salvar(arquivo);
                return View("Index");
            }
            catch (Exception ex)
            {
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
                 new ArquivoBO(IdArquivo).Importar();

                return View("Index");

            }catch(Exception ex)
            {
                return View("Index");
            }
        }
    }
}