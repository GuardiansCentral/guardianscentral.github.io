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
        var configFile = await Task.Run(() => Toml.ReadFile(tomlFilePath));

        string LoggerPath = configFile.Get<string>("LoggerPath");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File($"{LoggerPath}Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Converts TOML File data to strings
        string Server = configFile.Get<string>("Server");
        string Database = configFile.Get<string>("Database");
        string UserId = configFile.Get<string>("UserId");
        string Password = configFile.Get<string>("Password");
        List <long> weeklyRotatorTableHashes = new List<long>();
        if (weeklyRotatorTableHashes != null && weeklyRotatorTableHashes.Any()){
            //This will probably need to be moved in the future since we are manually passing in values for the list
            WeeklyRotatorsTable.BuildWeeklyRotatorsTable(Server, Database, UserId, Password, weeklyRotatorTableHashes);
        }
        UpdateActiveWeeklyRotators.UpdateTable(Server, Database, UserId, Password);
        
        Log.CloseAndFlush();  
    }
}