using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Business
{
    public class EmissorBO
    {
        Emissor emissor;
        EmissorDAO dao = new EmissorDAO();

        public EmissorBO(Emissor emissor)
        {
            this.emissor = emissor;
        }

        public EmissorBO(int idEmissor)
        {
            this.emissor = dao.Buscar(idEmissor);
        }

        public string GetConnectionString()
        {
            return "Data Source =" + emissor.IpBaseEmissor +  "; Initial Catalog =" + emissor.NomeBaseEmissor + "; Integrated Security = true;MultipleActiveResultSets=True";
        }
    }
}
