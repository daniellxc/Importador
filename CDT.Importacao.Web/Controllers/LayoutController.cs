
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDT.Importacao.Web.Controllers
{
    public class LayoutController : Controller
    {
        private LayoutDAO _dao = new LayoutDAO();

        // GET: Layout
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastro()
        {
            return View();
        }

        public ActionResult Salvar(Layout layout)
        {
            if (!ModelState.IsValid) return View("Cadastro",layout);

            try
            {
                _dao.Salvar(layout);
                return View("Index");
            }catch(Exception ex)
            {
                return View("Cadastro",layout);
            }
        }

        public ActionResult Editar(int IdLayout)
        {
            return View("Cadastro", _dao.Buscar(IdLayout));
        }

        public ActionResult Excluir(int IdLayout)
        {
            try
            {
                _dao.Excluir(IdLayout);
                
            }
            catch (Exception)
            {
               
            }
            return View("Index");
        }
    }
}