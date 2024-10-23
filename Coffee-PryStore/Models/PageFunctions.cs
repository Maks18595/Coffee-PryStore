using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace Coffee_PryStore.Models
{
    public class PageFunctions
    {
        private readonly SqlConnection connec;
        private readonly SqlCommand cmd;
        private readonly string connecString;

        public PageFunctions()
        {
            connecString = @"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CoffeePryStore;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            connec = new SqlConnection(connecString);
            cmd = new SqlCommand { Connection = connec };
        }

        public List<UsersData> GetUsers()
        {
            string query = "SELECT UserId, UserName, UserEmail, UserPhone FROM Users";
            var users = new List<UsersData>();

            var dt = GetData(query);
            foreach (DataRow row in dt.Rows)
            {
                users.Add(new UsersData
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    UserName = row["UserName"]?.ToString() ?? string.Empty,
                    UserEmail = row["UserEmail"]?.ToString() ?? string.Empty,
                    UserPhone = row["UserPhone"]?.ToString() ?? string.Empty

                });
            }

            return users;
        }

        public UsersData GetUserById(int id)
        {
            string query = $"SELECT UserId, UserName, UserEmail, UserPhone FROM Users WHERE UserId = {id}";
            var dt = GetData(query);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new UsersData
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    UserName = row["UserName"]?.ToString() ?? string.Empty,
                    UserEmail = row["UserEmail"]?.ToString() ?? string.Empty,
                    UserPhone = row["UserPhone"]?.ToString() ?? string.Empty
                };
            }

         
            return new UsersData
            {
                UserId = 0,
                UserName = string.Empty,
                UserEmail = string.Empty,
                UserPhone = string.Empty
            };
        }


        public bool SaveUser(UsersData user)
        {
            string query;
            if (user.UserId > 0)
            {
                query = $"UPDATE Users SET UserName = '{user.UserName}', UserEmail = '{user.UserEmail}', UserPhone = '{user.UserPhone}' WHERE UserId = {user.UserId}";
            }
            else
            {
                query = $"INSERT INTO Users (UserName, UserEmail, UserPhone) VALUES ('{user.UserName}', '{user.UserEmail}', '{user.UserPhone}')";
            }

            return SetData(query) > 0;
        }

        public bool DeleteUser(int id)
        {
            string query = $"DELETE FROM Users WHERE UserId = {id}";
            return SetData(query) > 0;
        }

        public DataTable GetData(string query)
        {
            using var dt = new DataTable();
            using (var adp = new SqlDataAdapter(query, connecString))
            {
                adp.Fill(dt);
            }
            return dt;
        }

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
