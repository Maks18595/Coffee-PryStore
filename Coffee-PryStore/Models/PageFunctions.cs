using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace Coffee_PryStore.Models
{
    public class PageFunctions
    {
        private SqlConnection connec;
        private SqlCommand cmd;
        private DataTable dt;
        private SqlDataAdapter adp;
        private string connecString;

        public PageFunctions()
        {
            connecString = @"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CoffeePryStore;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            connec = new SqlConnection(connecString);
            cmd = new SqlCommand();
            cmd.Connection = connec;
        }

        // Метод для отримання всіх користувачів
        public List<UsersData> GetUsers()
        {
            string query = "SELECT UserId, UserName, UserEmail, UserPhone FROM Users"; // Запит до вашої таблиці користувачів
            List<UsersData> users = new List<UsersData>();

            dt = GetData(query);
            foreach (DataRow row in dt.Rows)
            {
                users.Add(new UsersData
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    UserName = row["UserName"].ToString(),
                    UserEmail = row["UserEmail"].ToString(),
                    UserPhone = row["UserPhone"].ToString()
                });
            }

            return users;
        }

        // Метод для отримання одного користувача за ID
        public UsersData GetUserById(int id)
        {
            string query = $"SELECT UserId, UserName, UserEmail, UserPhone FROM Users WHERE UserId = {id}";
            dt = GetData(query);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new UsersData
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    UserName = row["UserName"].ToString(),
                    UserEmail = row["UserEmail"].ToString(),
                    UserPhone = row["UserPhone"].ToString()
                };
            }

            return null;
        }

        // Метод для додавання або оновлення користувача
        public bool SaveUser(UsersData user)
        {
            string query;
            if (user.UserId > 0)
            {
                // Оновлення існуючого користувача
                query = $"UPDATE Users SET UserName = '{user.UserName}', UserEmail = '{user.UserEmail}', UserPhone = '{user.UserPhone}' WHERE UserId = {user.UserId}";
            }
            else
            {
                // Додавання нового користувача
                query = $"INSERT INTO Users (UserName, UserEmail, UserPhone) VALUES ('{user.UserName}', '{user.UserEmail}', '{user.UserPhone}')";
            }

            return SetData(query) > 0;
        }

        // Метод для видалення користувача
        public bool DeleteUser(int id)
        {
            string query = $"DELETE FROM Users WHERE UserId = {id}";
            return SetData(query) > 0;
        }

        // Метод для отримання даних (вже є в класі)
        public DataTable GetData(string query)
        {
            dt = new DataTable();
            adp = new SqlDataAdapter(query, connecString);
            adp.Fill(dt);
            return dt;
        }

        // Метод для внесення змін у базу даних (вже є в класі)
        public int SetData(string query)
        {
            int counts = 0;
            if (connec.State == ConnectionState.Closed)
            {
                connec.Open();
            }
            cmd.CommandText = query;
            counts = cmd.ExecuteNonQuery();
            connec.Close();
            return counts;
        }
    }
}
