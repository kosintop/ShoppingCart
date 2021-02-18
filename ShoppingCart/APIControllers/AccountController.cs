using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShoppingCart.Data;
using ShoppingCart.Models;

namespace ShoppingCart.APIControllers
{
    [Route("api")]
    [AllowAnonymous]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private string _jwtIssuer;
        private string _jwtKey;

        public AccountController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration config
        )
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtIssuer = config["Jwt:Issuer"];
            _jwtKey = config["Jwt:Key"];
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register(RequestUserRegisterDTO request)
        {
            //begin transaction
            var transaction = _context.Database.BeginTransaction();

            //create user
            var newUser = new ApplicationUser();
            newUser.Email = request.Email;
            newUser.UserName = request.Email;

            var createUserResult = await _userManager.CreateAsync(newUser, request.Password);
            if (!createUserResult.Succeeded) return BadRequest(createUserResult.Errors);

            //add to role
            var addRoleResult = await _userManager.AddToRoleAsync(newUser, "Customer");
            if (!addRoleResult.Succeeded) return BadRequest(addRoleResult.Errors);

            //commit all changes, roll back if fail
            await transaction.CommitAsync();

            return Ok();
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> LogIn(RequestUserLogInDTO request)
        {
            //find user
            var user = await _userManager.FindByNameAsync(request.Email);
            if (user == null) return BadRequest("User not found");

            //prevent admin from login to api
            if (await _userManager.IsInRoleAsync(user, "Admin")) return BadRequest("Admin can't Login");

            //login and create token if success
            var result = await _signInManager.CheckPasswordSignInAsync(user,request.Password,false);
            if (!result.Succeeded) return BadRequest("Invalid Attempt");
             var token = await CreateToken(user.UserName);

            return Ok(token);
        }

        private async Task<string> CreateToken(string username)
        {
            //prepare list of all claims
            var claimList = new List<Claim>();

            //add user id
            var user = await _userManager.FindByNameAsync(username);
            claimList.Add(new Claim("Id", user.Id));

            //add role claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claimList.Add(new Claim(ClaimTypes.Role, role));
            }

            //sign
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenObject = new JwtSecurityToken(
                issuer: _jwtIssuer,
                claims: claimList,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
              );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return tokenString;
        }

    }
}
