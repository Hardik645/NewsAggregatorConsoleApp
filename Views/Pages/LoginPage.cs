using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Services;
using System.Net;

namespace NewsAggregatorConsoleApp.Views.Pages
{
    public class LoginPage(PageSharedStorage pageManager) : IPage
    {
        public async Task Render()
        {
            PageHelper.DisplayHeader();
            PageHelper.CenterText("Login to your NewsAggregator Account\n");
            Console.WriteLine();

            var (email, password) = PromptUserCredentials();

            Console.WriteLine();
            ResponseMessage response = await AuthService.Login(email, password);

            await ProcessLoginResponse(response);
        }

        private static (string Email, string Password) PromptUserCredentials()
        {
            PageHelper.CenterText("Enter Email: ");
            string email = Console.ReadLine() ?? "";

            PageHelper.CenterText("Enter Password: ");
            string password = PasswordHelper.ReadPassword();

            return (email, password);
        }
        private async Task ProcessLoginResponse(ResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if(Guid.TryParse(response.Data?["userId"]?.ToString(), out Guid userID))
                {
                    pageManager.User.UserId = userID;
                }
                pageManager.User.Token = response.Data?["token"]?.ToString();
                pageManager.User.Role = response.Data?["role"]?.ToString();

                pageManager.IsAuthenticated = true;
                await PageHelper.ShowSuccessToast("Login successful! Proceeding to Home Page...", 2000);
            }
            else
            {
                pageManager.IsAuthenticated = false;
                await PageHelper.ShowErrorToast($"Login failed: {response.Message}", 3000);
            }
        }
    }
}
