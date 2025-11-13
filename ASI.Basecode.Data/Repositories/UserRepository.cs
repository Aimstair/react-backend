using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System.Linq;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IEnumerable<User> GetUsers() 
        {
            return this.GetDbSet<User>().ToList();
        }

        public void AddUser(User user)
        {
            this.GetDbSet<User>().Add(user);
        }

        public void Delete(User user)
        {
            this.GetDbSet<User>().Remove(user);
        }
    }
}
