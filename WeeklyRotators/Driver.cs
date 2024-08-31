using Nett;
using Serilog;
using Newtonsoft.Json;

class Program{
    static async Task Main(string[] args){        
        // Check if the command-line argument for the TOML file path is provided
        if (args.Length < 1){
            Console.WriteLine("Please provide a config path");
            return;
        }

        // Get the TOML file path from the command-line argument
        string tomlFilePath = args[0];

        // Check if the TOML file exists
        if (!File.Exists(tomlFilePath)){
            Console.WriteLine($"Error: The specified config file '{tomlFilePath}' does not exist.");
            return;
        }

        // Gets all the data from the TOML file
        var configFile = Toml.ReadFile(tomlFilePath);

        string LoggerPath = configFile.Get<string>("LoggerPath");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File($"{LoggerPath}Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Converts TOML File data to strings
        string BungieRootPath = configFile.Get<string>("BungieRootPath");
        string BungieApiRootPath = configFile.Get<string>("BungieAPIRootPath");
        string GetPublicMilestoneEndpoint = configFile.Get<string>("GetPublicMilestoneEndpoint");
        string GuardiansCentralApiKey = configFile.Get<string>("GuardiansCentralApiKey");
        string Server = configFile.Get<string>("Server");
        string Database = configFile.Get<string>("Database");
        string UserId = configFile.Get<string>("UserId");
        string Password = configFile.Get<string>("Password");


        PublicMilestonesResponse.RootObject PublicMilestonesObject = await BungieApiRequests.GetPublicMilestonesRequest(BungieRootPath, GetPublicMilestoneEndpoint, GuardiansCentralApiKey);
        Log.Information(JsonConvert.SerializeObject(PublicMilestonesObject));
        List <long> weeklyRotatorTableHashes = new List<long>();
        weeklyRotatorTableHashes.Add(2122313384);
        weeklyRotatorTableHashes.Add(1042180643);
        weeklyRotatorTableHashes.Add(910380154);
        weeklyRotatorTableHashes.Add(3881495763);
        weeklyRotatorTableHashes.Add(1441982566);
        weeklyRotatorTableHashes.Add(1374392663);
        weeklyRotatorTableHashes.Add(2381413764);
        weeklyRotatorTableHashes.Add(107319834);
        weeklyRotatorTableHashes.Add(2823159265);
        weeklyRotatorTableHashes.Add(1262462921);
        weeklyRotatorTableHashes.Add(2582501063);
        weeklyRotatorTableHashes.Add(2032534090);
        weeklyRotatorTableHashes.Add(1077850348);
        weeklyRotatorTableHashes.Add(4078656646);
        weeklyRotatorTableHashes.Add(313828469);
        weeklyRotatorTableHashes.Add(2668737148);
        weeklyRotatorTableHashes.Add(1221538367);
        weeklyRotatorTableHashes.Add(509188661);
        weeklyRotatorTableHashes.Add(196691221);
        weeklyRotatorTableHashes.Add(3883295757);
        WeeklyRotatorsTable.BuildWeeklyRotatorsTable(PublicMilestonesObject, Server, Database, UserId, Password, weeklyRotatorTableHashes);
        //UpdateActiveWeeklyRotators.UpdateTable(PublicMilestonesObject, Server, Database, UserId, Password);


    

        Log.CloseAndFlush();  
    }
}