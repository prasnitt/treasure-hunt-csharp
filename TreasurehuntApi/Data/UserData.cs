using TreasurehuntApi.Model;

namespace TreasurehuntApi.Data
{
    public static class UserData
    {
        public static class UserRoles
        {
            public const string SuperAdmin = "SuperAdmin";

            public const string TeamA = "TeamA";
            public const string TeamB = "TeamB";

        }
        static private List<User> API_USERS = new List<User>()
        {
            new User{
                FullName = "Prashant",
                Id = Guid.NewGuid(),

                Username = "prasnitt",
                Password = "Test123",
                Role = UserRoles.SuperAdmin,
            },

            new User{
                FullName = "Team A",
                Id = Guid.NewGuid(),
                Username = "teama",
                Password = "testA",
                Role = UserRoles.TeamA,
            },

            new User{
                FullName = "Team B",
                Id = Guid.NewGuid(),
                Username = "teamb",
                Password = "testB",
                Role = UserRoles.TeamB,
            }

        };


        public static User? ValidateUser(UserLoginRequest request)
        {
            return API_USERS.FirstOrDefault(u => u.Username == request.Username && u.Password == request.Password);
        }

        public static User? UserById(string userId)
        {
            return API_USERS.FirstOrDefault(u => u.Id.ToString() == userId);
        }
    }
}
