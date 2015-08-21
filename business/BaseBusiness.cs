using System;
using System.Collections.Generic;

namespace business
{
    public class BaseBusiness<T>
    {
        public static PetaPoco.Database db = new PetaPoco.Database("DefaultDB");

        private string TableName { get; set; }

        private string PrimaryKey { get; set; }

        public BaseBusiness(string tableName, string primaryKey)
        {
            this.TableName = tableName;
            this.PrimaryKey = primaryKey;
        }

        public void Insert(T t, bool autoIncrement=true)
        {
            db.Insert(TableName, PrimaryKey, autoIncrement, t);
        }

        public void Update(T t)
        {
            db.Update(TableName, PrimaryKey, t);
        }

        public void UpdateBySql(string sql)
        {
            db.Update<T>(sql,null);
        }

        public void Update(Object param, object id)
        {
            db.Update(TableName, PrimaryKey, param, id);
        }

        public void Delete(T t)
        {
            db.Delete(t);
        }

        public void DeleteById(int id)
        {
            db.Delete(TableName, PrimaryKey, id);
        }

        public void DeleteBySql(string sql)
        {
            db.Delete<T>(sql,null);
        }

        public T GetSingleOrDefault(string sql)
        {
            return db.SingleOrDefault<T>(sql);
        }

        public IEnumerable<T> GetList(string sql)
        {
            return db.Query<T>(sql);
        }
    }
}