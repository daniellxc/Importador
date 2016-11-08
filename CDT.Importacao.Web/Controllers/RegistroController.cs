
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDT.Importacao.Web.Controllers
{
    public class RegistroController : Controller
    {
        RegistroDAO _dao = new RegistroDAO();

        // GET: Registro
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastro()
        {
            return View();
        }

        public ActionResult Salvar(Registro registro)
        {
            if (!ModelState.IsValid) return View("Cadastro", registro);

            try
            {
                _dao.Salvar(registro);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Cadastro", registro);
                
            }
           
        }

        public ActionResult Editar(int IdRegistro)
        {
            return View("Cadastro", _dao.Buscar(IdRegistro));
        }

        public ActionResult Excluir(int IdRegistro)
        {
            try
            {
                _dao.Excluir(IdRegistro);
                return RedirectToAction("Index");

            }catch(Exception ex)
            {
                ViewBag.Erro = ex.Message;
            }
            return View("Index");
        }
    }
}