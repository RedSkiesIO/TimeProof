using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Repository
{
    public interface IUserRepository
    {
        Task<UserDao> GetUserById(string userId, CancellationToken cancellationToken);

        Task<UserDao> GetUserByEmail(string email, CancellationToken cancellationToken);

        Task<UserDao> CreateUser(UserDao user, CancellationToken cancellationToken);
        
        Task<UserDao> UpdateUser(UserDao user, CancellationToken cancellationToken);
        
        Task DeleteUser(string userId, CancellationToken cancellationToken);
    }
}
