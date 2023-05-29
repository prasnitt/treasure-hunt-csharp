using TresurehuntApi.Data;
using TresurehuntApi.Model;

namespace TresurehuntApi.lib
{
    public static class AuthLib
    {
        public static string LoginCookie = "UserId";
        public static TimeSpan MaxLoggingTime = TimeSpan.FromMinutes(120);

        public static void SetCookie(HttpResponse response, string userId)
        {
            response.Cookies.Append(LoginCookie, userId, new CookieOptions { Expires = DateTimeOffset.Now.Add(MaxLoggingTime) });
        }

        public static User? GetLoggedInUser(HttpRequest request)
        {

            if (request.Cookies.TryGetValue(LoginCookie, out string userId))
            {
                return UserData.UserById(userId);
            }

            return null;
        }
    }
}
