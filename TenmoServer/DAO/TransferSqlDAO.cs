using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;


namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;
        private readonly IAccountDAO accountSqlDAO;
        public TransferSqlDAO(string dbConnectionString, IAccountDAO _accountSqlDAO)
        {
            connectionString = dbConnectionString;
            accountSqlDAO = _accountSqlDAO;
        }

        public List<Transfer> GetTranfersForCurrentUser(int userId)
        {
            List<Transfer> returnTransfers = new List<Transfer>();

            try
            {
                int accountId = 0;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand getAcctId = new SqlCommand("SELECT account_id FROM accounts WHERE user_id = @userid;", conn))
                    {
                        getAcctId.Parameters.AddWithValue("@userId", userId);
                        accountId = Convert.ToInt32(getAcctId.ExecuteScalar());
                    }

                    using (SqlCommand getTransfers = new SqlCommand("SELECT transfer_id, transfers.transfer_type_id, transfer_type_desc, transfers.transfer_status_id, transfer_status_desc, account_from, u1.username AS from_user, account_to, u2.username AS to_user, amount " +
                        "FROM transfers " +
                        "JOIN accounts ac1 ON transfers.account_from = ac1.account_id " +
                        "JOIN users u1 ON ac1.user_id = u1.user_id JOIN accounts ac2 ON transfers.account_to = ac2.account_id " +
                        "JOIN users u2 ON ac2.user_id = u2.user_id " +
                        "JOIN transfer_statuses ON transfers.transfer_status_id = transfer_statuses.transfer_status_id " +
                        "JOIN transfer_types ON transfers.transfer_type_id = transfer_types.transfer_type_id " +
                        "WHERE account_to = @accountId OR account_from = @accountId;", conn))
                    {
                        getTransfers.Parameters.AddWithValue("@accountId", accountId);
                        SqlDataReader reader = getTransfers.ExecuteReader();

                        while (reader.Read())
                        {
                            Transfer t = new Transfer()
                            {
                                TransferId = Convert.ToInt32(reader["transfer_id"]),
                                TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]),
                                TransferTypeDesc = Convert.ToString(reader["transfer_type_desc"]),
                                TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]),
                                TransferStatusDesc = Convert.ToString(reader["transfer_status_desc"]),
                                AccountFrom = Convert.ToInt32(reader["account_from"]),
                                UserFrom = Convert.ToString(reader["from_user"]),
                                AccountTo = Convert.ToInt32(reader["account_to"]),
                                UserTo = Convert.ToString(reader["to_user"]),
                                Amount = Convert.ToDecimal(reader["amount"])
                            };
                            returnTransfers.Add(t);
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }

            return returnTransfers;
        }

        public List<Transfer> GetPendingRequestsForCurrentUser(int userId)
        {
            List<Transfer> returnTransfers = new List<Transfer>();

            try
            {
                int accountId = 0;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand getAcctId = new SqlCommand("SELECT account_id FROM accounts WHERE user_id = @userid;", conn))
                    {
                        getAcctId.Parameters.AddWithValue("@userId", userId);
                        accountId = Convert.ToInt32(getAcctId.ExecuteScalar());
                    }

                    using (SqlCommand getTransfers = new SqlCommand("SELECT transfer_id, transfers.transfer_type_id, transfer_type_desc, transfers.transfer_status_id, transfer_status_desc, account_from, u1.username AS from_user, account_to, u2.username AS to_user, amount " +
                        "FROM transfers " +
                        "JOIN accounts ac1 ON transfers.account_from = ac1.account_id " +
                        "JOIN users u1 ON ac1.user_id = u1.user_id JOIN accounts ac2 ON transfers.account_to = ac2.account_id " +
                        "JOIN users u2 ON ac2.user_id = u2.user_id " +
                        "JOIN transfer_statuses ON transfers.transfer_status_id = transfer_statuses.transfer_status_id " +
                        "JOIN transfer_types ON transfers.transfer_type_id = transfer_types.transfer_type_id " +
                        "WHERE account_from = @accountId AND transfers.transfer_status_id = 1;", conn))
                    {
                        getTransfers.Parameters.AddWithValue("@accountId", accountId);
                        SqlDataReader reader = getTransfers.ExecuteReader();

                        while (reader.Read())
                        {
                            Transfer t = new Transfer()
                            {
                                TransferId = Convert.ToInt32(reader["transfer_id"]),
                                TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]),
                                TransferTypeDesc = Convert.ToString(reader["transfer_type_desc"]),
                                TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]),
                                TransferStatusDesc = Convert.ToString(reader["transfer_status_desc"]),
                                AccountFrom = Convert.ToInt32(reader["account_from"]),
                                UserFrom = Convert.ToString(reader["from_user"]),
                                AccountTo = Convert.ToInt32(reader["account_to"]),
                                UserTo = Convert.ToString(reader["to_user"]),
                                Amount = Convert.ToDecimal(reader["amount"])
                            };
                            returnTransfers.Add(t);
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }

            return returnTransfers;
        }

        public Transfer MakeTransfer(int acctToTransferTo, int acctToTransferFrom, decimal amtToTransfer)
        {
            int transferId = 0;
            Transfer newTransfer = new Transfer();

            if (CheckBalance(acctToTransferFrom, amtToTransfer))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES (2, 2, @accountfrom, @accountto, @amount); SELECT SCOPE_IDENTITY();", conn);

                        cmd.Parameters.AddWithValue("@accountfrom", acctToTransferFrom);
                        cmd.Parameters.AddWithValue("@accountto", acctToTransferTo);
                        cmd.Parameters.AddWithValue("@amount", amtToTransfer);
                        transferId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    UpdateBalance(acctToTransferTo, acctToTransferFrom, amtToTransfer);
                    newTransfer = GetTransferById(transferId);
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return newTransfer;
        }

        public bool UpdateBalance(int acctToTransferTo, int acctToTransferFrom, decimal amtToTransfer)
        {
            int rowsAffectedUpdateTo = 0;
            int rowsAffectedUpdateFrom = 0;
            bool updateSuccessful = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmdUpdateTo = new SqlCommand("UPDATE accounts SET balance = balance + @amount WHERE account_id = @accountto;", conn))
                    {
                        cmdUpdateTo.Parameters.AddWithValue("@accountto", acctToTransferTo);
                        cmdUpdateTo.Parameters.AddWithValue("@amount", amtToTransfer);
                        cmdUpdateTo.ExecuteNonQuery();
                    }
                    using (SqlCommand cmdUpdateFrom = new SqlCommand("UPDATE accounts SET balance = balance - @amount WHERE account_id = @accountfrom;", conn))
                    {
                        cmdUpdateFrom.Parameters.AddWithValue("@accountfrom", acctToTransferFrom);
                        cmdUpdateFrom.Parameters.AddWithValue("@amount", amtToTransfer);
                        cmdUpdateFrom.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            if (rowsAffectedUpdateTo > 0 && rowsAffectedUpdateFrom > 0)
            {
                updateSuccessful = true;
            }
            return updateSuccessful;
        }

        public bool CheckBalance(int acctId, decimal amtTransfer)
        {
            bool okToTransfer = false;

            decimal currentBalance = accountSqlDAO.GetBalance(acctId);

            if (currentBalance >= amtTransfer)
            {
                okToTransfer = true;
            }

            return okToTransfer;
        }

        public Transfer MakeTransferRequest(int acctToTransferTo, int acctToTransferFrom, decimal amtToTransfer)
        {
            int transferId = 0;
            Transfer newTransfer = new Transfer();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES (1, 1, @accountfrom, @accountto, @amount); SELECT SCOPE_IDENTITY();", conn);

                    cmd.Parameters.AddWithValue("@accountfrom", acctToTransferFrom);
                    cmd.Parameters.AddWithValue("@accountto", acctToTransferTo);
                    cmd.Parameters.AddWithValue("@amount", amtToTransfer);
                    transferId = Convert.ToInt32(cmd.ExecuteScalar());
                }
                newTransfer = GetTransferById(transferId);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return newTransfer;
        }

        public string RespondToTransferRequest(int transferId, int updatedStatusId, int acctToTransferTo, int acctToTransferFrom, decimal amtToTransfer)
        {
            string returnString = "";
            Transfer existingTransfer = GetTranfersForCurrentUser(accountSqlDAO.GetUserId(acctToTransferFrom)).Find(a => a.TransferId == transferId);

            if (existingTransfer != null)
            {
                if (updatedStatusId == 2)
                {
                    if (CheckBalance(acctToTransferFrom, amtToTransfer))
                    {
                        UpdateTransfer(transferId, 2);
                        UpdateBalance(acctToTransferTo, acctToTransferFrom, amtToTransfer);
                        returnString = $"Transfer Approved. Confirm Transfer ID: {transferId}";
                    }
                    else
                    {
                        UpdateTransfer(transferId, 3);
                        returnString = $"Transfer unsuccessful. Insufficient balance.";
                    }
                }
                else if (updatedStatusId == 3)
                {
                    UpdateTransfer(transferId, 3);
                    returnString = $"Transfer rejected.";
                }
            }
            else
            {
                returnString = "Transfer not found.";
            }
           
            return returnString;
        }

        public Transfer UpdateTransfer(int transferId, int updatedStatusId)
        {
            Transfer updatedTransfer = GetTransferById(transferId);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using SqlCommand updateTransfer = new SqlCommand("UPDATE transfers SET transfer_status_id = @updatedStatusId WHERE transfer_id = @transferId;", conn);
                    updateTransfer.Parameters.AddWithValue("@updatedStatusId", updatedStatusId);
                    updateTransfer.Parameters.AddWithValue("@transferId", transferId);
                    updateTransfer.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }

            return updatedTransfer;
        }

        public Transfer GetTransferById(int transferId)
        {
            Transfer returnTransfer = new Transfer();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using SqlCommand selectTransfer = new SqlCommand("SELECT transfer_id, transfer_type_id, transfers.transfer_status_id, account_from, u1.username AS from_user, account_to, u2.username AS to_user, amount FROM transfers " +
                        "JOIN accounts ac1 ON transfers.account_from = ac1.account_id " +
                        "JOIN users u1 ON ac1.user_id = u1.user_id " +
                        "JOIN accounts ac2 ON transfers.account_to = ac2.account_id " +
                        "JOIN users u2 ON ac2.user_id = u2.user_id " +
                        "JOIN transfer_statuses ON transfers.transfer_status_id = transfer_statuses.transfer_status_id " +
                        "WHERE transfer_id = @transfer_Id;", conn);
                    selectTransfer.Parameters.AddWithValue("@transfer_Id", transferId);
                    SqlDataReader reader = selectTransfer.ExecuteReader();

                    while (reader.Read())
                    {
                        returnTransfer.TransferId = Convert.ToInt32(reader["transfer_id"]);
                        returnTransfer.TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]);
                        returnTransfer.TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]);
                        returnTransfer.AccountFrom = Convert.ToInt32(reader["account_from"]);
                        returnTransfer.UserFrom = Convert.ToString(reader["from_user"]);
                        returnTransfer.AccountTo = Convert.ToInt32(reader["account_to"]);
                        returnTransfer.UserTo = Convert.ToString(reader["to_user"]);
                        returnTransfer.Amount = Convert.ToDecimal(reader["amount"]);
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }

            return returnTransfer;
        }
    }
}
