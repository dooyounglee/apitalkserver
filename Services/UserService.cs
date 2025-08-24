using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rest1.Attibutes;
using rest1.Models;
using rest1.Repositories;

namespace rest1.Services
{
    public interface IUserService
    {
        public User login(string id, string pw);
        public List<User> getUserList();
        public User getUser(int usrNo);

        public User Me { get; }
        public void logout();
        public void save(User user);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private User _user;
        public User Me { get => _user; }

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Transaction]
        public User login(string id, string pw)
        {
            _user = _userRepository.login("user" + id, "");
            _user.Ip = "127.0.0.1";
            _user.Port = 8080;
            if (_user.UsrNo == 0) _user.IsAdmin = true;
            return _user;
        }

        [Transaction]
        public List<User> getUserList()
        {
            return _userRepository.getUserList();
        }

        [Transaction]
        public User getUser(int usrNo)
        {
            return _userRepository.findById(usrNo);
        }

        public void logout()
        {
            _user = null;
        }

        [Transaction]
        public void save(User user)
        {
            _userRepository.save(user);
        }
    }
}
