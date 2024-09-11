using System.ComponentModel.DataAnnotations;

namespace WebApiCRUD.Models;

public class RegisterModel
{
    [StringLength(100)]
    public string UserName { get; set; }
    [StringLength(128)]
    public string Email { get; set; }
    [StringLength(256)]
    public string Password { get; set; }
}
