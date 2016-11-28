using CDT.Importacao.Data.DAL.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CDT.Importacao.Web.Controllers
{
    public class LoginController : BaseController
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Logon(string Login, string Senha)
        {
            if (!ModelState.IsValid)
            {
                return View("Login");
            }

            try
            {
                var user = new UsuarioDAO().Buscar(Login, Senha);
                if (user != null)
                {
                    if (user.Ativo)
                    {
                        FormsAuthentication.SetAuthCookie(user.IdUsuario.ToString(), false);
                       
                        
                        return RedirectToAction("Index", "Home");
                    }
                    else throw new Exception("Usuário inativo.");
                }
                else
                    throw new Exception("Login ou senha inválida.");
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                return View("Login");
            }
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
           
            return RedirectToAction("Index","Home");

        }
    }
}