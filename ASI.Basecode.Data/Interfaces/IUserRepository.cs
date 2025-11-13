using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        void AddUser(User user);

        void Delete(User user);
    }
}
