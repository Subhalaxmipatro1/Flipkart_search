using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Flipkart_search.Model
{
    public static class FlipkartClient
    {
        public static async Task<string> SearchAsync(string query)
        {
          
            try
            {
                using (var client = new HttpClient())
                {
                    var url = "https://1.rome.api.flipkart.com/api/4/page/fetch";

                    
                    string jsonBody = $@"
                    {{
                        ""pageUri"": ""/search?q={query}&otracker=search&otracker1=search&marketplace=FLIPKART&as-show=on&as=off"",
                        ""pageContext"": {{
                            ""fetchSeoData"": true,
                            ""paginatedFetch"": false,
                            ""pageNumber"": 1
                        }},
                        ""requestContext"": {{
                            ""type"": ""BROWSE_PAGE"",
                            ""ssid"": ""3mlpptnrxc0000001762328909214"",
                            ""sqid"": ""n9kovbchgw0000001762331407849""
                        }}
                    }}";

                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                
                    client.DefaultRequestHeaders.Clear();

                    // Add headers
                    client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                    client.DefaultRequestHeaders.Add("X-User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/141.0.0.0 Safari/537.36 FKUA/website/42/website/Desktop");
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/141.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"141\", \"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"141\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Add("Origin", "https://www.flipkart.com");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-site");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                    client.DefaultRequestHeaders.Add("Referer", "https://www.flipkart.com/");
                    //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                    client.DefaultRequestHeaders.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");

                    
                    // Send POST request
                    var response = await client.PostAsync(url, content);

                    response.EnsureSuccessStatusCode();

                    // Read response body
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            catch (HttpRequestException ex)
            {
                return $"HTTP Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
        }
    }
}


//public static class FlipkartClient
//{
//    private static readonly HttpClient client = new HttpClient();

//    public static async Task<string> SearchAsync(string query)
//    {

//        string url = "https://www.flipkart.com/search?q=smartphone&otracker=search&otracker1=search&marketplace=FLIPKART&as-show=on&as=off&as-pos=1&as-type=HISTORY ";


//        string body = $@"
//        {{
//            ""pageUri"": ""/search?q={query}"",
//            ""pageContext"": {{
//                ""paginated"": true
//            }}
//        }}";


//        var content = new StringContent(body, Encoding.UTF8, "application/json");


//        client.DefaultRequestHeaders.Clear();
//        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
//        client.DefaultRequestHeaders.Add("Accept", "application/json");
//        client.DefaultRequestHeaders.Add("Origin", "https://www.flipkart.com");
//        client.DefaultRequestHeaders.Add("Referer", "https://www.flipkart.com/");


//        var response = await client.PostAsync(url, content);


//        response.EnsureSuccessStatusCode();


//        return await response.Content.ReadAsStringAsync();
//    }
//}