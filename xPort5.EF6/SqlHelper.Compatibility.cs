using System;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace xPort5.EF6
{
    public class SqlHelper
    {
        public static SqlHelper Default = new SqlHelper();

        public SqlDataReader ExecuteReader(string sql)
        {
            string entityConnectionString = ConfigurationManager.ConnectionStrings["xPort5Entities"].ConnectionString;
            var builder = new EntityConnectionStringBuilder(entityConnectionString);
            string providerConnectionString = builder.ProviderConnectionString;

            SqlConnection conn = new SqlConnection(providerConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public SqlDataReader ExecuteReader(CommandType commandType, string sql)
        {
            return ExecuteReader(sql);
        }

        public SqlDataReader ExecuteReader(SqlCommand cmd)
        {
            string entityConnectionString = ConfigurationManager.ConnectionStrings["xPort5Entities"].ConnectionString;
            var builder = new EntityConnectionStringBuilder(entityConnectionString);
            string providerConnectionString = builder.ProviderConnectionString;

            SqlConnection conn = new SqlConnection(providerConnectionString);
            conn.Open();
            cmd.Connection = conn;
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public DataSet ExecuteDataSet(CommandType commandType, string sql)
        {
            string entityConnectionString = ConfigurationManager.ConnectionStrings["xPort5Entities"].ConnectionString;
            var builder = new EntityConnectionStringBuilder(entityConnectionString);
            string providerConnectionString = builder.ProviderConnectionString;

            using (SqlConnection conn = new SqlConnection(providerConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = commandType;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                return ds;
            }
        }

        public int ExecuteNonQuery(CommandType commandType, string sql)
        {
            string entityConnectionString = ConfigurationManager.ConnectionStrings["xPort5Entities"].ConnectionString;
            var builder = new EntityConnectionStringBuilder(entityConnectionString);
            string providerConnectionString = builder.ProviderConnectionString;

            using (SqlConnection conn = new SqlConnection(providerConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = commandType;
                return cmd.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(CommandType commandType, string sql)
        {
            string entityConnectionString = ConfigurationManager.ConnectionStrings["xPort5Entities"].ConnectionString;
            var builder = new EntityConnectionStringBuilder(entityConnectionString);
            string providerConnectionString = builder.ProviderConnectionString;

            using (SqlConnection conn = new SqlConnection(providerConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = commandType;
                return cmd.ExecuteScalar();
            }
        }

        // Overloads for stored procedures with parameters
        public DataSet ExecuteDataSet(string storedProcedureName, SqlParameter[] parameters)
        {
            string entityConnectionString = ConfigurationManager.ConnectionStrings["xPort5Entities"].ConnectionString;
            var builder = new EntityConnectionStringBuilder(entityConnectionString);
            string providerConnectionString = builder.ProviderConnectionString;

            using (SqlConnection conn = new SqlConnection(providerConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(storedProcedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                return ds;
            }
        }

        public int ExecuteNonQuery(string storedProcedureName, SqlParameter[] parameters)
        {
            string entityConnectionString = ConfigurationManager.ConnectionStrings["xPort5Entities"].ConnectionString;
            var builder = new EntityConnectionStringBuilder(entityConnectionString);
            string providerConnectionString = builder.ProviderConnectionString;

            using (SqlConnection conn = new SqlConnection(providerConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(storedProcedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(string storedProcedureName, SqlParameter[] parameters)
        {
            string entityConnectionString = ConfigurationManager.ConnectionStrings["xPort5Entities"].ConnectionString;
            var builder = new EntityConnectionStringBuilder(entityConnectionString);
            string providerConnectionString = builder.ProviderConnectionString;

            using (SqlConnection conn = new SqlConnection(providerConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(storedProcedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteScalar();
            }
        }
    }
}
