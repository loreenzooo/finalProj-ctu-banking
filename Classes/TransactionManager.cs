using System;
using System.Data;
using System.Data.SqlClient;

namespace Main.Classes
{
    public class TransactionManager
    {

        // DEPOSIT
        public string ProcessDeposit(int accountNumber, decimal amount)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string updateQuery = "UPDATE Users SET balance = balance + @Amount WHERE account_number = @AccountNo";
                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Amount", amount);
                            cmd.Parameters.AddWithValue("@AccountNo", accountNumber);
                            cmd.ExecuteNonQuery();
                        }

                        string logQuery = "INSERT INTO Transactions (receiver_account, transaction_type, amount, transaction_time) VALUES (@AccountNo, 'Deposit', @Amount, GETDATE())";
                        using (SqlCommand cmd = new SqlCommand(logQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AccountNo", accountNumber);
                            cmd.Parameters.AddWithValue("@Amount", amount);
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return "Error: " + ex.Message;
                    }
                }
            }
        }

        // WITHDRAW
        public string ProcessWithdraw(int accountNumber, decimal amount)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string updateQuery = "UPDATE Users SET balance = balance - @Amount WHERE account_number = @AccountNo";
                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Amount", amount);
                            cmd.Parameters.AddWithValue("@AccountNo", accountNumber);
                            cmd.ExecuteNonQuery();
                        }

                        string logQuery = "INSERT INTO Transactions (sender_account, transaction_type, amount, transaction_time) VALUES (@AccountNo, 'Withdraw', @Amount, GETDATE())";
                        using (SqlCommand cmd = new SqlCommand(logQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AccountNo", accountNumber);
                            cmd.Parameters.AddWithValue("@Amount", amount);
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return "Error: " + ex.Message;
                    }
                }
            }
        }

        // CLOUD MONEY
        public string SendCloudMoney(int senderAccountNo, int receiverAccountNo, decimal amount)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string deductQuery = "UPDATE Users SET balance = balance - @Amount WHERE account_number = @SenderNo";
                        using (SqlCommand cmd = new SqlCommand(deductQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Amount", amount);
                            cmd.Parameters.AddWithValue("@SenderNo", senderAccountNo);
                            cmd.ExecuteNonQuery();
                        }

                        string addQuery = "UPDATE Users SET balance = balance + @Amount WHERE account_number = @ReceiverNo";
                        using (SqlCommand cmd = new SqlCommand(addQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Amount", amount);
                            cmd.Parameters.AddWithValue("@ReceiverNo", receiverAccountNo);
                            cmd.ExecuteNonQuery();
                        }

                        string transferQuery = "INSERT INTO Transactions (sender_account, receiver_account, transaction_type, amount, transaction_time) VALUES (@SenderNo, @ReceiverNo, 'Transfer', @Amount, GETDATE())";
                        using (SqlCommand cmd = new SqlCommand(transferQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@SenderNo", senderAccountNo);
                            cmd.Parameters.AddWithValue("@ReceiverNo", receiverAccountNo);
                            cmd.Parameters.AddWithValue("@Amount", amount);
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return "Error: " + ex.Message;
                    }
                }
            }
        }

        // DASHBOARD GETBALANCE
        public decimal GetBalance(int accountNumber)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT balance FROM Users WHERE account_number = @AccountNo";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountNo", accountNumber);
                    var result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
        }

        // DASHBOARD GETTOTALSENT
        public decimal GetTotalSent(int accountNumber)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT SUM(amount) FROM Transactions WHERE sender_account = @AccountNo AND transaction_type = 'Transfer'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountNo", accountNumber);
                    var result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
        }

        // DASHBOARD NOTIFICATIONS
        public DataTable GetRecentReceivedTransfers(int accountNumber)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT TOP 5
                        sender_account   AS SenderAccount,
                        amount           AS Amount,
                        transaction_time AS Date
                    FROM Transactions
                    WHERE receiver_account = @AccountNo
                    AND transaction_type = 'Transfer'
                    ORDER BY transaction_time DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountNo", accountNumber);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd)) sda.Fill(dt);
                }
            }
            return dt;
        }

        // TABLE FETCHING
        public DataTable GetStatement(int accountNumber, string fromDate, string toDate)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string query = @"
                    SELECT
                        SeqNum,
                        Date,
                        Description,
                        Debit,
                        Credit,
                        Balance
                    FROM (
                        SELECT
                            ROW_NUMBER() OVER (ORDER BY transaction_time DESC, transaction_id DESC) AS SeqNum,
                            transaction_time AS Date,

                            
                            CASE
                                WHEN transaction_type = 'Transfer' THEN
                                    CASE WHEN sender_account = @AccountNo THEN 'Sent' ELSE 'Received' END
                                ELSE transaction_type
                            END AS Description,

                            CASE WHEN sender_account   = @AccountNo THEN amount ELSE NULL END AS Debit,
                            CASE WHEN receiver_account = @AccountNo THEN amount ELSE NULL END AS Credit,
                            SUM(
                                CASE
                                    WHEN receiver_account = @AccountNo THEN  amount
                                    WHEN sender_account   = @AccountNo THEN -amount
                                    ELSE 0
                                END
                            ) OVER (
                                ORDER BY transaction_time ASC, transaction_id ASC
                                ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                            ) AS Balance,
                            transaction_time AS tx_time
                        FROM Transactions
                        WHERE (sender_account = @AccountNo OR receiver_account = @AccountNo)
                    ) AS FullHistory
                    WHERE 1=1";

                if (!string.IsNullOrEmpty(fromDate)) query += " AND tx_time >= @FromDate";
                if (!string.IsNullOrEmpty(toDate)) query += " AND tx_time <= @ToDate";

                query += " ORDER BY Date DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountNo", accountNumber);
                    if (!string.IsNullOrEmpty(fromDate)) cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    if (!string.IsNullOrEmpty(toDate)) cmd.Parameters.AddWithValue("@ToDate", toDate + " 23:59:59");
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd)) sda.Fill(dt);
                }
            }
            return dt;
        }


        public DataTable GetDepositsWithdrawals(int accountNumber, string fromDate, string toDate, string type)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                // Build a single flat WHERE clause with all conditions.
                string whereClause = @"
                    WHERE (sender_account = @AccountNo OR receiver_account = @AccountNo)
                    AND transaction_type IN ('Deposit', 'Withdraw')";

                // Inline type filter using literal SQL values
                if (type == "D") whereClause += " AND transaction_type = 'Deposit'";
                else if (type == "W") whereClause += " AND transaction_type = 'Withdraw'";
                if (!string.IsNullOrEmpty(fromDate)) whereClause += " AND transaction_time >= @FromDate";
                if (!string.IsNullOrEmpty(toDate)) whereClause += " AND transaction_time <= @ToDate";

                string query = @"
                    SELECT
                        ROW_NUMBER() OVER (ORDER BY transaction_time DESC) AS SeqNum,
                        CASE WHEN transaction_type = 'Deposit' THEN 'D' ELSE 'W' END AS Type,
                        transaction_time AS Date,
                        amount AS Amount
                    FROM Transactions"
                    + whereClause +
                    " ORDER BY transaction_time DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountNo", accountNumber);
                    if (!string.IsNullOrEmpty(fromDate)) cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    if (!string.IsNullOrEmpty(toDate)) cmd.Parameters.AddWithValue("@ToDate", toDate + " 23:59:59");
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd)) sda.Fill(dt);
                }
            }
            return dt;
        }

        // REPORT: SENT OR RECEIVED TRANSACTIONS
        public DataTable GetTransfers(int accountNumber, string fromDate, string toDate, string type)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string whereClause = @"
                    WHERE transaction_type = 'Transfer'
                    AND (sender_account = @AccountNo OR receiver_account = @AccountNo)";

                if (type == "Sent") whereClause += " AND sender_account = @AccountNo";
                else if (type == "Received") whereClause += " AND receiver_account = @AccountNo";
                if (!string.IsNullOrEmpty(fromDate)) whereClause += " AND transaction_time >= @FromDate";
                if (!string.IsNullOrEmpty(toDate)) whereClause += " AND transaction_time <= @ToDate";

                string query = @"
                    SELECT
                        ROW_NUMBER() OVER (ORDER BY transaction_time DESC) AS SeqNum,
                        transaction_time AS DateSent,
                        amount AS Amount,
                        CASE WHEN sender_account   = @AccountNo THEN receiver_account ELSE NULL END AS SentTo,
                        CASE WHEN receiver_account = @AccountNo THEN sender_account   ELSE NULL END AS ReceivedFrom
                    FROM Transactions"
                    + whereClause +
                    " ORDER BY transaction_time DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountNo", accountNumber);
                    if (!string.IsNullOrEmpty(fromDate)) cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    if (!string.IsNullOrEmpty(toDate)) cmd.Parameters.AddWithValue("@ToDate", toDate + " 23:59:59");
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd)) sda.Fill(dt);
                }
            }
            return dt;
        }
    }
}