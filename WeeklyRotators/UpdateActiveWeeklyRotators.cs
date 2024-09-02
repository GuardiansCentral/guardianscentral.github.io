using Microsoft.Data.SqlClient;
using System.Text;
using Serilog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class Activity{
    // Property for the type of activity
    public string? ActivityType { get; set; }

    // Property for the hash associated with the activity
    public string? Hash { get; set; }

    // Property for the sequence number
    public int? Sequence { get; set; }

    public string? Name { get; set; }
    
    // Constructor to initialize the properties
    public Activity(string activityType, string hash, int sequence, string name){
        ActivityType = activityType;
        Hash = hash;
        Sequence = sequence;
        Name = name;
    }
    
}

public class RotatorEntry{
    public string? RotatorType { get; set; }
    public int? MaxSequence { get; set; }
}

public class UpdateActiveWeeklyRotators{
    public static void UpdateTable(string Server, string Database, string UserId, string Password){
        // Create MS SQL Server connection
        string sqlConnectionString = $"Server={Server};Database={Database};TrustServerCertificate=True;Uid={UserId};Pwd={Password};";
        Log.Information(sqlConnectionString);
        string tableName = "ActiveWeeklyRotatorsTable";
 
        using (SqlConnection msSqlConnection = new SqlConnection(sqlConnectionString)){
            try{
                msSqlConnection.Open();
                bool doesTableExist = TableExists(connection:msSqlConnection, tableName:tableName);
                if(doesTableExist == false){
                    Log.Information($"{tableName} not found");
                    using (SqlCommand createTableCommand = new SqlCommand($"CREATE TABLE {tableName} (NAME VARCHAR(MAX) , RotatorList VARCHAR(MAX))", msSqlConnection)) {
                        createTableCommand.ExecuteNonQuery();
                    }
                }
                // If the ActiveWeeklyRotators does not exist it will set the table to the start of the rotator sequence
                bool doesNameExistsInColumn = NameExistsInColumn(connection:msSqlConnection, tableName:tableName, columnName:"NAME", nameToCheck:"ActiveWeeklyRotators");
                if(doesNameExistsInColumn == false){
                    string valueForNameColumn = "ActiveWeeklyRotators";
                    Log.Information($"{valueForNameColumn} name in column was not found. Will run ResetAndPopulateWeeklyRotatorsTable method");
                    ResetAndPopulateWeeklyRotatorsTable(msSqlConnection:msSqlConnection, tableName:tableName, valueForNameColumn:valueForNameColumn);
                }else if(doesNameExistsInColumn == true){
                    Log.Information("The ActiveWeeklyRotatorsTableExist and it is not empty....Attempting to set new rotators");
                    string valueForNameColumn = "ActiveWeeklyRotators";
                    string valueForRotatorListColumn = "RotatorList";
                    string? currentActiveWeeklyRotatorsString = "";

                    // This Query gets the Active Weekly Rotators String from SQL Server and sets it to a variable
                    using (SqlCommand findActiveWeeklyRotatorsQuery = new SqlCommand($"SELECT {valueForRotatorListColumn} FROM {tableName} WHERE NAME = @NameValue", msSqlConnection)){
                        findActiveWeeklyRotatorsQuery.Parameters.AddWithValue("@NameValue", valueForNameColumn);
                        using (SqlDataReader reader = findActiveWeeklyRotatorsQuery.ExecuteReader()){
                            if (reader.Read()){
                                currentActiveWeeklyRotatorsString = reader[valueForRotatorListColumn].ToString();
                            }
                        }
                    }
                    Log.Information("Got the Active Weekly Rotators String");

                    // This will set a default weekly rotators table and end UpdateActiveWeeklyRotatorsMethod if there is something wrong with the table
                    if(currentActiveWeeklyRotatorsString == null || currentActiveWeeklyRotatorsString == ""){
                        Log.Information("The Active Weekly Rotators String is either empty or null");
                        ResetAndPopulateWeeklyRotatorsTable(msSqlConnection:msSqlConnection, tableName:tableName, valueForNameColumn:valueForNameColumn);
                        return; 
                    }

                    // Remove Square brackets from string
                    string trimmedCurrentActiveWeeklyRotatorsString = currentActiveWeeklyRotatorsString.Trim('[', ']');
                    // Split the string by commas
                    string[] currentActiveWeeklyRotatorsStringArray = trimmedCurrentActiveWeeklyRotatorsString.Split(',');
                    // Convert the string array to a list of integers
                    List<long> currentActiveWeeklyRotatorsList = currentActiveWeeklyRotatorsStringArray
                        .Select(s => long.Parse(s.Trim())) // Parse string to long
                        .ToList();
                    
                    int? currentActivitySequence = null;
                    string? currentActivityRotatorType = "";
                    List<Activity> newActiveWeeklyRotatorsList = new List<Activity>();
                    List<RotatorEntry> rotatorEntries = GetMaxSequenceByRotatorType(msSqlConnection:msSqlConnection, tableName:"RotatorSchedule", rotatorTypeColumnName:"RotatorType");
                    // Loops through all the hashes in the current rotator list and returns the new active weekly rotator list
                    Log.Information("Looping through the currentActiveWeeklyRotatorsList");
                    foreach(long hash in currentActiveWeeklyRotatorsList){
                        // Query to get current Activity Sequence and RotatorType
                        using (SqlCommand findCurrentRotatorQuery = new SqlCommand($"SELECT Sequence,RotatorType,Hash FROM RotatorSchedule WHERE Hash = @Hash",msSqlConnection)) {
                            findCurrentRotatorQuery.Parameters.AddWithValue("@Hash", hash);
                            using (SqlDataReader reader = findCurrentRotatorQuery.ExecuteReader()){
                                if (reader.Read()){
                                    currentActivitySequence = Convert.ToInt32(reader["Sequence"]);
                                    currentActivityRotatorType = reader["RotatorType"].ToString();
                                }
                            }
                        }

                        // checks if rotatortype found is at the end of the sequence. If it is..it resets sequence to 1 else it adds 1 to sequence
                        int? newActivitySequence = null;
                        foreach (var entry in rotatorEntries){
                            if(entry.RotatorType == currentActivityRotatorType){
                                if(currentActivitySequence >= entry.MaxSequence){
                                    newActivitySequence = 1;
                                }else{
                                    newActivitySequence = currentActivitySequence + 1;
                                }
                            }
                        }

                        // Builds the Object Activity that is then placed into the AcitveWeeklyRotatorsList
                        using(SqlCommand findNewRotatorQuery = new SqlCommand($"SELECT Hash, RotatorType, Sequence, Name FROM RotatorSchedule WHERE Sequence = @NewActivitySequence AND RotatorType = @NewActivityRotatorType",msSqlConnection)){
                            findNewRotatorQuery.Parameters.AddWithValue("@NewActivitySequence", newActivitySequence);
                            findNewRotatorQuery.Parameters.AddWithValue("@NewActivityRotatorType", currentActivityRotatorType);
                            using (SqlDataReader reader = findNewRotatorQuery.ExecuteReader()) {
                                while (reader.Read()){
                                    Activity newActivity = new Activity(
                                        activityType: reader["RotatorType"].ToString(),
                                        hash: reader["Hash"].ToString(),
                                        sequence: Convert.ToInt32(reader["Sequence"]),
                                        name: reader["Name"].ToString()
                                    );
                                    newActiveWeeklyRotatorsList.Add(newActivity);
                                }
                            }
                        }
                    }

                    bool raid = false;
                    bool dungeon = false;
                    bool exoticMission = false;

                    foreach (Activity activity in newActiveWeeklyRotatorsList){
                        if (activity.ActivityType == "Raid"){
                            raid = true;
                        } else if(activity.ActivityType == "Dungeon"){
                            dungeon = true;
                        } else if(activity.ActivityType == "Exotic Mission"){
                            exoticMission = true;
                        }
                    }

                    string newActiveWeeklyRotatorsString = "";
                    if (raid == true && dungeon == true && exoticMission == true){
                        Log.Information("A new raid, dungeon, and exotic mission was found. Updating Active Weekly Rotators Table");
                        // Collect hashes into a list
                        var hashes = newActiveWeeklyRotatorsList
                            .Select(activity => activity.Hash) // Select hashes
                            .ToList(); // Convert to list

                        // Join hashes with commas and format with square brackets
                        newActiveWeeklyRotatorsString = "[" + string.Join(",", hashes) + "]";

                        using (SqlCommand updateRowCommand = new SqlCommand($"UPDATE {tableName} SET RotatorList = @RotatorList", msSqlConnection)){
                            updateRowCommand.Parameters.AddWithValue("@RotatorList", string.Join(",", newActiveWeeklyRotatorsString));
                            updateRowCommand.ExecuteNonQuery();
                        }
                    }else{
                        Log.Error("The code could not find one of the previous Raids, Dungeon or ExoticMission");
                        ResetAndPopulateWeeklyRotatorsTable(msSqlConnection:msSqlConnection, tableName:tableName, valueForNameColumn:valueForNameColumn);
                    }
                }
            }catch(Exception ex){
                Log.Error(ex.Message);
            }
        }

  
    }

