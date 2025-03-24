using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AnimeTaste.WebApi.Auth
{
    public class JwtHelper(IConfiguration configuration)
    {
        /// <summary>
        /// 颁发token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public string CreateToken(List<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"] ?? ""));

            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            var jwtSecurityToken = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                signingCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }


    }
}
