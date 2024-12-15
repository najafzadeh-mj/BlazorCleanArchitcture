﻿using Application.Contracts;
using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IAccount account) : Controller
    {
        [HttpPost("identity/create")]
        public async Task<ActionResult<GeneralResponse>> CreateAccount(CreateAccountDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Model cannot be null");
            return Ok(await account.CreateAccountAsync(model));
        }

        [HttpPost("identity/login")]
        public async Task<ActionResult<GeneralResponse>> LoginAccount(LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Model cannot be null");
            return Ok(await account.LoginAccountAsync(model));
        }
        
        [HttpPost("identity/refresh-token")]
        public async Task<ActionResult<GeneralResponse>> RefreshToken(RefreshTokenDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Model cannot be null");
            return Ok(await account.RefreshTokenAsync(model));
        }

        [HttpGet("identity/role/list")]
        public async Task<IEnumerable<GetRoleDTO>> GetRole()
        =>   await account.GetRolesAsync();
        

        [HttpPost("identity/role/create")]
        public async Task<ActionResult<GeneralResponse>> CreateRole(CreateRoleDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Model cannot be null");
            return Ok(await account.CreateRoleAsync(model));
        }
        [HttpPost("/setting")]
        public async Task<ActionResult> CreateAdmin()
        {
            await account.CreateAdmin();
            return Ok();
        }


        [HttpGet("identity/users-with-roles")]
        public async Task<ActionResult<IEnumerable<GetusersWithRolesResponseDTO>>> GetUserWithRoles() 
            => Ok(await account.GetusersWithRolesAsync());

        [HttpPost("identity/change-role")]
        public async Task<ActionResult<GeneralResponse>> ChangeUserRole(ChangeUserRoleRequestDTO model)
           => Ok(await account.ChangeUserRoleAsync(model));
    }
}
