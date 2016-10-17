
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDT.Importacao.Web.Controllers
{
    public class CampoController : Controller
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
                return View("Index");
            }
            catch (Exception ex)
            {
                return View("Cadastro", campo);
            }
        }

        public ActionResult Editar(int IdCampo)
        {
            return View("Cadastro", _dao.Buscar(IdCampo));
        }
    }
}