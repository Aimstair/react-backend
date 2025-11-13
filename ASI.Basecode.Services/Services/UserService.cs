using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository repository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public void AddUser(UserViewModel user)
        {
            user.Password = PasswordManager.EncryptPassword(user.Password);

            var userEntity = new Data.Models.User
            {
                UserId = user.UserId?.Trim() ?? throw new ArgumentException("UserId is required"),
                Password = user.Password?.Trim() ?? throw new ArgumentException("Password is required"),
                Name = $"{user.FirstName?.Trim()} {user.LastName?.Trim()}".Trim(),
                FirstName = user.FirstName?.Trim(),
                LastName = user.LastName?.Trim(),
                Role = user.Role?.Trim() ?? "user",
                Email = user.Email?.Trim() ?? "", 
                CreatedBy = "System",
                CreatedTime = DateTime.Now,
                UpdatedBy = "System",
                UpdatedTime = DateTime.Now
            };

            // Validate string lengths before saving
            if (userEntity.UserId.Length > 50)
                throw new ArgumentException("UserId too long (max 50 characters)");
            if (userEntity.Name.Length > 50)
                throw new ArgumentException("Name too long (max 50 characters)");

            _repository.AddUser(userEntity);
            _unitOfWork.SaveChanges();
        }

        public LoginResult AuthenticateUser(string userId, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.UserId == userId &&
                                                     x.Password == passwordKey).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        public void UpdateUser(int id, UserViewModel user)
        {
            try
            {
                var existingUser = _repository.GetUsers().FirstOrDefault(u => u.Id == id);
                if (existingUser != null)
                {
                    existingUser.UserId = user.UserId;
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Name = $"{user.FirstName} {user.LastName}".Trim();
                    existingUser.Role = user.Role;
                    existingUser.Email = user.Email; 
                    existingUser.UpdatedBy = "System";
                    existingUser.UpdatedTime = DateTime.Now;

                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}", ex);
            }
        }

        public void DeleteUser(int id)
        {
            try
            {
                var existingUser = _repository.GetUsers().FirstOrDefault(u => u.Id == id);
                if (existingUser != null)
                {
                    _repository.Delete(existingUser);
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user: {ex.Message}", ex);
            }
        }

        public IEnumerable<UserViewModel> GetUsersByRole(string role)
        {
            try
            {
                // Filter users by exact role match (case-sensitive for "user")
                var users = _repository.GetUsers().Where(x => x.Role != null && x.Role.Equals(role, StringComparison.OrdinalIgnoreCase));
                return users.Select(user => new UserViewModel
                {
                    Id = user.Id,
                    UserId = user.UserId,
                    Name = user.Name,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    Email = user.Email,
                    CreatedBy = user.CreatedBy,
                    CreatedTime = user.CreatedTime,
                    UpdatedBy = user.UpdatedBy,
                    UpdatedTime = user.UpdatedTime
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting users by role '{role}': {ex.Message}", ex);
            }
        }

        public IEnumerable<UserViewModel> GetAllUsers()
        {
            try
            {
                var users = _repository.GetUsers();
                return users.Select(user => new UserViewModel
                {
                    Id = user.Id,
                    UserId = user.UserId,
                    Name = user.Name,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    Email = user.Email,
                    CreatedBy = user.CreatedBy,
                    CreatedTime = user.CreatedTime,
                    UpdatedBy = user.UpdatedBy,
                    UpdatedTime = user.UpdatedTime
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all users: {ex.Message}", ex);
            }
        }

        public IEnumerable<UserViewModel> GetUsers()
        {
            var users = _repository.GetUsers();
            return users.Select(u => new UserViewModel
            {
                Id = u.Id,
                UserId = u.UserId,
                Name = u.Name,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role,
                CreatedBy = u.CreatedBy,
                CreatedTime = u.CreatedTime,
                UpdatedBy = u.UpdatedBy,
                UpdatedTime = u.UpdatedTime
            });
        }
    }
}