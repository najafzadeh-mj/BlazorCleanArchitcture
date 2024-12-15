using Application.Contracts;
using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Application.Extensions;
using Domain.Entity.Authentication;
using Infrastructure.Data;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class AccountRepository
        (RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager, IConfiguration config,
        SignInManager<ApplicationUser> signInManager, AppDbContext context) : IAccount
    {

        private async Task<ApplicationUser> FindUserByEmailAsync(string email)
            => await userManager.FindByEmailAsync(email);
        private async Task<IdentityRole> FindRoleByNameAsync(string roleName)
            => await roleManager.FindByNameAsync(roleName);

        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));


        private async Task<string> GeneratrToken(ApplicationUser user)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var userClaim = new[]
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, (await userManager.GetRolesAsync(user)).FirstOrDefault().ToString()),
                    new Claim("FullName",user.Name)
                };

                var token = new JwtSecurityToken(

                    issuer: config["Jwt:Issuer"],
                    audience: config["Jwt:Audience"],
                    claims: userClaim,
                    signingCredentials: credentials,
                    expires: DateTime.Now.AddMinutes(30));
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch { return null!; }
        }

        private async Task<GeneralResponse> AssignUserToRole(ApplicationUser user, IdentityRole role)
        {
            if (user is null || role is null) return new GeneralResponse(false, "Model State cannot be  empty");

            if (await FindRoleByNameAsync(role.Name) == null)
                await CreateRoleAsync(role.Adapt(new CreateRoleDTO()));

            IdentityResult result = await userManager.AddToRoleAsync(user, role.Name);
            string error = CheckResponse(result);

            if (!string.IsNullOrEmpty(error))
                return new GeneralResponse(false, error);
            else
                return new GeneralResponse(true, $"{user.Name} assigned to {role.Name} role");
        }

        private static string CheckResponse(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(_ => _.Description);
                return string.Join(Environment.NewLine, errors);
            }
            return null!;
        }

        public async Task CreateAdmin()
        {
            try
            {
                if (await FindUserByEmailAsync("najafzadeh.mj@gmail.com") != null) return;
                if ((await FindRoleByNameAsync(Constant.Role.Admin)) == null)
                {
                    await CreateRoleAsync(new CreateRoleDTO()
                    {
                        Name = "Admin"
                    });
                }
                //if ((await FindRoleByNameAsync(Constant.Role.Admin)) != null) return;
                var admin = new CreateAccountDTO()
                {
                    Name = "Admin",
                    Password = "Admin@1234",
                    EmailAddress = "najafzadeh.mj@gmail.com",
                    Role = Constant.Role.Admin
                };
                await CreateAccountAsync(admin);
            }
            catch { }
        }

        public async Task<GeneralResponse> CreateAccountAsync(CreateAccountDTO model)
        {
            try
            {
                if (await FindUserByEmailAsync(model.EmailAddress) != null)
                    return new GeneralResponse(false, "Sorry, user is already exist");

                var user = new ApplicationUser()
                {
                    Name = model.Name,
                    UserName = model.EmailAddress,
                    Email = model.EmailAddress,
                    PasswordHash = model.Password
                };

                var result = await userManager.CreateAsync(user, model.Password);
                string error = CheckResponse(result);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponse(false, error);

                var (flag, message) = await AssignUserToRole(user, new IdentityRole() { Name = model.Role });
                return new GeneralResponse(flag, message);
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, ex.Message);
            }
        }

        public async Task<LoginResponse> LoginAccountAsync(LoginDTO model)
        {
            try
            {
                var user = await FindUserByEmailAsync(model.EmailAddress);
                if (user is null)
                    return new LoginResponse(false, "User Not Found!");

                SignInResult result;
                try
                {
                    result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                }
                catch
                {
                    return new LoginResponse(false, "Invalid Credential");
                }
                if (!result.Succeeded)
                    return new LoginResponse(false, "Invalid Credential!");

                string jwtToken = await GeneratrToken(user);
                string refreshToken = GenerateRefreshToken();
                if (string.IsNullOrEmpty(jwtToken) || string.IsNullOrEmpty(refreshToken))
                    return new LoginResponse(false, "Error ocured while logging in account, please contract administrator");
                else
                {
                    var saveResult = await SaveRefreshToken(user.Id, refreshToken);
                    if (saveResult.Flag)
                        return new LoginResponse(true, $"{user.Name} successfully logged in", jwtToken, refreshToken);
                    else
                        return new LoginResponse();
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse(false, ex.Message);
            }
        }

        public async Task<GeneralResponse> CreateRoleAsync(CreateRoleDTO model)
        {
            try
            {
                if (await roleManager.FindByNameAsync(model.Name) == null)
                {
                    var response = await roleManager.CreateAsync(new IdentityRole(model.Name));
                    var error = CheckResponse(response);
                    if (!string.IsNullOrEmpty(error))
                        throw new Exception(error);
                    else
                        return new GeneralResponse(true, $"Role {model.Name} Created");
                }
                return new GeneralResponse(false, $"Role {model.Name} Exist");
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }


        }

        public async Task<IEnumerable<GetRoleDTO>> GetRolesAsync()
            => (await roleManager.Roles.ToListAsync()).Adapt<IEnumerable<GetRoleDTO>>();

        public async Task<IEnumerable<GetusersWithRolesResponseDTO>> GetusersWithRolesAsync()
        {
            var allUsers = await userManager.Users.ToListAsync();
            if (allUsers is null) return null!;

            var list = new List<GetusersWithRolesResponseDTO>();
            foreach (var item in allUsers)
            {
                var getUserRole = (await userManager.GetRolesAsync(item)).FirstOrDefault();
                var getRoleInfo = await roleManager.Roles.FirstOrDefaultAsync(m => m.Name.ToLower() == getUserRole.ToLower());
                list.Add(new GetusersWithRolesResponseDTO()
                {
                    Name = item.Name,
                    Email = item.Email,
                    RoleId = getRoleInfo.Id,
                    RoleName = getRoleInfo.Name
                });
            }
            return list;
        }

        public async Task<GeneralResponse> ChangeUserRoleAsync(ChangeUserRoleRequestDTO model)
        {
            if (await FindRoleByNameAsync(model.RoleName) is null) return new GeneralResponse(false, "Role not Found");
            if (await FindUserByEmailAsync(model.UserEmail) is null) return new GeneralResponse(false, "User not Found");

            var user = await FindUserByEmailAsync(model.UserEmail);
            var previousRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
            var removeOldRole = await userManager.RemoveFromRoleAsync(user, previousRole);

            var error = CheckResponse(removeOldRole);
            if (!string.IsNullOrEmpty(error)) return new GeneralResponse(false, error);

            var result = await userManager.AddToRoleAsync(user, model.RoleName);
            var response = CheckResponse(result);
            if (!string.IsNullOrEmpty(response)) return new GeneralResponse(false, response);
            else return new GeneralResponse(true, "Role Changed");

        }

        private async Task<GeneralResponse> SaveRefreshToken(string userId, string token)
        {
            try
            {
                var user = await context.RefreshTokens.FirstOrDefaultAsync(m => m.UserId == userId);
                if (user == null)
                    context.RefreshTokens.Add(new RefreshToken() { UserId = userId, Token = token });
                else
                    user.Token = token;

                await context.SaveChangesAsync();
                return new GeneralResponse(true, null!);
            }
            catch (Exception ex) { return new GeneralResponse(false, ex.Message); }
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenDTO model)
        {
            var token = await context.RefreshTokens.FirstOrDefaultAsync(m => m.Token == model.Token);
            if (token == null) return new LoginResponse();

            var user = await userManager.FindByIdAsync(token.UserId);
            string newtoken = await GeneratrToken(user);
            string newRefreshToken = GenerateRefreshToken();

            var saveResult = await SaveRefreshToken(user.Id, newRefreshToken);

            if (saveResult.Flag)
                return new LoginResponse(true, $"{user.Name} succefuly re-logged in", newtoken, newRefreshToken);
            else
                return new LoginResponse();
        }
    }
}
