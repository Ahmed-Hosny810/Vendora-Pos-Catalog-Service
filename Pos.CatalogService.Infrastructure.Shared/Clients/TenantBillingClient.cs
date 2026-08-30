using Pos.CatalogService.Application.Features.Products.DTOS;
using Pos.CatalogService.Application.Interfaces.Clients;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Pos.CatalogService.Infrastructure.Shared.Clients
{
    public class TenantBillingClient : ITenantBillingClient
    {
        private readonly HttpClient _httpClient;

        private readonly ICurrentUserService _currentUserService;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public TenantBillingClient(HttpClient httpClient,ICurrentUserService currentUserService)
        {
            _httpClient = httpClient;
            _currentUserService = currentUserService;
        }
        public async Task<Result<IncreaseProductUsageResult>> IncreaseProductUsageAsync(ProductUsageRequest request, CancellationToken cancellationToken)
        {
            var accessToken = _currentUserService.AccessToken;

            if (string.IsNullOrWhiteSpace(accessToken))
                return Result<IncreaseProductUsageResult>.Failure("Access token is missing.");

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                "/api/v1/tenants/usage/increase-product");

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            httpRequest.Content = JsonContent.Create(request);

            try
            {
                var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                var apiResponse = DeserializeResponse<IncreaseProductUsageResult>(responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    var errors = ExtractErrors(
                        apiResponse,
                        $"Tenant Billing request failed. StatusCode: {(int)response.StatusCode}.");

                    return Result<IncreaseProductUsageResult>.Failure(errors);
                }

                if (apiResponse == null)
                    return Result<IncreaseProductUsageResult>.Failure("Tenant Billing returned an empty response.");

                if (!apiResponse.Succeeded)
                {
                    var errors = ExtractErrors(
                        apiResponse,
                        "Tenant Billing rejected product usage increase.");

                    return Result<IncreaseProductUsageResult>.Failure(errors);
                }

                if (apiResponse.Data == null)
                    return Result<IncreaseProductUsageResult>.Failure("Tenant Billing returned empty usage result.");

                return Result<IncreaseProductUsageResult>.Success(apiResponse.Data);
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return Result<IncreaseProductUsageResult>.Failure("Tenant Billing request timed out.");
            }
            catch (HttpRequestException ex)
            {
                return Result<IncreaseProductUsageResult>.Failure(
                    $"Tenant Billing service is unavailable: {ex.Message}");
            }
            catch (JsonException)
            {
                return Result<IncreaseProductUsageResult>.Failure(
                    "Tenant Billing returned an invalid response.");
            }
        }
        public async Task<Result<DecreaseProductUsageResult>> DecreaseProductUsageAsync(ProductUsageRequest request, CancellationToken cancellationToken)
        {
            var accessToken = _currentUserService.AccessToken;

            if (string.IsNullOrWhiteSpace(accessToken))
                return Result<DecreaseProductUsageResult>.Failure("Access token is missing.");

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                "/api/v1/tenants/usage/decrease-branch");

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            httpRequest.Content = JsonContent.Create(request);

            try
            {
                var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                var apiResponse = DeserializeResponse<DecreaseProductUsageResult>(responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    var errors = ExtractErrors(
                        apiResponse,
                        $"Tenant Billing request failed. StatusCode: {(int)response.StatusCode}.");

                    return Result<DecreaseProductUsageResult>.Failure(errors);
                }

                if (apiResponse == null)
                    return Result<DecreaseProductUsageResult>.Failure("Tenant Billing returned an empty response.");

                if (!apiResponse.Succeeded)
                {
                    var errors = ExtractErrors(
                        apiResponse,
                        "Tenant Billing rejected product usage decrease.");

                    return Result<DecreaseProductUsageResult>.Failure(errors);
                }

                if (apiResponse.Data == null)
                    return Result<DecreaseProductUsageResult>.Failure("Tenant Billing returned empty usage result.");

                return Result<DecreaseProductUsageResult>.Success(apiResponse.Data);
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return Result<DecreaseProductUsageResult>.Failure("Tenant Billing request timed out.");
            }
            catch (HttpRequestException ex)
            {
                return Result<DecreaseProductUsageResult>.Failure(
                    $"Tenant Billing service is unavailable: {ex.Message}");
            }
            catch (JsonException)
            {
                return Result<DecreaseProductUsageResult>.Failure(
                    "Tenant Billing returned an invalid response.");
            }
        }

        private static Response<T>? DeserializeResponse<T>(string responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
                return null;

            return JsonSerializer.Deserialize<Response<T>>(
                responseBody,
                JsonOptions);
        }

        private static string[] ExtractErrors<T>(
            Response<T>? response,
            string fallbackMessage)
        {
            if (response?.Errors != null && response.Errors.Any())
                return response.Errors.ToArray();

            if (!string.IsNullOrWhiteSpace(response?.Message))
                return new[] { response.Message };

            return new[] { fallbackMessage };
        }

    }
}
