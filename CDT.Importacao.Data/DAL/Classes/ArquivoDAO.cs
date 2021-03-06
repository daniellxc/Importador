﻿using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class ArquivoDAO
    {
        private class DAO : AbstractCrudDao<Arquivo>
        {
            public DAO(Contexto ctx) : base(ctx)
            {

            }
        }

        DAO _dao = new DAO(new Contexto());


        public void Salvar(Arquivo arquivo)
        {
            try
            {
                if(arquivo.IdArquivo == 0)
                {
                    _dao.Add(arquivo);
                    _dao.CommitChanges();
             

                }else
                {
                    _dao.Update(arquivo, arquivo.IdArquivo);
                }

            }catch(Exception ex)
            {
                throw ex;
            }
        }

       

        public List<Arquivo> ListarTodos()
        {
            return _dao.GetAll();
        }

        public Arquivo Buscar(string nome)
        {
            return _dao.Find(x => x.NomeArquivo.Equals(nome)).FirstOrDefault();
        }

        public Arquivo Buscar(int id)
        {
            return _dao.Get(id);
        }

        public List<Arquivo> BuscarPorLayout(int idLayout)
        {
            return _dao.Find(x => x.IdLayout == idLayout).ToList(); 
        }

        public Arquivo BuscarPorLayout(int idLayout, DateTime dataReferencia)
        {
            DateTime data = DateTime.Parse(dataReferencia.ToShortDateString());
            return _dao.Find(x => x.IdLayout == idLayout && x.DataRegistro == data).FirstOrDefault();
        }

        public Arquivo Buscar(DateTime dataRegistro)
        {
            DateTime data = DateTime.Parse(dataRegistro.ToShortDateString());
           return _dao.Find(x => x.DataRegistro.Equals(dataRegistro)).FirstOrDefault();
        }
    }
}
