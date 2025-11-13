using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        LoginResult AuthenticateUser(string userid, string password, ref User user);

        void AddUser(UserViewModel user);
        IEnumerable<UserViewModel> GetUsers();

        void UpdateUser(int id, UserViewModel user);
        void DeleteUser(int id);

        IEnumerable<UserViewModel> GetUsersByRole(string role);
        IEnumerable<UserViewModel> GetAllUsers();
    }
}
