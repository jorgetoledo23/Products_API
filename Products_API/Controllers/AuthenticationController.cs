using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Products_API.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
namespace Products_API.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthenticationController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Registro(UsuarioDTO userDTO)
        {
            if (userDTO == null) return BadRequest();

            Usuario usuario = new Usuario();
            usuario.Name = userDTO.Name;
            usuario.Email = userDTO.Email;
            usuario.Username = userDTO.Username;

            CreatePasswordHash(userDTO.Password,
                out byte[] passwordHash, out byte[] passwordSalt);

            usuario.PasswordHash = passwordHash;
            usuario.PasswordSalt = passwordSalt;

            _context.Add(usuario);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginIn(LoginDTO loginDTO)
        {
            if (loginDTO == null) return BadRequest();

            var user = _context.Usuarios.FirstOrDefault(u => u.Username == loginDTO.Username);

            if (user == null) return BadRequest("Usuario NO Encontrado");

            if (VerifyPasswordHash(loginDTO.Password, user.PasswordHash, user.PasswordSalt))
            {
                string tk = CreateToken(user);
                return Ok(tk);
            }
            else
            {
                return BadRequest("Credenciales Incorrectas");
            }
        }

        private void CreatePasswordHash(string password,
            out byte[] passwordHash,
            out byte[] passwordSalt) //By Ref
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8
                    .GetBytes(password));
            }
        }


        private bool VerifyPasswordHash(string password,
            byte[] passwordHash,
            byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8
                    .GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }


        private string CreateToken(Usuario user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "Free")
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _config.GetSection("AppSettings:Token").Value));

            var credential = new SigningCredentials(key,
                SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credential
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }





    }
}
