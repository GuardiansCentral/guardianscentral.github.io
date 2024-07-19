using Newtonsoft.Json;

public class Tokenizer{
    public static async Task<(AccessToken accessToken, RefreshToken refreshToken)> GetAccessToken(string? ClientID, string? ClientSecret, string? code){

        using (HttpClient httpClient = new HttpClient()){
            try{
                if (ClientID == null || ClientSecret == null || code == null){
                    throw new Exception("Failed to parse JSON response.");
                }

                var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>{
                    { "grant_type", "authorization_code" },
                    { "client_id", ClientID},
                    { "client_secret", ClientSecret},
                    { "code", code}
                });

                var response = await httpClient.PostAsync("https://www.bungie.net/Platform/App/OAuth/token/", requestContent);

                if (response.IsSuccessStatusCode){
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseContentDictionary = ParseTokenResponse(responseContent);
                    (AccessToken accessToken, RefreshToken refreshToken) = SetAccessToken(responseContentDictionary);
                    return (accessToken, refreshToken);

                }
                else{
                    throw new Exception($"Failed to get access token: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException ex){
                Console.WriteLine($"Error: {ex.Message}");
                return (new AccessToken(), new RefreshToken());
            }
        }
    }

    private static Dictionary<string, string> ParseTokenResponse(string responseContent){

        // Parse the JSON response to extract the access token, refresh token, and expiration time
        // For simplicity, assuming the response is a JSON object with fields "access_token", "refresh_token", and "expires_in"
        // You might need to use a JSON parsing library like Newtonsoft.Json for proper parsing
        // Example parsing logic (replace with actual parsing)

        // Parse the JSON string into a dictionary
        Dictionary<string, string>? acessTokenDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

        if (acessTokenDict == null){
            // Throw an exception if JSON parsing fails
            throw new Exception("Failed to parse JSON response.");
        }

        return acessTokenDict;
    }

    private static (AccessToken accessToken, RefreshToken refreshToken) SetAccessToken(Dictionary<string, string> accessTokenDict){
        AccessToken accessToken = new AccessToken{
            Token = accessTokenDict["access_token"],
            TokenType = accessTokenDict["token_type"],
            ExpiresIn = int.Parse(accessTokenDict["expires_in"]),

        };
        accessToken.Expiration = DateTime.UtcNow.AddSeconds(accessToken.ExpiresIn);


        RefreshToken refreshToken = new RefreshToken{
            Token = accessTokenDict["refresh_token"],
            TokenType = accessTokenDict["token_type"],
            ExpiresIn = int.Parse(accessTokenDict["refresh_expires_in"]),
        };
        refreshToken.Expiration = DateTime.UtcNow.AddSeconds(refreshToken.ExpiresIn);

        return (accessToken, refreshToken);
    }

    public static bool IsTokenValid(DateTime accessTokenExpiresAt){
        if (DateTime.UtcNow >= accessTokenExpiresAt){
            return false;
        }
        return true;
    }

    public static async Task<(AccessToken accessToken, RefreshToken refreshToken)> RefreshAccessToken(string? ClientID, string? ClientSecret, string? RefreshToken){

        using (HttpClient httpClient = new HttpClient()){
            try{

                if (ClientID == null || ClientSecret == null || RefreshToken == null){
                    throw new Exception("Failed to parse JSON response.");
                }

                var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>{
                    { "grant_type", "refresh_token" },
                    { "client_id", ClientID },
                    { "client_secret", ClientSecret },
                    { "refresh_token", RefreshToken }

                });

                var response = await httpClient.PostAsync("https://www.bungie.net/Platform/App/OAuth/token/", requestContent);

                if (response.IsSuccessStatusCode){
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                    var responseContentDictionary = ParseTokenResponse(responseContent);
                    (AccessToken accessToken, RefreshToken refreshToken) = SetAccessToken(responseContentDictionary);
                    return (accessToken, refreshToken);
                }
                else{
                    throw new Exception($"Failed to get access token: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException ex){
                Console.WriteLine($"Error: {ex.Message}");
                return (new AccessToken(), new RefreshToken());
            }
        }
    }

    public class AccessToken{
        public string? Token { get; set; }
        public string? TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string? MembershipId { get; set; }
        public DateTime Expiration { get; set; }

    }

    public class RefreshToken{
        public string? Token { get; set; }
        public string? TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string? MembershipId { get; set; }
        public DateTime Expiration { get; set; }
    }
}
