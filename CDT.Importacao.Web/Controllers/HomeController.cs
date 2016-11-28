using CDT.Importacao.Data.Utils.Quartz.Schedulers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDT.Importacao.Web.Controllers
{
   [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            /*INICIANDO SCHEDULERS*/
            // ConciliacaoJobScheduler.Start();
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

       
    }
}