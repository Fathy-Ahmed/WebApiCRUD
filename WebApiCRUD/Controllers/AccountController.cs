using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiCRUD.DTO;
using WebApiCRUD.Models;
using WebApiCRUD.Services;

namespace WebApiCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;
        private readonly IAuthServices _authServices;

        public AccountController
            (UserManager<ApplicationUser> userManager,IConfiguration config,IAuthServices authServices)
        {
            this.userManager = userManager;
            this.config = config;
            this._authServices = authServices;
        }


        [HttpPost("register1")]
        public async Task<IActionResult> Register1(RegisterDTO registerDTO)
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
        //---------------------------------------------------------------------------
        [HttpPost("register2")]
        public async Task<IActionResult> Register2([FromBody]RegisterModel model)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);
            var result=await _authServices.RegisterAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        //---------------------------------------------------------------------------
        [HttpPost("login1")]
        public async Task<IActionResult> Login1(LoginDTO loginDTO)
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
        //---------------------------------------------------------------------------
        [HttpPost("login2")]
        public async Task<IActionResult> Login2([FromBody] TokenRequstModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authServices.GetTokenAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        //---------------------------------------------------------------------------
        [HttpPost("login2")]
        public async Task<IActionResult> AddUsertoRole([FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authServices.AddRoleAsync(model);
            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }
        //---------------------------------------------------------------------------

    }
}
