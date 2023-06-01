﻿namespace TreasurehuntApi.Model
{
    public class User
    {
        public Guid Id { get; set; }

        public string? UserName { get; set; }

        public string? FullName { get; set; }
        public string? Password { get; set; }

        public string? Role { get; set; }
    }


    public class UserLoginRequest
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