    private static bool TableExists(SqlConnection connection, string tableName) {
        using (SqlCommand command = new SqlCommand($"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'", connection)) {
            return command.ExecuteScalar() != null;
        }
    }
    private static bool NameExistsInColumn(SqlConnection connection, string tableName, string columnName, string nameToCheck){
        string sqlQuery = $"SELECT 1 FROM {tableName} WHERE {columnName} = @NameToCheck";
        using (SqlCommand command = new SqlCommand(sqlQuery, connection)){
            command.Parameters.AddWithValue("@NameToCheck", nameToCheck);
            return command.ExecuteScalar() != null;
        }
    }
    private static void ResetAndPopulateWeeklyRotatorsTable(SqlConnection msSqlConnection, string tableName, string valueForNameColumn){
        Log.Information("Dropping Active Weekly Rotators Table");
        using (SqlCommand deleteTableCommand = new SqlCommand($"DROP TABLE {tableName}", msSqlConnection)){
            deleteTableCommand.ExecuteNonQuery();
        }

        Log.Information("Creating Active Weekly Rotators Table");
        using (SqlCommand createTableCommand = new SqlCommand($"CREATE TABLE {tableName} (NAME VARCHAR(MAX), RotatorList VARCHAR(MAX))", msSqlConnection)){
            createTableCommand.ExecuteNonQuery();
        }

        List<long> defaultWeeklyRotatorList = new List<long>{
            2122313384,
            2823159265,
            2668737148
        };

        string defaultWeeklyRotatorListString = "[" + string.Join(",", defaultWeeklyRotatorList) + "]";
        Log.Information("The Active Weekly Rotators Table was found but it's either empty or missing rotators or one the next sequence number was not found. Setting default.");

        using (SqlCommand addDefaultRowCommand = new SqlCommand($"INSERT INTO {tableName} (NAME, RotatorList) VALUES (@NAME, @RotatorList)", msSqlConnection)){
            addDefaultRowCommand.Parameters.AddWithValue("@NAME", valueForNameColumn);
            addDefaultRowCommand.Parameters.AddWithValue("@RotatorList", defaultWeeklyRotatorListString);
            addDefaultRowCommand.ExecuteNonQuery();
        }
    }

    private static List<RotatorEntry> GetMaxSequenceByRotatorType(SqlConnection msSqlConnection, string tableName, string rotatorTypeColumnName){
        var result = new List<RotatorEntry>();
        using (SqlCommand getMaxSequenceByRotatorTypeQuery = new SqlCommand($"SELECT {rotatorTypeColumnName}, MAX(Sequence) AS MaxSequence FROM {tableName} GROUP BY {rotatorTypeColumnName}",msSqlConnection)){
            using (SqlDataReader reader = getMaxSequenceByRotatorTypeQuery.ExecuteReader()){
                while (reader.Read()){
                    result.Add(new RotatorEntry{
                        RotatorType = reader[rotatorTypeColumnName].ToString(),
                        MaxSequence = Convert.ToInt32(reader["MaxSequence"])
                    });
                }
            }
        }

        return result;
    }


}

//Figure out what to do when you cant add 1 anymore to the sequence like for exaple there is only 8 raids what happens when you add 1 to 8. It will most likely fail. See what the error is and catch it. 
//Add more logging to see what code is doing. 