using WebApiCRUD.Models;

namespace WebApiCRUD.Services;

public interface IAuthServices
{
    Task<AuthModel> RegisterAsync(RegisterModel model);
    Task<AuthModel> GetTokenAsync(TokenRequstModel model);
    Task<string> AddRoleAsync(AddRoleModel model);
}
