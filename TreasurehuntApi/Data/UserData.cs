using TreasurehuntApi.Model;

namespace TreasurehuntApi.Data
{
    public static class UserData
    {
        // Note this must be unique
        public const string TeamAName = "TeamA";
        public const string TeamBName = "TeamB";


        public static class UserRoles
        {
            public const string SuperAdmin = "SuperAdmin";
            public const string Team = "Team";
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
                FullName = TeamAName,
                Id = Guid.NewGuid(),
                Username = "teama",
                Password = "testA",
                Role = UserRoles.Team,
            },

            new User{
                FullName = TeamBName,
                Id = Guid.NewGuid(),
                Username = "teamb",
                Password = "testB",
                Role = UserRoles.Team,
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
