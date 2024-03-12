using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Watsons.Common
{
    public class SysCredential
    {
        private record Credential(string Host, string UserId, string Password, string Instance, string InitialCatalog, string DatabaseType, string? Port = null);

        public static string GetConnectionString(string instance, string? initialCatalog)
        {
            var credential = new Credential("", "", "", "", "", "", "");

            using (SqlConnection connection = new SqlConnection("Server=10.98.32.185;Database=SysCred;User ID= sa;Password=!QAZ2wsx#EDC;TrustServerCertificate=true;"))
            {
                connection.Open();

                string sql = @"SELECT * FROM ServerCred WHERE Name = @name";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@name", instance);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var host = EncryptionEngineCore.CoreDecrypt((string)reader["Host"]);
                    var userId = EncryptionEngineCore.CoreDecrypt((string)reader["UserId"]);
                    var password = EncryptionEngineCore.CoreDecrypt((string)reader["Password"]);
                    var databaseType = (string)reader["DatabaseType"];
                    var port = reader["Port"] == DBNull.Value ? "Not Available" : EncryptionEngineCore.CoreDecrypt((string)reader["Port"]);
                    credential = new Credential(host, userId, password, instance, initialCatalog, databaseType, port);
                }
                reader.Close();
                connection.Close();
            }

            string connectionString = "";
            if (credential.DatabaseType == "TSQL")
                connectionString = BuildTsqlConnectionString(credential);

            if (credential.DatabaseType == "Oracle")
                connectionString = BuildOracleConnectionString(credential);

            return connectionString;
        }
        private static string BuildTsqlConnectionString(Credential credential)
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = credential.Host;
            sqlBuilder.UserID = credential.UserId;
            sqlBuilder.Password = credential.Password;
            sqlBuilder.InitialCatalog = credential.InitialCatalog ?? "Master";
            //sqlBuilder.IntegratedSecurity = true; // for windows authentication
            sqlBuilder.TrustServerCertificate = true;
            string connectionString = sqlBuilder.ConnectionString;

            return connectionString;
        }
        private static string BuildOracleConnectionString(Credential credential)
        {
            string connectionString = $"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST ={credential.Host})(PORT ={credential.Port})) (CONNECT_DATA =(SERVICE_NAME ={credential.Instance})));User id={credential.UserId};Password={credential.Password};";

            return connectionString;
        }

    }
}
