using AtlasCity.TimeProof.Abstractions.DAO;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Abstractions.Services
{
    public interface IUserService
    {
        Task<UserDao> GetUserById(string userId, CancellationToken cancellationToken);

        Task<UserDao> CreateUser(UserDao user, CancellationToken cancellationToken);

        Task UpdateUser(string userId, string firstName, string lastName, CancellationToken cancellationToken);

        Task DeleteUser(string userId, CancellationToken cancellationToken);

        Task SendWelcomeEmailAndStoreKey(string userId, string keyDetail, CancellationToken cancellationToken);

        Task<KeyDao> GetUserKey(string userId, CancellationToken cancellationToken);
    }
}
