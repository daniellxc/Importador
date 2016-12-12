using CDT.Importacao.Data.Utils.Log;
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

        public void LogWARN(string source, string message)
        {
            Logger.Warn(source, message, User.Identity.Name);
        }

        public void LogINFO(string source, string message)
        {
            Logger.Info(source, message, User.Identity.Name);
        }
    }
}