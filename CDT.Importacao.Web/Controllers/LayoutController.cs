
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
    public class LayoutController : BaseController
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
            string acao = layout.IdLayout == 0 ? "Salvar arquivo: " : "Editar arquivo: ";
            try
            {
                _dao.Salvar(layout);
                LogINFO(this.ToString(), acao + LAB5Utils.ReflectionUtils.GetObjectDescription(layout));
                return View("Index");
            }catch(Exception ex)
            {
                Alert(ex.Message);
                LogWARN(this.ToString(), acao + LAB5Utils.ReflectionUtils.GetObjectDescription(layout) + ex.Message);
                return View("Cadastro",layout);
            }
        }

        public ActionResult Editar(int IdLayout)
        {
            return View("Cadastro", _dao.Buscar(IdLayout));
        }

        public ActionResult Excluir(int IdLayout)
        {
            Layout lyt = _dao.Buscar(IdLayout);
            try
            {
                
                _dao.Excluir(IdLayout);
                LogINFO(this.ToString(), "Excluir arquivo: " + LAB5Utils.ReflectionUtils.GetObjectDescription(lyt));
                
            }
            catch (Exception ex)
            {
                LogWARN(this.ToString(), "Excluir arquivo: " + LAB5Utils.ReflectionUtils.GetObjectDescription(lyt) + ex.Message);
                Alert(ex.Message);
            }
            return View("Index");
        }

        
    }
}