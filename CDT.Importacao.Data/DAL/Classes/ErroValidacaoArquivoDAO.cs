using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class ErroValidacaoArquivoDAO
    {
        private class DAO : AbstractCrudDao<ErroValidacaoArquivo>
        {
            public DAO(Contexto ctx) : base(ctx)
            {

            }
        }

        DAO _dao = new DAO(new Contexto());

        public void Salvar(List<ErroValidacaoArquivo> erros)
        {
            try
            {
                _dao.InsertData(erros);
            }catch(Exception ex)
            {
                throw new Exception("Erro ao salvar erro de validação." + ex.Message);
            }
        }
    }
}
