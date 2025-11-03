using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace StadiumSensor.Infrastructure.Services
{
    public abstract class ServiceBase
    {
        public readonly JsonSerializerSettings JsonOptions = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.Indented
        };

        protected async Task<ContentResult> CreateErrorResponseMessage(HttpStatusCode statusCode, string? errorMessage = null, string? details = null, string contentType = "application/json")
        {
            var code = (int)statusCode;
            var response = new ContentResult
            {
                StatusCode = code,
                ContentType = contentType
            };

            // We only need content when detail is present
            if (errorMessage == null) return response;

            var error = new ProblemDetails
            {
                Title = errorMessage,
                Status = code,
                Detail = details
            };

            var content = new StringContent(JsonConvert.SerializeObject(error, JsonOptions));
            response.Content = await content.ReadAsStringAsync();
            return response;
        }

        protected async Task<ContentResult> CreateResponse(HttpStatusCode statusCode, dynamic? obj = null)
        {
            var code = (int)statusCode;
            var response = new ContentResult
            {
                StatusCode = code,
                ContentType = "application/json"
            };

            // We only need content when detail is present
            if (obj == null) return response;
            var content = new StringContent(JsonConvert.SerializeObject(obj, JsonOptions));
            response.Content = await content.ReadAsStringAsync();
            return response;
        }
    }
}
