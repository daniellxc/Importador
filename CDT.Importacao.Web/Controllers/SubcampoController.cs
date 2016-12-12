using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using LAB5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDT.Importacao.Web.Controllers
{
    [Authorize]
    public class SubcampoController : BaseController
    {
        SubcampoDAO _dao = new SubcampoDAO();
        // GET: Subcampo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastro(int? IdCampo)
        {
            if (IdCampo.HasValue)
            {
                Subcampo subcampo = new Subcampo();
                subcampo.IdCampo = IdCampo.Value;
                return View(subcampo);
            }
            return View();


        }

        public ActionResult Salvar(Subcampo subcampo)
        {
            if (!ModelState.IsValid) return View("Cadastro", subcampo);
            string acao = subcampo.IdSubcampo == 0 ? "Salvar subcampo: " : "Editar subcampo";
            try
            {
                _dao.Salvar(subcampo);
                LogINFO(this.ToString(), acao + LAB5Utils.ReflectionUtils.GetObjectDescription(subcampo));
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                LogWARN(this.ToString(), acao + LAB5Utils.ReflectionUtils.GetObjectDescription(subcampo) + ex.Message);
                return View("Cadastro", subcampo);
            }
        }


        public ActionResult Suspender()
        {
            Session["IdCampo"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Editar(int IdSubcampo)
        {
            return View("Cadastro", _dao.Buscar(IdSubcampo));
        }

        public ActionResult Excluir(int IdSubcampo)
        {
            Subcampo subc = _dao.Buscar(IdSubcampo);
            try
            {
                _dao.Excluir(IdSubcampo);
                LogINFO(this.ToString(), "Excluir campo: " + LAB5Utils.ReflectionUtils.GetObjectDescription(subc));
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                LogINFO(this.ToString(), "Excluir campo: " + LAB5Utils.ReflectionUtils.GetObjectDescription(subc) + ex.Message);
                ViewBag.Erro = ex.Message;
            }
            return View("Index");
        }
    }
}