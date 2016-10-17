using CDT.Importacao.Data.Business.Import;
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Business
{
    public class ArquivoBO
    {
        Arquivo arquivo;
        ArquivoDAO _dao = new ArquivoDAO();

        public ArquivoBO(int IdArquivo)
        {
            this.arquivo = _dao.Buscar(IdArquivo);
        }

        public ArquivoBO(Arquivo arquivo)
        {
            this.arquivo = arquivo;
        }

        public IImportador ObjetoImportador()
        {
            if (arquivo != null)
            {
                Assembly asm = Assembly.Load("CDT.Importacao.Data");
                Type importadorElo = asm.GetType(arquivo.FK_Layout.ClasseImportadora);
                Object imp = Activator.CreateInstance(importadorElo);
                if (imp != null)
                    return (IImportador)imp;
                return null;
            }
            else
                return null;
            
        }

        public void Importar()
        {
            
            try
            {
                if (arquivo.IdStatusArquivo != 2)
                {
                    
                    ObjetoImportador().Importar(arquivo);
                    arquivo.IdStatusArquivo = 2;
                    arquivo.DataImportacao = DateTime.Now;
                    _dao.Salvar(arquivo);
                }
                else
                    throw new Exception("Este arquivo já foi processado");
               

            }catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
