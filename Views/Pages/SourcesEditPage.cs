using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Services;
using System.Net;
using System.Text.Json.Nodes;

namespace NewsAggregatorConsoleApp.Views.Pages
{
    public class SourcesEditPage(PageSharedStorage pageSharedStorage) : IPage
    {
        private readonly List<(int Id, string Name, string Url, string Key)> _servers = [];

        public async Task Render()  
        {
            PageHelper.DisplayHeader();
            PageHelper.CenterText("Update/Edit the external servers details\n");
            Console.WriteLine();

            ResponseMessage response = await SourceService.GetAllSources(pageSharedStorage);
            await ProcessSourceStatusResponse(response);

            if (_servers.Count == 0)
            {
                PageHelper.CenterText("No external servers found.");
            }
            else
            {
                PageHelper.CenterText("Enter the external server ID to edit (or press Enter to cancel): ");
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int serverId))
                {
                    var server = _servers.FirstOrDefault(s => s.Id == serverId);
                    if (server != default)
                    {
                        await EditServer(server);
                    }
                    else
                    {
                        await PageHelper.ShowErrorToast("Server ID not found.", 2000);
                    }
                }
            }

            Console.WriteLine();
            PageHelper.CenterText("Press any key to return...");
            Console.ReadKey();
        }

        private async Task EditServer((int Id, string Name, string Url, string Key) server)
        {
            Console.WriteLine();
            PageHelper.CenterText($"Editing Server: {server.Name} (ID: {server.Id})\n");

            Console.Write("New Name (leave blank to keep current): ");
            string? newName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newName)) newName = server.Name;

            Console.Write("New URL (leave blank to keep current): ");
            string? newUrl = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newUrl)) newUrl = server.Url;

            Console.Write("New Key (leave blank to keep current): ");
            string? newKey = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newKey)) newKey = server.Key;

            var updateRequest = new
            {
                apiName = newName,
                baseUrl = newUrl,
                apiKey = newKey,
            };

            var updateResponse = await SourceService.UpdateSource(pageSharedStorage, server.Id, updateRequest);
            if (updateResponse.StatusCode == HttpStatusCode.OK)
            {
                await PageHelper.ShowSuccessToast("Server updated successfully.", 2000);
            }
            else
            {
                await PageHelper.ShowErrorToast("Failed to update server.", 2000);
            }
        }

        private async Task ProcessSourceStatusResponse(ResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                _servers.Clear();
                _servers.AddRange(ParseServers(response.Data));
            }
            else
            {
                await PageHelper.ShowErrorToast($"Unable to fetch the data", 3000);
            }
        }

        private static List<(int Id, string Name, string Url, string Key)> ParseServers(JsonNode? data)
        {
            var result = new List<(int Id, string Name, string Url, string Key)>();
            if (data is JsonArray array)
            {
                foreach (var item in array)
                {
                    if (item is JsonObject obj)
                    {
                        int id = obj["id"]?.GetValue<int>() ?? 0;
                        string name = obj["name"]?.ToString() ?? "";
                        string apiUrl = obj["apiUrl"]?.ToString() ?? "";
                        string[] urlParts = apiUrl.Split('/');
                        string url = urlParts.Length > 2 ? urlParts[2] : "";
                        string key = obj["apiKey"]?.ToString() ?? "";
                        result.Add((id, name, url, key));
                    }
                }
            }
            return result;
        }
    }
}