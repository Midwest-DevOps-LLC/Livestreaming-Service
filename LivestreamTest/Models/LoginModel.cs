using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LivestreamTest.Models
{
    public class LoginModel
    {
        public string Username
        {
            get; set;
        }

        public string Password
        {
            get; set;
        }

        public string Application
        {
            get; set;
        }
    }

    public class RegisterModel
    {
        public string Username
        {
            get; set;
        }

        public string Email
        {
            get; set;
        }

        public string Application
        {
            get; set;
        }

        public string Password
        {
            get; set;
        }

        public string RetypePassword
        {
            get; set;
        }
    }

    public class UpdatePasswordModel
    {
        public int UserID
        {
            get; set;
        }

        public string UUID
        {
            get; set;
        }

        public string Username
        {
            get; set;
        }

        public string Email
        {
            get; set;
        }

        public string Password
        {
            get; set;
        }

        public string RetypePassword
        {
            get; set;
        }
    }
}
