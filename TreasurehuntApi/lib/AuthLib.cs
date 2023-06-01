using TreasurehuntApi.Data;
using TreasurehuntApi.Model;

namespace TreasurehuntApi.lib
{
    public static class AuthLib
    {
        public static string LoginCookie = "UserId";
        public static TimeSpan MaxLoggingTime = TimeSpan.FromMinutes(120);

        public static void SetCookie(HttpResponse response, string userId, int maxUserSessionInMinutes = 0)
        {
            var timeSpan = MaxLoggingTime;
            if (maxUserSessionInMinutes != 0)
                timeSpan = TimeSpan.FromMinutes(maxUserSessionInMinutes);

            response.Cookies.Append(LoginCookie, userId, new CookieOptions { Expires = DateTimeOffset.Now.Add(timeSpan) });
        }

        public static User? GetLoggedInUser(HttpRequest request, string? expectedRole = null)
        {
            if (request.Cookies.TryGetValue(LoginCookie, out string? userId))
            {
                var user =  UserData.UserById(userId);

                if (user != null && expectedRole != null && user.Role != expectedRole)
                {
                    return null;
                }

                return user;
            }

            return null;
        }
    }
}
