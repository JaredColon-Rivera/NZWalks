using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models;
using NZWalks.UI.Models.DTO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace NZWalks.UI.Controllers
{
    public class RegionsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public RegionsController(IHttpClientFactory httpClientFactory) 
        {
            this.httpClientFactory = httpClientFactory;
        }


        public async Task<IActionResult> Index()
        {

            List<RegionDto> response = new List<RegionDto>();

            try
            {
                // Get all regions from web api
                var client = httpClientFactory.CreateClient();

                var httpResponseMessage = await client.GetAsync("https://localhost:7010/api/regions");

                httpResponseMessage.EnsureSuccessStatusCode();

                response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());

                
            }
            catch(Exception ex)
            {
                // Log the exception
            }

            return View(response);
        }

        [HttpGet]
        public IActionResult AddRegion()
        {
            return View();
        }

        public async Task<IActionResult> AddRegion(AddRegionViewModel model)
        {
            var client = httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7010/api/regions"),
                Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")

            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);

            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();

            if(response is not null)
            {
                return RedirectToAction("Index", "Regions");
            }

            return View();

        }

        [HttpGet]
        public async Task<IActionResult> EditRegion(Guid id)
        {
            var client = httpClientFactory.CreateClient();

            var response = await client.GetFromJsonAsync<RegionDto>($"https://localhost:7010/api/regions/{id.ToString()}");

            if(response is not null)
            {
                return View(response);
            }


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditRegion(RegionDto request)
        {
            var client = httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"https://localhost:7010/api/regions/{request.Id}"),
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")

            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);

            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();

            if(response is not null)
            {
                return RedirectToAction("EditRegion", "Regions");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRegion(RegionDto request)
        {

            try
            {
                var client = httpClientFactory.CreateClient();

                var httpResponseMessage = await client.DeleteAsync($"https://localhost:7010/api/regions/{request.Id}");

                httpResponseMessage.EnsureSuccessStatusCode();

                return RedirectToAction("Index", "Regions");
            }
            catch(Exception ex)
            {
                // Console 
            }
            
            return View("EditRegion");
        }

    }
}
