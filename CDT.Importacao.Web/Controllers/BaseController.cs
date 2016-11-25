using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDT.Importacao.Web.Controllers
{
    public class BaseController:Controller
    {
        public void Alert(string mensagem)
        {
            ViewBag.Erro = mensagem;
        }
    }
}