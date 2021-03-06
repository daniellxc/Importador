﻿using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class EmissorDAO
    {
        private class DAO : AbstractCrudDao<Emissor>
        {
            public DAO(Contexto ctx) : base(ctx)
            {

            }
        }

        DAO _dao = new DAO(new Contexto());


        public void Salvar(Emissor emissor)
        {
            try
            {
                if (emissor.IdEmissor == 0)
                {
                    _dao.Add(emissor);
                    _dao.CommitChanges();
                }
                else
                {
                    _dao.Update(emissor);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<Emissor> ListarTodos()
        {
            return _dao.GetAll();
        }

        public Emissor Buscar(string nome)
        {
            return _dao.Find(x => x.NomeEmissor.Equals(nome)).FirstOrDefault();
        }

        public Emissor Buscar(int id)
        {
            return _dao.Get(id);
        }

        public void Atualizar(Emissor emissor)
        {
            _dao.Update(emissor);
            
        }
    }
}
