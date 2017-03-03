using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDT.Importacao.Web.Utils.Seguranca
{
    public class Permissions: AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult)
                filterContext.HttpContext.Response.Redirect("~/Login/AcessoNegado");
            else
            {
                
                    AcaoPermitida(filterContext, HttpContext.Current.User.Identity.Name );
                

            }
        }

        private void AcaoPermitida(AuthorizationContext filterContext, string idUsuario)
        {
            Usuario user = new UsuarioDAO().Buscar(int.Parse(idUsuario));
            if (!user.Admin)
            {
                
                filterContext.HttpContext.Response.Redirect("~/Admin/Home/AcessoNegado");
            }
                
        }
    }
}