using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Presentation;
using Domain.Models;
using HttpClientFactory;
using Microsoft.Extensions.Configuration;
using Persistence.Http.Interfaces;
using Refit;

namespace Persistence.Http
{
    public class ViaCep : IViaCep
    {

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };


        public async Task<List<CepResponse>> GetAddress(List<string> cep)
        {
            List<CepResponse> CepResponse = new List<CepResponse>();

            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://viacep.com.br/ws/");

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            foreach(var c in cep)
            {
                var response = client.GetAsync($"{c}/json/").Result;
                var content = await JsonSerializer.DeserializeAsync<CepResponse>(response.Content.ReadAsStream(), _jsonOptions);

                content.Cep.Replace("-", string.Empty);
                CepResponse.Add(content);
            }

            return CepResponse;
        }
    }
}