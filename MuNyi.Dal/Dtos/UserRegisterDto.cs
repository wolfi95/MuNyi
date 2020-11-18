using System;
using System.Collections.Generic;
using System.Text;

namespace MuNyi.Dal.Dto
{
    public class UserRegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string EmailRe { get; set; }
        public string PasswordRe { get; set; }
        public string Name { get; set; }
    }
}
