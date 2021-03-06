﻿using System;
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
        public OdbcConnection databaseConnection { get; }

        /// <summary>
        /// Construct the Database with a specified connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public Database(string connectionString) {
            databaseConnection = new OdbcConnection(connectionString);
        }

        /// <summary>
        /// Execute a query.
        /// </summary>
        /// <param name="query">Query to be executed.</param>
        /// <returns>DataSet containing the result of the query.</returns>
        public DataSet ExecuteQuery(string query) {
            OdbcDataAdapter dbAdapter = new OdbcDataAdapter(query, databaseConnection);

            databaseConnection.Open();

            DataSet data = new DataSet();
            dbAdapter.Fill(data);

            databaseConnection.Close();

            return data;
        }

        /// <summary>
        /// Execute a command.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        public void ExecuteCommand(string command) {
            OdbcCommand dbCmd = new OdbcCommand(command, databaseConnection);

            databaseConnection.Open();
            dbCmd.ExecuteNonQuery();
            databaseConnection.Close();
        }

        /// <summary>
        /// Executes a series of commands. If any of the commands fail, no changes will be made to the database.
        /// </summary>
        /// <param name="commands">The commands to be executed.</param>
        /// <returns>Return a boolean indicating whether the commands were successful.</returns>
        public bool ExecuteCommands(params string[] commands) {
            OdbcCommand cmd = new OdbcCommand();
            cmd.Connection = databaseConnection;
            OdbcTransaction transaction = null;
            databaseConnection.Open();

            try {
                transaction = databaseConnection.BeginTransaction();
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
                    databaseConnection.Close();
                }
                // Transaction failed, so return false.
                return false;
            }

            // Close connection and return true, indicating that the transaction was successful.
            databaseConnection.Close();
            return true;
        }

        /// <summary>
        /// Get all of the rows from a specified table.
        /// </summary>
        /// <param name="table">The table from which the rows are retrieved.</param>
        /// <returns>Collection of rows.</returns>
        public DataRowCollection GetAll(string table) {
            DataRowCollection rows = ExecuteQuery(string.Format("SELECT * FROM {0};", table)).Tables[0].Rows;
            return rows;
        }

        /// <summary>
        /// Get all of the rows from a specified table, and specify which columns to retrieve.
        /// </summary>
        /// <param name="table">The table from which the rows are retrieved.</param>
        /// <param name="columns">The columns to include in the result.</param>
        /// <returns>Collection of rows.</returns>
        public DataRowCollection GetAll(string table, params string[] columns) {
            string query = "SELECT {0} FROM {1};";

            string columnString = "";
            for (int i = 0; i < columns.Length; i++) {
                if (i < columns.Length - 1)
                    columnString += columns[i] + ", ";
                else columnString += columns[i];
            }

            query = string.Format(query, columnString, table);

            DataRowCollection rows = ExecuteQuery(query).Tables[0].Rows;
            return rows;
        }
    }
}
