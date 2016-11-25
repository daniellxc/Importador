
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDT.Importacao.Web.Controllers
{
    public class CampoController : BaseController
    {
        CampoDAO _dao = new CampoDAO();
        // GET: Campo
        public ActionResult Index()
        {
            return View();
        }

       

        public ActionResult Cadastro(int? IdRegistro)
        {
            if (IdRegistro.HasValue)
            {
                Campo campo = new Campo();
                campo.IdRegistro = IdRegistro.Value;
                return View(campo);
            }
            return View();
            
           
        }

        public ActionResult Salvar(Campo campo)
        {
            if (!ModelState.IsValid) return View("Cadastro", campo);

            try
            {
                _dao.Salvar(campo);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                return View("Cadastro", campo);
            }
        }

        
        public ActionResult RegistrosLayout(int id)
        {
            return Json(new SelectList(new RegistroDAO().RegistrosLayout(id), "IdRegistro", "NomeRegistro"));
        }

        public ActionResult Editar(int IdCampo)
        {
            return View("Cadastro", _dao.Buscar(IdCampo));
        }

        public ActionResult Excluir(int IdCampo)
        {
            try
            {
                _dao.Excluir(IdCampo);
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                ViewBag.Erro = ex.Message;
            }
            return View("Index");
        }

        public ActionResult AddSubcampo(int IdCampo)
        {
            Session["IdCampo"]= IdCampo;
            return RedirectToAction("Cadastro", "Subcampo", _dao.Buscar(IdCampo));
        }

    }
}