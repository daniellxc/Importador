
using CDT.Importacao.Data.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PagedList;

namespace CDT.Importacao.Data.Model
{
    public abstract class AbstractCrudDao<T> where T :  class
    {
        private IContext _context;

        private DbContext contextoConcreto;

    

        public AbstractCrudDao(IContext context)
        {
            _context = context;
            contextoConcreto = _context.GetContext();
        }
      

        public void Add(T entity)
        {
            
            contextoConcreto.Set<T>().Add(entity);

        }

        public void ZZZBulkInsert(IEnumerable<T> entities)
        {

            try
            {
                contextoConcreto.BulkInsert<T>(entities);
               
            }
            catch (Exception)
            { throw; }
        }

        public void BulkInsert(IEnumerable<T> entities)
        {
           
            contextoConcreto.Set<T>().AddRange(entities);
            contextoConcreto.SaveChanges();
                
            
        }


     


        public void Delete(T entity)
        {
            contextoConcreto.Entry<T>(entity).State = System.Data.Entity.EntityState.Deleted;
            CommitChanges();
        }

        public List<T> Find(Expression<Func<T, bool>> where)
        {

            return contextoConcreto.Set<T>().Where<T>(where).ToList();
        }

        

        public IQueryable<T> ZZZFind(Expression<Func<T, bool>> where)
        {
           
            return contextoConcreto.Set<T>().Where<T>(where);
        }


        public T Get(params Object[] keys)
        {
            return contextoConcreto.Set<T>().Find(keys);
        }

        public List<T> GetAll()
        {
            
            return contextoConcreto.Set<T>().ToList<T>();
            
        }

        public void Update(T updateEntity, object key)
        {
            var original = this.Get(key);
            if (original != null)
            {
                contextoConcreto.Entry(original).CurrentValues.SetValues(updateEntity);
                contextoConcreto.SaveChanges();
            }

        }

        public DbContext GetContext()
        {
            return contextoConcreto;
        }

        public void CommitChanges()
        {
            contextoConcreto.SaveChanges();
        }
    }
}
