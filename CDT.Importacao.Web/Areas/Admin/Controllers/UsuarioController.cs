using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using CDT.Importacao.Web.Controllers;
using CDT.Importacao.Web.Utils.Seguranca;
using LAB5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDT.Importacao.Web.Areas.Admin.Controllers
{
    [Permissions()]
    public class UsuarioController : BaseController
    {
        private UsuarioDAO _dao = new UsuarioDAO();
        // GET: Admin/Usuario
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastro()
        {
            return View();
        }

        public ActionResult Salvar(Usuario usuario)
        {
            if (!ModelState.IsValid) return View("Cadastro", usuario);
            string acao = usuario.IdUsuario == 0 ? "Salvar usuario: " : "Editar usuario: ";
            try
            {
                _dao.Salvar(usuario);
                LogINFO(this.ToString(), acao + LAB5Utils.ReflectionUtils.GetObjectDescription(usuario));
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                LogWARN(this.ToString(), acao + LAB5Utils.ReflectionUtils.GetObjectDescription(usuario) + ex.Message);
                return View("Cadastro", usuario);
            }
        }

        public ActionResult Excluir(int id)
        {
            try
            {
                _dao.Excluir(id);
               

            }catch(Exception ex)
            {
                Alert(ex.Message);
                LogWARN(this.ToString(), "Erro ao excluir usuário. " + ex.Message);
            }

            return RedirectToAction("index");
        }

        public ActionResult Editar(int id)
        {
            return View("Cadastro", _dao.Buscar(id));
        }
    }

    
}