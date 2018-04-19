using System;
using System.Data;
using System.Data.Odbc;

namespace Tools {
    /// <summary>
    /// This class is used as a simple way to connect to a database.
    /// </summary>
    public class Database {
        /// <summary>
        /// The object which makes the database connection.
        /// </summary>
        private OdbcConnection dbCon;

        /// <summary>
        /// Construct the Database with a specified connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public Database(string connectionString) {
            dbCon = new OdbcConnection(connectionString);
        }

        /// <summary>
        /// Execute a query.
        /// </summary>
        /// <param name="query">Query to be executed.</param>
        /// <returns>DataSet containing the result of the query.</returns>
        public DataSet ExecuteQuery(string query) {
            OdbcDataAdapter dbAdapter = new OdbcDataAdapter(query, dbCon);

            dbCon.Open();

            DataSet data = new DataSet();
            dbAdapter.Fill(data);

            dbCon.Close();

            return data;
        }

        /// <summary>
        /// Execute a command.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        public void ExecuteCommand(string command) {
            OdbcCommand dbCmd = new OdbcCommand(command, dbCon);

            dbCon.Open();
            dbCmd.ExecuteNonQuery();
            dbCon.Close();
        }

        /// <summary>
        /// Executes a series of commands. If any of the commands fail, no changes will be made to the database.
        /// </summary>
        /// <param name="commands">The commands to be executed.</param>
        /// <returns>Return a boolean indicating whether the commands were successful.</returns>
        public bool ExecuteCommands(params string[] commands) {
            OdbcCommand cmd = new OdbcCommand();
            cmd.Connection = dbCon;
            OdbcTransaction transaction = null;
            dbCon.Open();

            try {
                transaction = dbCon.BeginTransaction();
                cmd.Transaction = transaction;

                // Execute each query; if there is an exception, it will be caught before commiting.
                foreach (string c in commands) {
                    cmd.CommandText = c;
                    cmd.ExecuteNonQuery();
                }

                // Commit the transaction.
                transaction.Commit();
            } catch (Exception e) {
                // Attempt to roll back transaction.
                try {
                    transaction.Rollback();
                } catch {
                    // Transaction failed to rollback.
                } finally {
                    // Close connection regardless of whether it rolled back or not.
                    dbCon.Close();
                }
                // Transaction failed, so return false.
                return false;
            }

            // Close connection and return true, indicating that the transaction was successful.
            dbCon.Close();
            return true;
        }
    }
}
