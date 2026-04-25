using System;
using System.Data;
using System.Data.SqlClient;

namespace Main.Classes
{
    public class TransactionManager
    {
        // ==========================================
        // TRANSACTION PROCESSING
        // ==========================================
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

        // ==========================================
        // HELPERS FOR DASHBOARD STATS
        // ==========================================
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

        // ==========================================
        // TABLE FETCHING
        // ==========================================

        // FIX 1 (Bug 1): Running balance using SUM() OVER() window function.
        // FIX 4 (SeqNum): ROW_NUMBER() as SeqNum.
        // FIX 5 (Date filter + running balance): Subquery computes full running balance
        //        first, then outer query applies date filter so Balance is always correct.
        // FIX 6 (Description): Transfer rows now show "Sent" or "Received" instead of
        //        "Transfer" — determined by whether the user is the sender or receiver.
        //   OLD: transaction_type AS Description  ->  always shows "Transfer"
        //   NEW: CASE WHEN transaction_type = 'Transfer'
        //              THEN CASE WHEN sender_account = @AccountNo THEN 'Sent' ELSE 'Received' END
        //             ELSE transaction_type  ->  Deposit / Withdraw unchanged
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

                            -- FIX 6: Show Sent/Received for transfers instead of Transfer
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

        // FIX 2 (Bug 2): toDate + " 23:59:59" to include the full end day.
        // FIX 3 (SeqNum): ROW_NUMBER() for clean per-table sequence numbers.
        // FIX 7 (Type filter + SeqNum): Wrapped in subquery so ROW_NUMBER() runs
        //        AFTER the type filter is applied. Without this, selecting "Deposit"
        //        would still number rows based on ALL deposits+withdrawals before
        //        filtering, causing gaps (e.g. 1, 3, 5 instead of 1, 2, 3).
        //   OLD: ROW_NUMBER() at top level then AND transaction_type = 'Deposit' appended
        //        -> ROW_NUMBER counts all rows first, filter applied after = wrong SeqNums
        //   NEW: Filter inside inner query first, then ROW_NUMBER() in outer query
        //        -> ROW_NUMBER only counts the rows that pass the filter = always 1, 2, 3...
        public DataTable GetDepositsWithdrawals(int accountNumber, string fromDate, string toDate, string type)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                // Build a single flat WHERE clause with all conditions.
                // ROW_NUMBER() is computed in the outer SELECT after all filters are applied,
                // so SeqNum is always 1, 2, 3... for exactly the rows returned.
                string whereClause = @"
                    WHERE (sender_account = @AccountNo OR receiver_account = @AccountNo)
                    AND transaction_type IN ('Deposit', 'Withdraw')";

                // Inline type filter using literal SQL values (safe — only our own constants, never user input)
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

        // FIX 2 (Bug 2): toDate + " 23:59:59" to include the full end day.
        // FIX 3 (SeqNum): Same subquery pattern as GetDepositsWithdrawals —
        //        ROW_NUMBER() runs after all filters so SeqNum is always 1, 2, 3...
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