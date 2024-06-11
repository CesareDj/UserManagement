﻿using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public class UserService(UserManagementDbContext context) : IUserService
    {
        private readonly UserManagementDbContext _context = context;

        public Task<List<User>> GetUsersAsync()
        {
            return _context.Users.ToListAsync();
        }

        public Task<User?> GetUserAsync(int id)
        {
            return _context.Users.FindAsync(id).AsTask();
        }

        public async Task<User> CreateUserAsync(UserDto userDto)
        {
            Country country = _context.Countries.FirstOrDefault(c => c.Name == userDto.Country)
                              ?? _context.Countries.Add(new Country { Name = userDto.Country }).Entity;

            Company company = _context.Companies.FirstOrDefault(c => c.Name == userDto.Company)
                               ?? _context.Companies.Add(new Company { Name = userDto.Company }).Entity;

            User user = new()
            {
                Id = userDto.Id,
                Email = userDto.Email,
                First = userDto.First,
                Last = userDto.Last,
                CompanyId = company.Id,
                CreatedAt = userDto.CreatedAt ?? DateTime.UtcNow,
                CountryId = country.Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<List<User>> CreateUsersAsync(List<UserDto> userDtos)
        {
            List<User> users = [];

            foreach (UserDto userDto in userDtos)
            {
                Country country = _context.Countries.FirstOrDefault(c => c.Name == userDto.Country)
                                  ?? _context.Countries.Add(new Country { Name = userDto.Country }).Entity;

                Company company = _context.Companies.FirstOrDefault(c => c.Name == userDto.Company)
                                   ?? _context.Companies.Add(new Company { Name = userDto.Company }).Entity;

                users.Add(new User
                {
                    Id = userDto.Id,
                    Email = userDto.Email,
                    First = userDto.First,
                    Last = userDto.Last,
                    CompanyId = company.Id,
                    CreatedAt = userDto.CreatedAt ?? DateTime.UtcNow,
                    CountryId = country.Id
                });
            }

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            return users;
        }

        public async Task<User?> UpdateUserAsync(int id, User user)
        {
            if (id != user.Id)
            {
                return null;
            }

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> DeleteUserAsync(int id)
        {
            User user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}