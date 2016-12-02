using CDT.Importacao.Data.Business.Import;
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Business
{
    public class ArquivoBO
    {
        Arquivo arquivo;
        ArquivoDAO _dao;

        public Arquivo Arquivo
        {
            get
            {
                return arquivo;
            }

            set
            {
                arquivo = value;
            }
        }

        public ArquivoBO(int IdArquivo)
        {
            this.Arquivo = _dao.Buscar(IdArquivo);
            if(_dao == null)
            {
                _dao = new ArquivoDAO();
            }
        }

        public ArquivoBO(Arquivo arquivo)
        {
            this.Arquivo = arquivo;
            if (_dao == null)
            {
                _dao = new ArquivoDAO();
            }
        }

        

        public Arquivo BuscarArquivoData(DateTime data)
        {
            return _dao.Buscar(data);
        }

        public IImportador ObjetoImportador()
        {
            if (Arquivo != null)
            {
                Assembly asm = Assembly.Load("CDT.Importacao.Data");
                Type importadorElo = asm.GetType(Arquivo.FK_Layout.ClasseImportadora);
                Object imp = Activator.CreateInstance(importadorElo);
                if (imp != null)
                    return (IImportador)imp;
                return null;
            }
            else
                return null;
            
        }

        public Arquivo GerarArquivo(int idLayout, int idEmissor)
        {
            
            try
            {
                if (new LayoutDAO().Buscar(idLayout) == null) throw new Exception("Layout não encontrado.");
                if (new EmissorDAO().Buscar(idEmissor) == null) throw new Exception("Emissor não encontrado.");

                this.Arquivo = new Arquivo();
                Arquivo.DataRegistro = DateTime.Now.Date;
                Arquivo.DataImportacao = DateTime.Parse("01/01/1900");
                Arquivo.IdEmissor = idEmissor;
                Arquivo.IdLayout = idLayout;
                Arquivo.IdStatusArquivo = 1;
                Arquivo.NomeArquivo = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + "_LQDNACELO.txt";
                _dao.Salvar(Arquivo);
                return this.Arquivo;
            }catch(Exception ex)
            {
                throw new Exception("Erro ao gerar arquivo." + ex.Message);
            }
            
        }

        private void AlterarStatus(int idStatus)
        {
            Arquivo.IdStatusArquivo = idStatus;
            _dao.Salvar(Arquivo);
        }

        public void Importar()
        {
         
            try
            {
                if (Arquivo.IdStatusArquivo != 2)
                {

                    ObjetoImportador().Importar(Arquivo);
                    ObjetoImportador().GerarTransacoesEmissor(Arquivo);
                    Arquivo.IdStatusArquivo = 2;
                    Arquivo.DataImportacao = DateTime.Now;
                    _dao.Salvar(Arquivo);
                }
                else
                    throw new Exception("Este arquivo já foi processado");
               

            }catch(Exception ex)
            {
                AlterarStatus(3);//erro no processamento
                throw new Exception("Erro ao processar o arquivo." + ex.Message);
            }
        }

   

    }
}
