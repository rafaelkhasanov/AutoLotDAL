using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using AutoLotDAL.EF;
using AutoLotDAL.Models.Base;

namespace AutoLotDAL.Repos
{
    public class BaseRepo<T> : IRepo<T>, IDisposable where T : EntityBase, new()
    {
        private readonly DbSet<T> _table;
        private readonly AutoLotEntities _db;
        public BaseRepo()
        {
            _db = new AutoLotEntities();
            _table = _db.Set<T>();
        }
        protected AutoLotEntities Context => _db;
        public void Dispose()
        {
            _db?.Dispose();
        }

        internal int SaveChanges()
        {
            try
            {
                return _db.SaveChanges();
            }

            catch (DbUpdateConcurrencyException ex)
            {
                //Генерируется, когда возникает ошибка, связанная с параллелизмом,
                //Пока что просто повторно сгенерировать исключение.
                throw;
            }

            catch (DbUpdateException ex)
            {
                //Генерируется, когда обновление базыв данных терпит неудачу.
                //Проверить внутреннее исключение (исключения), чтобы получить
                //дополнительные сведения и выяснить, на какие объекты это повлияло
                //Пока что просто сгенерировать исключение.
                throw;
            }

            catch (CommitFailedException ex)
            {
                //Обработать здесь отказы транзакции
                //Пока что просто повторно сгенерировать исключение
                throw;
            }

            catch (Exception ex)
            {
                //Произошло какое-то другое исключение, которое должно быть обработано
                throw;
            }
        }

        public T GetOne(int? id) => _table.Find(id);

        public int Add(T entity)
        {
            _table.Add(entity);
            return SaveChanges();
        }

        public int AddRange(IList<T> entities)
        {
            _table.AddRange(entities);
            return SaveChanges();
        }

        //Обновление записей
        public int Save(T entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
            return SaveChanges();
        }

        public int Delete(int id, byte[] timeStamp)
        {
            _db.Entry(new T() { Id = id, Timestamp = timeStamp }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public int Delete(T entity)
        {
            _db.Entry(entity).State = EntityState.Deleted;
            return SaveChanges();
        }

        public virtual List<T> GetAll() => _table.ToList();

        //Извлечение записей с помощью SQl 
        public List<T> ExecuteQuery(string sql) => _table.SqlQuery(sql).ToList();

        public List<T> ExecuteQuery(string sql, object[] sqlParametersObjects) => _table.SqlQuery(sql, sqlParametersObjects).ToList();
    }
}
