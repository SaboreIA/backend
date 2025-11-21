using Microsoft.IdentityModel.Tokens;
using SaboreIA.DTOs;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Interfaces.Service;
using SaboreIA.Models;
using SaboreIA.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SaboreIA.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerDto)
        {
            // Verificar se email já existe
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
            {
                throw new InvalidOperationException("Email already registered.");
            }

            // Criar usuário
            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Phone = registerDto.Phone,
                Role = UserRole.USER,
                ImageUrl = registerDto.ImageUrl,
                Address = registerDto.Address == null ? null : new Address
                {
                    Street = registerDto.Address.Street,
                    Number = registerDto.Address.Number,
                    Complement = registerDto.Address.Complement,
                    City = registerDto.Address.City,
                    State = registerDto.Address.State,
                    ZipCode = registerDto.Address.ZipCode,
                    Country = registerDto.Address.Country
                }
            };

            var createdUser = await _userRepository.CreateAsync(user);

            // Gerar token
            return GenerateAuthResponse(createdUser);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginDto)
        {
            // Buscar usuário
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            if (user == null || !user.Active)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            // Verificar senha
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            // Gerar token
            return GenerateAuthResponse(user);
        }

        private AuthResponseDto GenerateAuthResponse(User user)
        {
            var token = GenerateJwtToken(user);
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTimeHelper.Now.AddMinutes(expirationMinutes),
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    ImageUrl = user.ImageUrl,
                    Role = user.Role,
                    Active = user.Active,
                    CreatedAt = DateTimeHelper.ToSaoPauloTime(user.CreatedAt),
                    Address = user.Address == null ? null : new AddressDto
                    {
                        Id = user.Address.Id,
                        ZipCode = user.Address.ZipCode,
                        Street = user.Address.Street,
                        Number = user.Address.Number,
                        Complement = user.Address.Complement,
                        City = user.Address.City,
                        State = user.Address.State,
                        Country = user.Address.Country
                    }
                }
            };
        }


        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTimeHelper.ToUtc(DateTimeHelper.Now.AddMinutes(expirationMinutes)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
