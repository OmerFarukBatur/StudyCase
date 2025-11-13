using Core.DTOs;
using Core.DTOs.UserDtos;
using Core.Entities;
using Core.Interfaces.IRepositories;
using Core.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public AuthService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<ResponseMessageDto> UserCreateAsync(UserCreateDto userCreateDto)
        {
            var exists = await _userRepository.Table.AnyAsync(x => x.Email == userCreateDto.Email);
            if (exists)
                return new ResponseMessageDto
                {
                    Status = false,
                    Message = "Bu e-posta adresi zaten kayıtlı."
                };

            var employeeRole = await _roleRepository.Table.FirstOrDefaultAsync(r => r.Name == "Employee");
            if (employeeRole == null)
                return new ResponseMessageDto
                {
                    Status = false,
                    Message = "Sistem hatası: 'Employee' rolü bulunamadı."
                };

            string passwordHash = ComputeSha256Hash(userCreateDto.Password);

            var user = new User
            {
                FirstName = userCreateDto.FirstName,
                LastName = userCreateDto.LastName,
                Email = userCreateDto.Email,
                Username = userCreateDto.Email.Split('@')[0],
                PasswordHash = passwordHash,
                RoleId = employeeRole.Id,
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();

            return new ResponseMessageDto
            {
                Status = true,
                Message = "Kullanıcı başarıyla oluşturuldu."
            };
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using var sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            foreach (var t in bytes)
                builder.Append(t.ToString("x2"));
            return builder.ToString();
        }

        public async Task<Tuple<ResponseMessageDto, AuthenticationDto?>> Login(LoginDto loginDto)
        {
            string passwordHash = ComputeSha256Hash(loginDto.Password);

            var user = await _userRepository.Table.Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == loginDto.Email && x.PasswordHash == passwordHash);

            if (user == null)
                return new Tuple<ResponseMessageDto, AuthenticationDto?>(new ResponseMessageDto
                {
                    Status = false,
                    Message = "Geçersiz e-posta veya şifre."
                }, null);

            if (!user.IsActive)
                return new Tuple<ResponseMessageDto, AuthenticationDto?>(new ResponseMessageDto
                {
                    Status = false,
                    Message = "Kullanıcı hesabı pasif durumda."
                }, null);

            return new Tuple<ResponseMessageDto, AuthenticationDto?>(new ResponseMessageDto
            {
                Status = true,
                Message = $"Hoş geldin {user.FullName}"
            },
            new AuthenticationDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.Name,
                IsActive = user.IsActive,
                Username = user.Username
            });
        }
        public async Task<ResponseMessageDto> PasswordReset(PasswordResetDto passwordReset)
        {
            var user = await _userRepository.Table
                .FirstOrDefaultAsync(x => x.Id == passwordReset.Id);

            if (user == null)
                return new ResponseMessageDto
                {
                    Status = false,
                    Message = "Bu e-posta adresi ile kayıtlı kullanıcı bulunamadı."
                };

            string newPasswordHash = ComputeSha256Hash(passwordReset.Password);
            user.PasswordHash = newPasswordHash;

            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return new ResponseMessageDto
            {
                Status = true,
                Message = "Şifreniz başarıyla güncellendi."
            };
        }
    }
}
