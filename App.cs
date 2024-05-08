using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AIAssistant
{
    /// <summary>
    /// Provides a client for making requests to the OpenAI API.
    /// </summary>
    public class AIQueryClient
    {
        private readonly string _apiKey;
        private readonly string _model;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the AIQueryClient class.
        /// </summary>
        /// <param name="apiKey">API key for authentication with OpenAI.</param>
        /// <param name="model">Model identifier to use for generating text.</param>
        public AIQueryClient(string apiKey, string model)
        {
            _apiKey = apiKey;
            _model = model;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openai.com/v1/")
            };
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        /// <summary>
        /// Asynchronously generates text based on a given prompt and parameters.
        /// </summary>
        /// <param name="prompt">Input prompt to generate text for.</param>
        /// <param name="maxTokens">Maximum number of tokens to generate.</param>
        /// <param name="temperature">Randomness in output generation.</param>
        /// <param name="repeatCount">Number of times to repeat generation.</param>
        /// <param name="stopSequence">Sequence where the generation should stop.</param>
        /// <param name="isStreaming">Whether the API should stream back partial progress.</param>
        /// <param name="presencePenalty">Adjusts likelihood of new concepts appearing in the text.</param>
        /// <param name="bestOf">Generate several responses and return the best.</param>
        /// <param name="includeInput">Whether to include the prompt in the output.</param>
        /// <returns>The generated text.</returns>
        public async Task<string> GenerateTextAsync(string prompt, int maxTokens = 100, float temperature = 1.0f, int repeatCount = 1, string stopSequence = "", bool isStreaming = false, float presencePenalty = 0.0f, int bestOf = 1, bool includeInput = false)
        {
            var request = new TextGenerationRequest
            {
                Model = _model,
                Prompt = prompt,
                MaxTokens = maxTokens,
                Temperature = temperature,
                N = repeatCount,
                Stop = stopSequence,
                Stream = isStreaming,
                PresencePenalty = presencePenalty,
                BestOf = bestOf,
                Echo = includeInput
            };

            var response = await PostJsonAsync("engines/engine/generate", request);
            var textGenerationResponse = JsonConvert.DeserializeObject<TextGenerationResponse>(response);

            return textGenerationResponse.Choices[0].Text;
        }

        /// <summary>
        /// Helper method to post JSON data to a specified URL and return the response as a string.
        /// </summary>
        /// <param name="url">Endpoint URL to send the request to.</param>
        /// <param name="data">Data object to serialize to JSON and send in the request.</param>
        /// <returns>The response body as a string.</returns>
        private async Task<string> PostJsonAsync(string url, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }

    class TextGenerationRequest
    {
        public string Model { get; set; }
        public string Prompt { get; set; }
        public int MaxTokens { get; set; }
        public float Temperature { get; set; }
        public int N { get; set; }
        public string Stop { get; set; }
        public bool Stream { get; set; }
        public float PresencePenalty { get; set; }
        public int BestOf { get; set; }
        public bool Echo { get; set; }
    }

    class TextGenerationResponse
    {
        public Choice[] Choices { get; set; }
    }

    class Choice
    {
        public string Text { get; set; }
    }
}
