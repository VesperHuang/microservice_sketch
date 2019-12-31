using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using Dapper;

using microservice_sketch.Models;

namespace microservice_sketch.Services
{
    public interface IDataBase {
        public void addUser(user _user);
    }

    public class DataBase: IDataBase, IApiService
    {
        MySqlConnection mySqlConnection = null;

        public DataBase() {
            string connectionString = "server=192.168.1.50;userid=root;pwd=abc12345;port=3306;database=testdb;SslMode=none;";
            mySqlConnection = new MySqlConnection(connectionString);
        }

        public void addUser(user _user) {

            string sql = "insert into users(name,account,password) values(?,?,?)";
         
            var nums = mySqlConnection.Execute(sql, new { _user.name, _user.account,_user.password });
        }

        public void info(string function_name)
        {
            Console.WriteLine("dataBase Service info " + function_name);
        }

    }
}
