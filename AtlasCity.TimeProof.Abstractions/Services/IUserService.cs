using System.Threading;
using System.Threading.Tasks;
using AtlasCity.TimeProof.Abstractions.DAO;

namespace AtlasCity.TimeProof.Abstractions.Services
{
    public interface IUserService
    {
        Task<UserDao> GetUserById(string userId, CancellationToken cancellationToken);

        Task<UserDao> CreateUser(UserDao user, CancellationToken cancellationToken);

        Task DeleteUser(string userId, CancellationToken cancellationToken);

        Task SendKeyAsEmailAttachment(string userId, string attachmentText, CancellationToken cancellationToken);
    }
}
