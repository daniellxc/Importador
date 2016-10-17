
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CDT.Importacao.Data.Model
{
    public abstract class AbstractCrudDao<T> where T :  class
    {
        private Contexto _context = new Contexto();

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);

        }

        public void BulkInsert(IEnumerable<T> entities)
        {
            //  List<List<T>> partitions = entities.Select((x, i) => new { Index = i, Value = x })
            //.GroupBy(x => x.Index / 100000)
            //.Select(x => x.Select(v => v.Value).ToList())
            //.ToList();
            //    foreach(List<T> list in partitions)
            //    {
            //        _context.Set<T>().AddRange(list);
            //        _context.SaveChanges();

            //    }
            _context.Set<T>().AddRange(entities);
            _context.SaveChanges();
                
            
        }


        public Contexto GetContext()
        {
            if (_context != null)
                return _context;
            return new Contexto();
        }


        public void Delete(T entity)
        {
            _context.Entry<T>(entity).State = System.Data.Entity.EntityState.Deleted;
            CommitChanges();
        }

        public List<T> Find(Expression<Func<T, bool>> where)
        {
            return _context.Set<T>().Where<T>(where).ToList<T>();
        }


        public T Get(params Object[] keys)
        {
            return _context.Set<T>().Find(keys);
        }

        public List<T> GetAll()
        {
            return _context.Set<T>().ToList<T>();
        }

        public void Update(T updateEntity, object key)
        {
            var original = this.Get(key);
            if (original != null)
            {
                _context.Entry(original).CurrentValues.SetValues(updateEntity);
                _context.SaveChanges();
            }

        }


        public void CommitChanges()
        {
            _context.SaveChanges();
        }
    }
}
