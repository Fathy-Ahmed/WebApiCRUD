using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiCRUD.DTO;
using WebApiCRUD.Models;

namespace WebApiCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;

        public AccountController(UserManager<ApplicationUser> userManager,IConfiguration config)
        {
            this.userManager = userManager;
            this.config = config;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new()
                {
                    UserName= registerDTO.UserName,
                    Email=registerDTO.Email,
                };

              IdentityResult identityResult= await userManager.CreateAsync(applicationUser,registerDTO.Password);
                if (identityResult.Succeeded)
                {
                    return Ok("Created");
                }
                else
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (ModelState.IsValid)
            {
                // Check
                ApplicationUser applicationUser = (await userManager.FindByNameAsync(loginDTO.UserName));
                if (applicationUser != null)
                {
                    bool found = await userManager.CheckPasswordAsync(applicationUser, loginDTO.Password);
                    if (found)
                    {
                        // Claims
                        List<Claim> UserClaims = new();
                        // Add UserName
                        UserClaims.Add(new Claim(ClaimTypes.Name, applicationUser.UserName));
                        // Add User Id
                        UserClaims.Add(new Claim(ClaimTypes.NameIdentifier, applicationUser.Id));
                        // Add User Roles
                        var UserRoles=await userManager.GetRolesAsync(applicationUser);
                        foreach (var role in UserRoles)
                        {
                            UserClaims.Add(new Claim(ClaimTypes.Role, role));
                        }
                        // Add Jti
                        UserClaims.Add(new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()));
                        //-----------------------------------------------------------------------------

                        // Symmetric Key
                        SymmetricSecurityKey SignInKey = new(Encoding.UTF8.GetBytes(config["JWT:SecritKey"]));
                        // signingCredentials
                        SigningCredentials signingCreds=new(SignInKey, SecurityAlgorithms.HmacSha256);

                        // design Token
                        JwtSecurityToken token = new JwtSecurityToken(
                            issuer: config["JWT:IssuerIP"],
                            audience: config["JWT:AudienceIP"],
                            expires: DateTime.Now.AddDays(1),
                            claims: UserClaims,
                            signingCredentials: signingCreds
                            );
                        // Generate Token
                        return Ok(new
                        {
                            Token=new JwtSecurityTokenHandler().WriteToken(token),
                            expiration= DateTime.Now.AddDays(1) // or => token.ValidTo
                        });
                    }

                }

                ModelState.AddModelError("", "username or password wrong");

            }
            return BadRequest(ModelState);
        }

    }
}
