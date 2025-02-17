using System.Threading.Tasks;

namespace AuraDecor.Core.Services.Contract
{
    public interface IRoleService
    {
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> AssignRoleToUserAsync(string email, string roleName);
    }
}