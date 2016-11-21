using System.Collections.Generic;
using System;
using System.Linq;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using aspLoginReg.Models;
using CryptoHelper;

namespace LoginApp.Factory
{
    public class LoginRepository : IFactory<User>
    {
        private string connectionString;
        public LoginRepository()
        {
            connectionString = "server=localhost;userid=root;password=root;port=8889;database=loginDB;SslMode=None";
        }

        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(connectionString);
            }
        }
    

        public void Add(User item)
        {
            using (IDbConnection dbConnection = Connection) {
                string password_Hash = Crypto.HashPassword(item.password);
                string query = $"INSERT INTO users (first_name,last_name, email, password, created_at) VALUES ('{item.first_name}', '{item.last_name}', '{item.email}', '{password_Hash}', NOW())";
                dbConnection.Open();
                dbConnection.Execute(query);
            }
        }
        public User FindByID()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM users ORDER BY id DESC LIMIT 1").FirstOrDefault();
            }
        }
        public User FindEmail(string email)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM users WHERE email = @Email LIMIT 1", new { Email = email }).FirstOrDefault();
            }
        }
        public User CurrentUser(int num)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>($"SELECT * FROM users WHERE id = {num}").FirstOrDefault();
            }
        }
    }
}