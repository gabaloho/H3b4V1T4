// Services/Api/IApiService.cs

// using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HerbavitaLogistics.Models;
using HerbavitaLogistics.Services.Api;
using HerbavitaLogistics.Services.LocalDataStore;
using HerbavitaLogistics.Services.NetworkMonitor;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System.Net.Http;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;
using System.Net.Http.Json;
public class ApiService : IApiService
{
    private readonly HttpClient _http;
    public ApiService(HttpClient http) => _http = http;

    public async Task<Employee> ValidateEmployeeAsync(string badgeId) =>
        await _http.GetFromJsonAsync<Employee>($"/api/employees/validate?badgeId={badgeId}");

    public async Task<ItemValidationResult> ValidateItemAsync(string barcode) =>
        await _http.GetFromJsonAsync<ItemValidationResult>($"/api/items/validate?barcode={barcode}");
}



public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://your-api.com";

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<AuthResult> AuthenticateAsync(string badgeId)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseUrl}/api/auth",
            new { BadgeId = badgeId });

        return await response.Content.ReadFromJsonAsync<AuthResult>();
    }

    public async Task<ItemValidationResult> ValidateItemAsync(string barcode)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseUrl}/api/items/validate",
            new { Barcode = barcode });

        return await response.Content.ReadFromJsonAsync<ItemValidationResult>();
    }
}