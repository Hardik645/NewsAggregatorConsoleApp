using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Services;
using System.Net;
using System.Text.Json.Nodes;

namespace NewsAggregatorConsoleApp.Views.Pages
{
    public class SourcesDetailPage(PageSharedStorage pageSharedStorage) : IPage
    {
        private readonly List<AllSourceResponse> _servers = [];

        public async Task Render()
        {
            PageHelper.DisplayHeader();
            PageHelper.CenterText("External Servers Details\n");
            Console.WriteLine();

            ResponseMessage response = await SourceService.GetAllSources(pageSharedStorage);
            await ProcessSourceStatusResponse(response);

            if (_servers.Count == 0)
            {
                PageHelper.CenterText("No external servers found.");
            }
            else
            {
                PageHelper.CenterText(PageHelper.JoinWithSpacing(["Id","Name", "Api_URL", "__________________Api_Key__________________\n"], 100), color: ConsoleColor.Blue);
                PageHelper.DrawLine(max:100, lineSymbol: '-');
                Console.WriteLine();
                foreach (var server in _servers)
                {
                    PageHelper.CenterText(PageHelper.JoinWithSpacing([server.id.ToString(), server.Name, server.Url, $"{server.Key}\n"], 100));
                }
            }            

            Console.WriteLine();
            PageHelper.CenterText("Press any key to return...");
            Console.ReadKey();
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

        private static List<AllSourceResponse> ParseServers(JsonNode? data)
        {
            var result = new List<AllSourceResponse>();
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

                        result.Add(new AllSourceResponse
                        {
                            id = id,
                            Name = name,
                            Url = url,
                            Key = key
                        });
                    }
                }
            }
            return result;
        }
    }
}