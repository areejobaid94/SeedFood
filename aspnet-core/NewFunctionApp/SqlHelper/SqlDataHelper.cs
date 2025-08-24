/// <summary>
/// The Common namespace.
/// </summary>
namespace NewFunctionApp
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;

    /// <summary>
    /// The programming model for the Provider Independent Model is based on the use
    /// of the Factory design pattern and provides a single API to access databases
    /// across multiple providers, this is general static class that will be based on this Model to support two types
    /// of providers (SQL provider and oracle Provider).
    /// </summary>
    public static class SqlDataHelper
    {
        /*
         *  The programming model for the "Provider Independent Model" is based on the use
         *  of the Factory design pattern and provides a single API to access databases
         *  across multiple providers.
         *  this is general static class that will be based on this Model to support two types
         *  of providers (SQL provider & oracle Provider).
         */



        private static string dbConnectionString;


        /// <summary>
        /// Gets or Sets the database connection string.
        /// </summary>
        /// <value>The database connection string.</value>
        public static string DBConnectionString
        {
            get
            {
                string configurationConnectionString = string.Empty;
                try
                {
                    //var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


                    //configurationConnectionString = config.GetSection("ConnectionStrings")["Default"];// System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                    //if (string.IsNullOrWhiteSpace(configurationConnectionString))
                    //{
                    //    throw new Exception("The Configuration setting is empty in the Configuration file(web.config) which represents the Configuration Connection String of the database.");
                    //}
                    configurationConnectionString = Environment.GetEnvironmentVariable("DBConnectionString");
                    return configurationConnectionString;
                }

                //catch (ConfigurationErrorsException configurationException)
                //{
                //    throw new Exception("The Configuration setting is missing in the Configuration file(web.config) which represent the Configuration Connection String of the database.", configurationException);
                //}
                catch (Exception ex)
                {
                    throw new Exception("Unable to get the from the configuration source", ex);
                }
            }
        }

        /// <summary>
        /// Gets the Data provider name, that based on it we
        /// will create strongly-typed object for DbProviderFactory .
        /// </summary>
        public static string DBProviderInvariantName
        {
            get
            {
                return "System.Data.SqlClient";
            }
        }

        /// <summary>
        /// This function will execute the stored procedure and return
        /// list of result as generic type .
        /// it will depends on the data factory and data connection properties .
        /// </summary>
        /// <typeparam name="T">Type of object to be retrieved.</typeparam>
        /// <param name="spName">Stored Procedure to executed.</param>
        /// <param name="param">List of stored procedure parameter.</param>
        /// <param name="converter">Converter function to convert the retrieved data to object of type T.</param>
        /// <returns>General collection object represent the data in the dataReader object. </returns>
        public static IList<T> ExecuteReader<T>(string spName, DbParameter[] param, Converter<IDataReader, T> converter, string connectionString = null)
        {
            IList<T> results = new List<T>();
            IDataReader reader = null;
            DbCommand dbCommand = null;
            DbConnection dataConnection;

            try
            {
                dataConnection = new SqlConnection(DBConnectionString); //DataProviderFactory.CreateConnection();
                dataConnection.ConnectionString = connectionString != null ? connectionString : DBConnectionString;
                dbCommand = dataConnection.CreateCommand();
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.CommandText = spName;

                if (dbCommand.Connection.State == ConnectionState.Closed)
                {
                    dbCommand.Connection.Open();
                }

                if (param != null)
                {
                    dbCommand.Parameters.AddRange(param);
                }

                reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(converter(reader));
                }
            }catch(Exception ex)
            {


            }
            finally
            {
                dbCommand.Connection.Close();
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return results;
        }

        /// <summary>
        /// Execute stored procedure and return object that represent the retrieved data by using converter functions.
        /// </summary>
        /// <typeparam name="T">Type of object to be returned.</typeparam>
        /// <param name="spName">Stored procedure name.</param>
        /// <param name="param">List of parameter.</param>
        /// <param name="converter">Converter functions.</param>
        /// <returns>Object represent the retrieved data.</returns>
        public static T ExecuteItem<T>(string spName, DbParameter[] param, Converter<IDataReader, T> converter, string connectionString = null) where T : new()
        {
            T result = default(T);
            IDataReader reader = null;
            DbCommand dbCommand = null;
            DbConnection dataConnection;

            try
            {
                dataConnection = new SqlConnection(DBConnectionString); //DataProviderFactory.CreateConnection();
                dataConnection.ConnectionString = connectionString != null ? connectionString : DBConnectionString;
                dbCommand = dataConnection.CreateCommand();
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.CommandText = spName;

                if (param != null)
                {
                    dbCommand.Parameters.AddRange(param);
                }

                dbCommand.Connection.Open();

                reader = dbCommand.ExecuteReader();

                //Amer Check if this condition is true and avoid us null exception.
                if (reader.Read())
                {
                    result = converter(reader);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                dbCommand.Connection.Close();
                dbCommand.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Execute stored procedure and retrieve scalar object.
        /// </summary>
        /// <param name="spName">Stored procedure name.</param>
        /// <param name="param">List of parameter.</param>
        /// <returns>Object that represent the retrieved scalar.</returns>
        public static object ExecuteScalar(string spName, DbParameter[] param, string connectionString = null)
        {
            object results = null;
            DbCommand dbCommand = null;
            DbConnection dataConnection;

            try
            {
                dataConnection = new SqlConnection(DBConnectionString);//DataProviderFactory.CreateConnection();
                dataConnection.ConnectionString = connectionString != null ? connectionString : DBConnectionString;
                dbCommand = dataConnection.CreateCommand();
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.CommandText = spName;

                if (param != null)
                {
                    dbCommand.Parameters.AddRange(param);
                }

                if (dbCommand.Connection.State != ConnectionState.Open)
                {
                    dbCommand.Connection.Open();
                }

                results = dbCommand.ExecuteScalar();
            }
            finally
            {
                dbCommand.Connection.Close();
                dbCommand.Dispose();
            }

            return results;
        }

        /// <summary>
        /// Execute stored procedure .
        /// </summary>
        /// <param name="spName">Stored procedure name.</param>
        /// <param name="param">List of stored procedure parameters.</param>
        /// <returns>Number of effected rows .</returns>
        public static int? ExecuteNoneQuery(string spName, DbParameter[] param, string connectionString = null)
        {
            int? result = null;
            DbCommand dbCommand = null;
            DbConnection dataConnection;

            try
            {
                dataConnection = new SqlConnection(DBConnectionString);//DataProviderFactory.CreateConnection();
                dataConnection.ConnectionString = connectionString != null ? connectionString : DBConnectionString;
                dbCommand = dataConnection.CreateCommand();
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.CommandText = spName;

                if (param != null)
                {
                    dbCommand.Parameters.AddRange(param);
                }

                if (dbCommand.Connection.State == ConnectionState.Closed)
                {
                    dbCommand.Connection.Open();
                }

                result = dbCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            
            }
            finally
            {
                dbCommand.Connection.Close();
                dbCommand.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Check if column name exist in data reader
        /// </summary>
        /// <param name="reader">data reader</param>
        /// <param name="columnName">column name</param>
        /// <returns></returns>
        public static bool IsColumnExistsInDataReader(System.Data.IDataReader reader, string columnName)
        {
            using (var schemaTable = reader.GetSchemaTable())
            {
                if (schemaTable != null)
                {
                    schemaTable.DefaultView.RowFilter = string.Format("ColumnName= '{0}'", columnName);
                }

                return schemaTable != null && (schemaTable.DefaultView.Count > 0) && !string.IsNullOrEmpty(reader[columnName].ToString());
            }
        }

        /// <summary>
        /// Add output parameter
        /// </summary>
        /// <param name="parameterName">parameter name</param>
        /// <returns>sql parameter</returns>
        public static SqlParameter OutPutParameter(string parameterName, System.Data.DbType dataType)
        {
            SqlParameter parOutput = new SqlParameter();
            parOutput.DbType = dataType;
            if (dataType == DbType.String)
            {
                parOutput.Size = 1000;
            }
            parOutput.ParameterName = parameterName;
            parOutput.Value = -1;
            parOutput.Direction = ParameterDirection.Output;
            return parOutput;
        }

        public static dynamic GetValue(System.Data.IDataReader reader, string columnName)
        {
            dynamic value = default(dynamic);

            var columns = SqlDataHelper.GetColumns(reader);
            if (columns.ContainsKey(columnName))
            {
                switch (columns[columnName])
                {
                    case SystemDataTypes.Boolean:
                        if (reader[columnName] != DBNull.Value &&
                            !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = Boolean.Parse(reader[columnName].ToString());
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<bool>);
                        //}
                        break;

                    case SystemDataTypes.DateTime:
                        if (reader[columnName] != DBNull.Value &&
                            !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = DateTime.Parse(reader[columnName].ToString());
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<System.DateTime>);
                        //}
                        break;

                    case SystemDataTypes.Decimal:
                        if (reader[columnName] != DBNull.Value &&
                           !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = Decimal.Parse(reader[columnName].ToString());
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<decimal>);
                        //}
                        break;

                    case SystemDataTypes.Double:
                        if (reader[columnName] != DBNull.Value &&
                           !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = Double.Parse(reader[columnName].ToString());
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<decimal>);
                        //}
                        break;

                    case SystemDataTypes.Int16:
                        if (reader[columnName] != DBNull.Value &&
                           !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = Int16.Parse(reader[columnName].ToString());
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<Int16>);
                        //}
                        break;

                    case SystemDataTypes.Int32:
                        if (reader[columnName] != DBNull.Value &&
                           !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = Int32.Parse(reader[columnName].ToString());
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<Int32>);
                        //}
                        break;

                    case SystemDataTypes.Int64:
                        if (reader[columnName] != DBNull.Value &&
                           !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = Int64.Parse(reader[columnName].ToString());
                        }
                        else
                        {
                            value = 0;
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<Int64>);
                        //}
                        break;

                    case SystemDataTypes.String:
                        if (reader[columnName] != DBNull.Value)
                        {
                            value = reader[columnName].ToString();
                        }
                        else
                        {
                            value = string.Empty;
                        }
                        break;

                    case SystemDataTypes.TimeSpan:
                        if (reader[columnName] != DBNull.Value &&
                           !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = TimeSpan.Parse(reader[columnName].ToString());
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<TimeSpan>);
                        //}
                        break;
                    //case SystemDataTypes.SqlGeography:
                    //    if (reader[columnName] != DBNull.Value &&
                    //       !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                    //    {
                    //        var temp = reader[columnName].ToString();
                    //        value = System.Data.Entity.Spatial.DbGeography.FromText(temp);
                    //    }

                    //    break;
                    default:
                        value = default(dynamic);
                        break;
                }
            }

            return value;
        }

        public static dynamic GetValue<T>(System.Data.IDataReader reader, string columnName)
        {
            dynamic value = default(dynamic);

            var columns = SqlDataHelper.GetColumns(reader);
            if (columns.ContainsKey(columnName))
            {
                switch (columns[columnName])
                {
                    case SystemDataTypes.Boolean:
                        if (reader[columnName] != DBNull.Value &&
                            !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = Boolean.Parse(reader[columnName].ToString());
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<bool>);
                        //}
                        break;

                    case SystemDataTypes.DateTime:
                        if (reader[columnName] != DBNull.Value &&
                            !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = DateTime.Parse(reader[columnName].ToString());
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<System.DateTime>);
                        //}
                        break;

                    case SystemDataTypes.Decimal:
                        if (reader[columnName] != DBNull.Value &&
                           !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = Decimal.Parse(reader[columnName].ToString());
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<decimal>);
                        //}
                        break;

                    case SystemDataTypes.Double:
                        if (reader[columnName] != DBNull.Value &&
                           !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = Double.Parse(reader[columnName].ToString());
                        }
                        else
                        {
                            value = new double();
                        }
                        break;

                    case SystemDataTypes.Int16:
                        if (reader[columnName] != DBNull.Value &&
                           !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = Int16.Parse(reader[columnName].ToString());
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<Int16>);
                        //}
                        break;

                    case SystemDataTypes.Int32:
                        if (reader[columnName] != DBNull.Value &&
                           !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = Int32.Parse(reader[columnName].ToString());
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<Int32>);
                        //}
                        break;

                    case SystemDataTypes.Int64:
                        if (reader[columnName] != DBNull.Value &&
                           !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = Int64.Parse(reader[columnName].ToString());
                        }
                        else
                        {
                            value = 0;
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<Int64>);
                        //}
                        break;

                    case SystemDataTypes.String:
                        if (reader[columnName] != DBNull.Value)
                        {
                            value = reader[columnName].ToString();
                        }
                        else
                        {
                            value = string.Empty;
                        }
                        break;

                    case SystemDataTypes.TimeSpan:
                        if (reader[columnName] != DBNull.Value &&
                           !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = TimeSpan.Parse(reader[columnName].ToString());
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<TimeSpan>);
                        //}
                        break;

                    case SystemDataTypes.Guid:
                        if (reader[columnName] != DBNull.Value &&
                           !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                        {
                            value = Guid.Parse(reader[columnName].ToString());
                        }
                        //else
                        //{
                        //    value = typeof(Nullable<TimeSpan>);
                        //}
                        break;
                    //case SystemDataTypes.SqlGeography:
                    //    if (reader[columnName] != DBNull.Value &&
                    //       !string.IsNullOrWhiteSpace(reader[columnName].ToString()))
                    //    {
                    //        var temp = reader[columnName].ToString();
                    //        value = System.Data.Entity.Spatial.DbGeography.FromText(temp);
                    //    }

                    //    break;
                    default:
                        value = default(T);
                        break;
                }
            }
            else
            {
                value = default(T);
            }
            return value;
        }

        public static Dictionary<string, string> GetColumns(System.Data.IDataReader reader)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            using (var schemaTable = reader.GetSchemaTable())
            {
                foreach (DataRow row in schemaTable.Rows)
                {
                    var name = row["ColumnName"].ToString();
                    var dataType = row["DataType"].ToString();
                    result.Add(name, dataType);
                    //append these to a string or StringBuilder for writing out later...
                }
            }

            return result;
        }
    }
}