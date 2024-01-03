using Dapper;
using System.Data;
using YC.Demo1.Helpers;
using YC.Demo1.Interface;

namespace YC.Demo1.Models
{
    public class UserRepository : IUserRepository
    {
        private readonly DBHelper _context;
        public UserRepository(DBHelper context)
        {
            _context = context;
        }
        public async Task<(bool IsSuccess, User data)> CheckAccount(Login login)
        {
            using (var connection = _context.CreateConnection())
            {
                SqlMapper.GridReader results = await connection.QueryMultipleAsync(
                    "EXEC [pubs].[dbo].[CheckAccount] @Account, @Password;",
                    new { 
                        Account = login.UserName,
                        Password = login.Password
                    });
                var result = results.Read<SP_API_RESULT>().FirstOrDefault();
                if(result.CODE != 200)
                    return (IsSuccess: false, data: new User());

                return (IsSuccess: true, data: results.Read<User>().FirstOrDefault());
            }
        }
    }
}
