using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.PaymentMethod;

namespace BookStorePOS.MvcApp.Controllers;

public class PaymentMethodController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PaymentMethodController> _logger;

    public PaymentMethodController(IHttpClientFactory httpClientFactory, ILogger<PaymentMethodController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [ActionName("Index")]
    public async Task<IActionResult> IndexAsync()
    {
        try
        {
            _logger.LogInformation("Mvc PaymentMethod => Fetching payment methods");
            var client = _httpClientFactory.CreateClient("WebAPI");
            var httpResponse = await client.GetAsync("api/paymentmethod");
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<PaymentMethodListResponseModel>(jsonString);

            if (response != null && response.IsSuccess && response.Data != null)
            {
                ViewData["PaymentMethods"] = response.Data;
            }
            else
            {
                _logger.LogWarning("Mvc PaymentMethod => Failed to fetch payment methods: {Message}", response?.Message);
                TempData["Message"] = response?.Message ?? "Failed to load payment methods.";
                TempData["isSuccess"] = false;
            }

            return View("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mvc PaymentMethod => Exception in IndexAsync");
            TempData["Message"] = "Failed to load payment methods: " + ex.Message;
            TempData["isSuccess"] = false;
            return View("Index");
        }
    }

    [HttpPost]
    [ActionName("ToggleStatus")]
    public async Task<IActionResult> ToggleStatusAsync(int id, [FromBody] PaymentMethodToggleRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Mvc PaymentMethod => Toggling status for #{Id} to {IsActive}", id, requestModel.IsActive);
            var client = _httpClientFactory.CreateClient("WebAPI");
            var json = JsonConvert.SerializeObject(requestModel);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await client.PatchAsync($"api/paymentmethod/{id}/toggle", content);
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<PaymentMethodToggleResponseModel>(jsonString)
                           ?? new PaymentMethodToggleResponseModel { IsSuccess = false, Message = "Unknown response from server." };

            return Json(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mvc PaymentMethod => Exception in ToggleStatusAsync");
            return Json(new PaymentMethodToggleResponseModel { IsSuccess = false, Message = "Server Error: " + ex.Message });
        }
    }

    [HttpPost]
    [ActionName("UpdateSave")]
    public async Task<IActionResult> UpdateSaveAsync(int id, [FromBody] PaymentMethodPatchRequestModel requestModel)
    {
        try
        {
            _logger.LogInformation("Mvc PaymentMethod => Updating payment method #{Id}", id);
            requestModel.PaymentMethodId = id;
            var client = _httpClientFactory.CreateClient("WebAPI");
            var json = JsonConvert.SerializeObject(requestModel);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await client.PatchAsync($"api/paymentmethod/{id}", content);
            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<PaymentMethodPatchResponseModel>(jsonString)
                           ?? new PaymentMethodPatchResponseModel { IsSuccess = false, Message = "Unknown response from server." };

            return Json(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mvc PaymentMethod => Exception in UpdateSaveAsync");
            return Json(new PaymentMethodPatchResponseModel { IsSuccess = false, Message = "Server Error: " + ex.Message });
        }
    }
}
