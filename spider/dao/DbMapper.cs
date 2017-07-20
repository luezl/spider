using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using Dapper;


namespace spider{
    class DbMapper
    {
        private static readonly string _connStr = @"Data Source=.\spider.db;Pooling=true;FailIfMissing=false";

        /// <summary>
        /// 接続
        /// </summary>
        /// <returns></returns>
        public SQLiteConnection OpenConnection()
        {
            var connection = new SQLiteConnection(_connStr);
            connection.Open();
            return connection;
        }


        public int insert(edata d)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query2 = @"insert into edata values(null, @type, @name, @url, @value1, @value2, @value3, @value4, @value5, @create_at)";
                return conn.Execute(query2, d);
            }
        }
    }
}
