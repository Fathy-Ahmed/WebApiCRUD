namespace WebApiCRUD.Models;

public class AuthModel
{
    public string Token { get; set; }
    public DateTime ExoiresOn { get; set; }
    public string Message { get; set; }
    public bool IsAuthenticated { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
}
