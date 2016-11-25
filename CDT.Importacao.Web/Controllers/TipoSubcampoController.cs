using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDT.Importacao.Web.Controllers
{
    public class TipoSubcampoController : BaseController
    {
        private TipoSubcampoDAO _dao = new TipoSubcampoDAO();
        // GET: TipoSubcampo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastro()
        {
            return View("Cadastro");
        }

        public ActionResult Salvar(TipoSubcampo tipoSubcampo)
        {
            if (!ModelState.IsValid) return View("Cadastro", tipoSubcampo);
            bool editando = tipoSubcampo.IdTipoSubcampo > 0;
            try
            {
                _dao.Salvar(tipoSubcampo);
                if(!editando)
                    return RedirectToAction("Index","Subcampo");
                ModelState.Clear();
                return View("Cadastro",null);
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                return View("Cadastro", tipoSubcampo);
            }
        }

        public ActionResult Editar(int IdTipoSubcampo)
        {
            return View("Cadastro", _dao.Buscar(IdTipoSubcampo));
        }

        public ActionResult Excluir(int IdTipoSubcampo)
        {
            try
            {
                _dao.Excluir(IdTipoSubcampo);
                return View("Cadastro");

            }catch(Exception ex)
            {
                Alert(ex.Message);
                return View("Cadastro");
            }
        }
    }
}