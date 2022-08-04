using Business.Abstract;
using Core.Entities.Concrete;
using Core.Utilities.Security.Jwt;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private IAuthService _authService;





        public AuthController(IAuthService authService)
        {
            _authService = authService;


        }

        [HttpPost("login")]
        public ActionResult Login(UserForLoginDto userForLoginDto)
        {
            var userToLogin = _authService.Login(userForLoginDto);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }

            var result = _authService.CreateAccessToken(userToLogin.Data);
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

        [HttpPost("register")]
        public ActionResult Register(UserForRegisterDto userForRegisterDto)
        {
            var userExists = _authService.UserExists(userForRegisterDto.Email);
            if (!userExists.Success)
            {
                return BadRequest(userExists.Message);
            }

            var registerResult = _authService.Register(userForRegisterDto, userForRegisterDto.Password);
            var result = _authService.CreateAccessToken(registerResult.Data);
            if (registerResult.Success)
            {
                return Ok(registerResult.Data);
            }

            return BadRequest(registerResult.Message);
        }

        [HttpPut("update")]
        [Authorize(Roles = "UpdateRole")]
        public User Update(User user)
        {
           var context= _authService.UpdateUser(user);
           using (var DbContext = new NorthwindContext())
              {
                  DbContext.Users.Update(user);
                  return user;
              }
      


        }

            
    }
}
