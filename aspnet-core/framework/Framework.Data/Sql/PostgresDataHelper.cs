using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Framework.Data.Sql
{
    public static class PostgresDataHelper
    {
        /// <summary>
        /// Executes a PostgreSQL function that returns a result set.
        /// </summary>
        /// <typeparam name="T">The type of object each row will be mapped to.</typeparam>
        /// <param name="functionName">The function name (e.g., groups_get_all_per_user).</param>
        /// <param name="param">Array of NpgsqlParameters to pass to the function.</param>
        /// <param name="converter">A function that maps a single IDataReader row to an object of type T.</param>
        /// <param name="connectionString">PostgreSQL connection string.</param>
        /// <returns>List of objects of type T returned from the function.</returns>
        public static IList<T> ExecuteFunction<T>(
            string functionName,
            NpgsqlParameter[] param,
            Converter<IDataReader, T> converter,
            string connectionString)
        {
            IList<T> results = new List<T>();

            using (var dataConnection = new NpgsqlConnection(connectionString))
            using (var dbCommand = dataConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;

                // Build SELECT * FROM function(@param1, @param2...)
                var paramNames = param?.Select(p => "@" + p.ParameterName).ToArray() ?? Array.Empty<string>();
                dbCommand.CommandText = $"SELECT * FROM {functionName}({string.Join(", ", paramNames)})";

                if (param != null)
                    dbCommand.Parameters.AddRange(param);

                dataConnection.Open();

                using (var reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                        results.Add(converter(reader));
                }
            }

            return results;
        }

        /// <summary>
        /// Executes a PostgreSQL stored procedure (CALL).
        /// Usually procedures don't return result sets, but they can have OUT parameters.
        /// </summary>
        /// <param name="procedureName">The procedure name (e.g., reset_user_counters).</param>
        /// <param name="param">Array of NpgsqlParameters to pass to the procedure.</param>
        /// <param name="connectionString">PostgreSQL connection string.</param>
        public static void ExecuteProcedure(
            string procedureName,
            NpgsqlParameter[] param,
            string connectionString)
        {
            using (var dataConnection = new NpgsqlConnection(connectionString))
            using (var dbCommand = dataConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;

                // Build CALL procedure(@param1, @param2...)
                var paramNames = param?.Select(p => "@" + p.ParameterName).ToArray() ?? Array.Empty<string>();
                dbCommand.CommandText = $"CALL {procedureName}({string.Join(", ", paramNames)})";

                if (param != null)
                    dbCommand.Parameters.AddRange(param);

                dataConnection.Open();
                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a scalar PostgreSQL function that returns a single value (e.g., COUNT(*)).
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="functionName">The function name.</param>
        /// <param name="param">Parameters for the function.</param>
        /// <param name="connectionString">PostgreSQL connection string.</param>
        /// <returns>A single scalar value.</returns>
        public static T ExecuteScalarFunction<T>(
     string functionName,
     NpgsqlParameter[] param,
     string connectionString)
        {
            using (var dataConnection = new NpgsqlConnection(connectionString))
            using (var dbCommand = dataConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;

                var paramNames = param?.Select(p => "@" + p.ParameterName).ToArray() ?? new string[0];
                dbCommand.CommandText = $"SELECT {functionName}({string.Join(", ", paramNames)})";

                if (param != null)
                    dbCommand.Parameters.AddRange(param);

                dataConnection.Open();

                var result = dbCommand.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return default(T);

                return (T)result;
            }
        }
    }
}