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
            string acao = editando ? "Editar arquivo: " : "Salvar arquivo";
            try
            {
                _dao.Salvar(tipoSubcampo);
                if(!editando)
                    return RedirectToAction("Index","Subcampo");
                ModelState.Clear();
                LogINFO(this.ToString(), acao + LAB5Utils.ReflectionUtils.GetObjectDescription(tipoSubcampo));
                return View("Cadastro",null);
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                LogWARN(this.ToString(), acao + LAB5Utils.ReflectionUtils.GetObjectDescription(tipoSubcampo) + ex.Message);
                return View("Cadastro", tipoSubcampo);
            }
        }

        public ActionResult Editar(int IdTipoSubcampo)
        {
            return View("Cadastro", _dao.Buscar(IdTipoSubcampo));
        }

        public ActionResult Excluir(int IdTipoSubcampo)
        {
            TipoSubcampo tipoSbcp = _dao.Buscar(IdTipoSubcampo);
            try
            {
                _dao.Excluir(IdTipoSubcampo);
                LogINFO(this.ToString(), "Excluir tipo de subcampo: " + LAB5Utils.ReflectionUtils.GetObjectDescription(tipoSbcp));
                return View("Cadastro");

            }catch(Exception ex)
            {
                Alert(ex.Message);
                LogWARN(this.ToString(), "Excluir tipo de subcampo: " + LAB5Utils.ReflectionUtils.GetObjectDescription(tipoSbcp) + ex.Message);
                return View("Cadastro");
            }
        }
    }
}