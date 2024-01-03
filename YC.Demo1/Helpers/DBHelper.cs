using System.Data;
using System.Data.SqlClient;

namespace YC.Demo1.Helpers
{
    public class DBHelper
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private string _key { get => "UElicApHOmPeaDENIadesTESMOHamPrA"; }
        public DBHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = LSYS.Security.Decrypt(_configuration.GetSection("DB:A0").Get<string>(), _key);
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
