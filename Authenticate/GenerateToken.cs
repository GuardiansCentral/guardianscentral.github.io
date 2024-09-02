using Nett;

class Program{
    static async Task Main(){

        // Gets all the data from TOML File 
        var table = Toml.ReadFile<Config>("config.toml");

        string? ClientID = table.ClientID;
        string? ClientSecret = table.ClientSecret;

        try{

            string? Code = table.Code;
            string? AccessToken = table.AccessToken;
            // No value for Access token is not in toml file
            if (string.IsNullOrEmpty(AccessToken)){
                var (accessToken, refreshToken) = await Tokenizer.GetAccessToken(ClientID, ClientSecret, Code);

                Console.WriteLine("Access Token: " + accessToken.Token);
                Console.WriteLine("Refresh Token: " + refreshToken.Token);
                // Create a TomlObject with the access token string value
                //var accessTokenTomlObject = Toml.Create<string?>(accessToken.Token);

                //var config = Toml.ReadFile<Config>("config.toml");
                table.AccessToken = accessToken.Token;
                table.AccessTokenExpiresAt = accessToken.Expiration;

                table.RefreshToken = refreshToken.Token;
                table.RefreshTokenExpiresAt = refreshToken.Expiration;

                // Console.WriteLine(accessTokenTomlObject);

                // Update the AccessToken value in the TOML table
                //table["AccessToken"] = accessTokenTomlObject;
                //table["AccessToken"] = accessToken.Token;
                Toml.WriteFile(table, "config.toml");
            }
            else{
                DateTime AccessTokenExpiresAt = table.AccessTokenExpiresAt;

                string? RefreshToken = table.RefreshToken;

                if (Tokenizer.IsTokenValid(AccessTokenExpiresAt) == false){
                    var (accessToken, refreshToken) = await Tokenizer.RefreshAccessToken(ClientID, ClientSecret, RefreshToken);

                    table.AccessToken = accessToken.Token;
                    table.AccessTokenExpiresAt = accessToken.Expiration;

                    table.RefreshToken = refreshToken.Token;
                    table.RefreshTokenExpiresAt = refreshToken.Expiration;

                    Toml.WriteFile(table, "config.toml");
                }
            }
        }
        catch (Exception ex){
            // Handle exceptions
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}

class Config{
    public string? APIKey { get; set; }
    public string? ClientID { get; set; }
    public string? ClientSecret { get; set; }
    public string? Code { get; set; }
    public string? AccessToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }

}