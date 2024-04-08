using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Watsons.Common.JwtHelpers
{
    public class JwtHelper
    {
        private readonly JwtSecurityToken _jwtSecurityToken;
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtHelper(IOptions<JwtSettings> jwtSettings, IHttpContextAccessor httpContextAccessor)
        {
            _jwtSettings = jwtSettings.Value;
            _httpContextAccessor = httpContextAccessor;

            // initialized
            var token = _httpContextAccessor.HttpContext.Request.Cookies[_jwtSettings.CookieName];
            var handler = new JwtSecurityTokenHandler();
            if (!string.IsNullOrEmpty(token) && handler.CanReadToken(token))
            {
                _jwtSecurityToken = handler.ReadJwtToken(token);
            }
        }
        public string GenerateJwtToken(List<Claim>? claims = null)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string? DecodeJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token.Replace("Bearer ", ""));
            var payload = jwtToken.Payload;

            Console.WriteLine("JWT Header:");
            Console.WriteLine(jwtToken.Header);
            Console.WriteLine("\nJWT Payload:");

            //foreach (var claim in jwtToken.Claims)
            //{
            //    $"{claim.Type}: {claim.Value}");
            //}

            return JsonSerializer.Serialize(payload);
        }

        public JwtPayload Payload()
        {
            var payload = _jwtSecurityToken.Payload;
            return payload;
        }
        public string? GetRole()
        {
            var role = _jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);
            return role?.Value;
        }
        public string? GetEmail()
        {
            var email = _jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
            return email?.Value;
        }

        public static byte[] GenerateSecretKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] key = new byte[32]; // 32 bytes = 256 bits
                rng.GetBytes(key);
                return key;
            }
        }
    }
}