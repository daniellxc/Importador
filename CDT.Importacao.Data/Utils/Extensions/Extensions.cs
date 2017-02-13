using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Utils.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Retorna as mensagens de toda hierarquia de Exception.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetAllMessages(this Exception exception)
        {
            string message = "";
            if (exception.InnerException != null)
                message += GetAllMessages(exception.InnerException);
            message += exception.Message + " ";

            return message;

        }
    }
}
