using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDAO : IAccountDAO
    {
        private readonly string connectionString;

        public AccountSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        
        public int GetAccountId(int userId)
        {
            int acctId = 0;
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT account_id FROM accounts JOIN users ON accounts.user_id = users.user_id WHERE users.user_id = @userId;", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    acctId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }

            return acctId;
        }

        public int GetUserId(int acctId)
        {
            int userId = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT users.user_id FROM users JOIN accounts ON users.user_id = accounts.user_id WHERE accounts.account_id = @acctId;", conn);
                    cmd.Parameters.AddWithValue("@acctId", acctId);
                    userId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }

            return userId;
        }

        public decimal GetBalance(int accountId)
        {
            decimal balance = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT balance FROM accounts WHERE account_id = @accountId;", conn);
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    balance = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }

            return balance;
        }

        public List<User> ListUsers(int userId)
        {
            List<User> returnUsers = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, username FROM users WHERE user_id != @userId", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        User u = new User()
                        {
                            UserId = Convert.ToInt32(reader["user_id"]),
                            Username = Convert.ToString(reader["username"])
                        };
                        returnUsers.Add(u);
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }

            return returnUsers;
        }
    }
}
