using Core.Interfaces;
using JwtAuthenticationManager.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthenticationManager
{
    public class JwtTokenHandler
    {
        public const string JWT_SECURITY_KEY = "PQxkc9Y4xHGvFKLqWCAnfmJ23wJ-b9ucKxBA-UylF8WVN8XEcdh8Udz0jTTnbPB-BpbEfVzQkqJ4YPmn2ZPElA";
        private const int JWT_TOKEN_VALIDITY_MINS = 20;
        private readonly IUnitOfWork _unitOfWork;


        public JwtTokenHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<AuthenticationResponse?> GenerateJwtTokenAsync(AuthenticationRequest authenticationRequest)
        {
            // Validação básica dos campos de entrada
            if (string.IsNullOrWhiteSpace(authenticationRequest.UserName) || string.IsNullOrWhiteSpace(authenticationRequest.Password) )
                return null;

            // Valida o nome de usuário e a senha, e obtém o usuário
            var user = await _unitOfWork.UserRepository.ValidateUserAsync(authenticationRequest.UserName, authenticationRequest.Password);
            if (user == null)
            {
                return null; // Credenciais inválidas
            }

            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, authenticationRequest.UserName),
            });

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                NotBefore = DateTime.UtcNow,
                SigningCredentials = signingCredentials
            };
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return new AuthenticationResponse
            {
                Username = authenticationRequest.UserName,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds,
                JwtToken = token,
            };
        }
    }
}
