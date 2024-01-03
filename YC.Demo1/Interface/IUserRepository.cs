using YC.Demo1.Models;

namespace YC.Demo1.Interface
{
    public interface IUserRepository
    {
        public Task<(bool IsSuccess, User data)> CheckAccount(Login login);
    }
}

