using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.Util
{
    public class DataAccess
    {
        readonly static oneportalEntities _context = new oneportalEntities();
        // executes a command and returns the results as a DataTable object
        public static DataTable ExecuteSelectCommand(SqlCommand command)
        {
            // The DataTable to be returned
            DataTable table;

            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Open the data connection
                command.Connection.Open();

                // Execute the command and save the results in a DataTable
                SqlDataReader reader = command.ExecuteReader();
                table = new DataTable();
                table.Load(reader);

                // Close the reader
                reader.Close();

            }
            catch (NullReferenceException ex)
            {
                //Utilities.LogError(ex);
                throw ex;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                // Close the connection
                command.Connection.Close();
            }
            return table;
        }

        // executes a command and returns the results as a DataTable object
        public static SqlDataReader ExecuteSelectReaderCommand(SqlCommand command)
        {
            // The DataReader to be returned
            SqlDataReader reader;

            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Open the data connection
                command.Connection.Open();

                // Execute the command and save the results in a DataTable
                reader = command.ExecuteReader();


            }
            catch (NullReferenceException ex)
            {
                //Utilities.LogError(ex);
                throw ex;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                // Close the connection
                command.Connection.Close();
            }
            return reader;
        }






        // creates and prepares a new Command object on a new connection
        public static SqlCommand CreateCommand()
        {

            // Obtain the database connection string


            // Obtain a database specific connection object
            SqlConnection conn = (SqlConnection)_context.Database.Connection;

            // Create a database command object
            SqlCommand comm = new SqlCommand();

            //Set the connection of the command object
            comm.Connection = conn;

            // Set the command type to stored procedure
            comm.CommandType = CommandType.StoredProcedure;

            // Return the initialized command object
            return comm;

        }

        public static SqlConnection CreateConnection()
        {
            // Obtain the database provider name
            // string dataProviderName = ConStringConfig.DbProviderName;

            // Obtain the database connection string
            //string connectionString = ConStringConfig.DbConnectionString;

            // Obtain a database specific connection object
            SqlConnection conn = (SqlConnection)_context.Database.Connection;// new SqlConnection(connectionString);


            return conn;
        }

        // execute an update, delete, or insert command
        // and return the number of affected rows
        public static int ExecuteNonQuery(SqlCommand command)
        {

            //The affected no of rows
            int affectedRows = -1;

            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Open the data connection
                command.Connection.Open();

                // Execute the command and save the results in a DataTable
                affectedRows = command.ExecuteNonQuery();

                // Close the reader
                command.Connection.Close();

            }
            catch (NullReferenceException ex)
            {
                //Utilities.LogError(ex);
                throw ex;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                // Close the connection
                command.Connection.Close();
            }
            return affectedRows;
        }

        // execute a select command and return a single result as a string
        public static string ExecuteScalar(SqlCommand command)
        {
            // The value to be returned
            string value = "";

            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Open the connection of the command
                command.Connection.Open();

                // Execute the command and get the value to be returned
                value = command.ExecuteScalar().ToString();

            }
            catch (Exception ex)
            {
                // Log eventual errors and rethrow them
                //Utilities.LogError(ex);
                throw ex;
            }
            finally
            {

                // Close the connection
                command.Connection.Close();

            }

            // return the result
            return value;
        }


        public static DataSet GetDataSet(SqlCommand cmd)
        {
            // var conn = (SqlConnection)entity.Database.Connection;
            //var connstring = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //var conn = (SqlConnection)entity.Database.Connection;
            var connstring = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var conn = new SqlConnection(connstring))
            {
                conn.Open();
                cmd.Connection = conn;
                using (cmd)
                {
                    using (SqlDataAdapter a = new SqlDataAdapter(cmd))
                    {

                        DataSet t = new DataSet();
                        a.Fill(t);
                        return t;

                    }
                }
            }

        }


        public static SqlCommand TimeSheetCommand(SqlCommand cmd)
        {

            // Obtain the database connection string


            // Obtain a database specific connection object
            SqlConnection conn = (SqlConnection)_context.Database.Connection;

            // Create a database command object
            SqlCommand comm = new SqlCommand();

            //Set the connection of the command object
            comm.Connection = conn;

            // Set the command type to stored procedure
            comm.CommandType = CommandType.StoredProcedure;

            // Return the initialized command object
            return comm;

        }
    }
}
