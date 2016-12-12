
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
    public class RegistroController : BaseController
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
            string acao = registro.IdRegistro == 0 ? "Salvar registro: " : "Editar registro: ";
            try
            {
                _dao.Salvar(registro);
                LogINFO(this.ToString(), acao + LAB5Utils.ReflectionUtils.GetObjectDescription(registro));
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                LogWARN(this.ToString(), acao + LAB5Utils.ReflectionUtils.GetObjectDescription(registro) + ex.Message);
                return View("Cadastro", registro);
                
            }
           
        }

        public ActionResult Editar(int IdRegistro)
        {
            return View("Cadastro", _dao.Buscar(IdRegistro));
        }

        public ActionResult Excluir(int IdRegistro)
        {
            Registro reg = _dao.Buscar(IdRegistro);
            try
            {
                _dao.Excluir(IdRegistro);
                LogINFO(this.ToString(), "Excluir registro: " + LAB5Utils.ReflectionUtils.GetObjectDescription(reg));
                return RedirectToAction("Index");

            }catch(Exception ex)
            {
                Alert(ex.Message);
                LogWARN(this.ToString(), "Excluir registro: " + LAB5Utils.ReflectionUtils.GetObjectDescription(reg) + ex.Message);
                ViewBag.Erro = ex.Message;
            }
            return View("Index");
        }
    }
}