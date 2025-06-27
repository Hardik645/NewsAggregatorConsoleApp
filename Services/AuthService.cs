using NewsAggregatorConsoleApp.Models;
using System.Net;

namespace NewsAggregatorConsoleApp.Services
{
    public class AuthService : ApiService
    {
        public static async Task<ResponseMessage> Login(string email, string password)
        {
            try
            {
                ResponseMessage res = await SendRequest(() => new ()
                {
                    Url = "/auth/login",
                    Body = new { Email = email, Password = password },
                    Method = HttpMethod.Post
                });
                return res;
            }
            catch (Exception ex)
            {
                return new() { Message = $"Error during login: {ex.Message}", StatusCode=HttpStatusCode.BadRequest };
            }
        }

        public static async Task<ResponseMessage> SignUp(string username, string email, string password)
        {
            try
            {
                ResponseMessage res = await SendRequest(() => new ()
                {
                    Url = "/auth/signup",
                    Body = new {Username = username, Email = email, Password = password },
                    Method = HttpMethod.Post
                });
                return res;
            }
            catch (Exception ex)
            {
                return new() { Message = $"Error during signup: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }
    }
}
