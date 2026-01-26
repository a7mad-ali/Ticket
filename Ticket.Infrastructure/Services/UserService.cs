using System;
using System.Threading.Tasks;
using Ticket.Domain.Contracts.Interfaces.IRepository;

namespace Ticket.Infrastructure.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    }
}
