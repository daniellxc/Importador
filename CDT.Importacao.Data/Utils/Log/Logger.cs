using CDT.Importacao.Data.DAL;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CDT.Importacao.Data.Utils.Log
{
    public class Logger
    {
        static  LogDAO dao = new LogDAO();



        public static void Warn(string source, string message,string user)
        {
            CDT.Importacao.Data.Model.Log log = GenerateLog("WARN", source, message, user);
            log.Message = message;
            log.Source = source;
            dao.Salvar(log);

        }

        public static void Warn(List<CDT.Importacao.Data.Model.Log> logs)
        {
            dao.Salvar(logs);
        }

        public static void Info(string source, string message, string user)
        {
            CDT.Importacao.Data.Model.Log log = GenerateLog("INFO",source,message,user);
            log.Message = message;
            log.Source = source;
            dao.Salvar(log);

        }


        private static Model.Log GenerateLog(string level, string source, string message, string user)
        {
            Model.Log log = new Model.Log();
            log.Level = level;
            log.Source = source;
            log.Message = message;
            log.User = user;
            log.Date = DateTime.Now;

            return log;
        }
    }
}
