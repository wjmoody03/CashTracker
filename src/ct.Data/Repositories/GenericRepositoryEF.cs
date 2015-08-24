using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using ct.Data.Contexts;


namespace ct.Data.Repositories
{ 

    public interface IGenericRepositoryEF<T> where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetAllEagerly(params string[] includes);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Delete(T entity);
        void Edit(T entity);
        void Save();
    }

    public abstract class GenericRepositoryEF<T> :
        //ICoreSaver,
        IGenericRepositoryEF<T>
        where T : class
    {
        private IDbContext _context;

        public GenericRepositoryEF(IDbContext Context){
            _context = Context;
        }

        public virtual IQueryable<T> GetAllEagerly(params string[] includes)
        {
            _context.Configuration.LazyLoadingEnabled = false;
            _context.Configuration.ProxyCreationEnabled = false;
            var s = GetAll();           
            foreach (string i in includes)
            {
                s = s.Include(i);
            }
            return s;
        }

        public virtual IQueryable<T> GetAll()
        {
            _context.Configuration.ProxyCreationEnabled = false;
            IQueryable<T> query = _context.Set<T>();
            return query;
        }

        public IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            return query;
        }

        public virtual void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public virtual void AddRange(IEnumerable<object> entities)
        {
            AddRange((IEnumerable<T>)entities);
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                _context.Set<T>().Add(entity);
            }
        }

        public virtual void Delete(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Set<T>().Remove(entity);
        }

        public virtual void Edit(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;            
        }

        public virtual void Save()
        {
            _context.SaveChanges();
        }
    }


}
