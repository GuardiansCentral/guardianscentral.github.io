using Microsoft.Data.SqlClient;
using System.Text;
using Serilog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
public class UpdateActiveWeeklyRotators{
    public static void UpdateTable(PublicMilestonesResponse.RootObject PublicMilestonesObject, string Server, string Database, string UserId, string Password){
        // Create MS SQL Server connection
        string sqlConnectionString = $"Server={Server};Database={Database};TrustServerCertificate=True;Uid={UserId};Pwd={Password};";
        Log.Information(sqlConnectionString);
        string tableName = "ActiveWeeklyRotatorsTable";
        // Dictionary to store weekly rotator milestones
        Dictionary<string, PublicMilestonesResponse.Milestone> rawWeeklyRotatorsDictionary = new Dictionary<string, PublicMilestonesResponse.Milestone>();

        // List of banned hashes
        List<string> bannedHashes = new List<string> { "3690703860" };

        // Loops through all milestones and checks for the 3 Weekly Rotators and adds them to a list
        if (PublicMilestonesObject.Response != null) {
            foreach (KeyValuePair<string, PublicMilestonesResponse.Milestone> milestoneKVP in PublicMilestonesObject.Response){
                if (milestoneKVP.Value.Activities != null) {
                    foreach (PublicMilestonesResponse.Activity activity in milestoneKVP.Value.Activities){
                        if(activity.ChallengeObjectiveHashes != null){
                            if(activity.ChallengeObjectiveHashes.Count > 0){ 
                                // Check if the key exist already if it does do not add it. This will eliminate the Legend versions from the dict
                                if (!rawWeeklyRotatorsDictionary.ContainsKey(milestoneKVP.Key) && !bannedHashes.Contains(milestoneKVP.Key)) {
                                    // Create a new dictionary to hold the milestone
                                    rawWeeklyRotatorsDictionary.Add(milestoneKVP.Key, milestoneKVP.Value);
                                }
                            }
                        }
                    }
                }

            }
        } else {
            Log.Warning("PublicMilestonesObject.Response is null. Unable to iterate through milestones.");
            throw new ApplicationException("PublicMilestonesObject.Response is null. Unable to iterate through milestones.");
        }

        List<string> activeWeeklyRotatorsList = new List<string>(rawWeeklyRotatorsDictionary.Keys);

        // // //NODE.OVRD.AVALON// 87546222900
        // // Starcrossed 87546333822

        // if (activeWeeklyRotatorsList < 3) {
        //     Log.Information("The Active Weekly Rotator List returned less than 3 milestones")
        //     Log.Information("This most likely means a activity doesnt have a milestone and will need to be manually added to the table and this code will need to be updated with that information")
        //     // BUILD out the needed json object for the activeweeklyrotators table 
        //     // Then come here and figure out a way to determine which is the active mission

        //     list.add(missing milestone)


        // }

        // Convert dictionary to JSON string and log
        string dictionaryJson = JsonConvert.SerializeObject(rawWeeklyRotatorsDictionary, Formatting.Indented);
        Log.Information(dictionaryJson);
        Log.Information(string.Join(", ", activeWeeklyRotatorsList));
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
                bool doesNameExistsInColumn = NameExistsInColumn(connection:msSqlConnection, tableName:tableName, columnName:"NAME", nameToCheck:"ActiveWeeklyRotators");
                if(doesNameExistsInColumn == false){
                    string valueForNameColumn = "ActiveWeeklyRotators";
                    string weeklyRotatorList = "";
                    Log.Information($"{valueForNameColumn} name in column was not found. Will create the row now");
                    using (SqlCommand addEmptyRowCommand = new SqlCommand($"INSERT INTO {tableName} (NAME, RotatorList) VALUES (@NAME, @RotatorList)",msSqlConnection)){
                        addEmptyRowCommand.Parameters.AddWithValue("@NAME", valueForNameColumn);
                        addEmptyRowCommand.Parameters.AddWithValue("@RotatorList", weeklyRotatorList);
                        addEmptyRowCommand.ExecuteNonQuery();
                    }
                }
                using (SqlCommand updateRowCommand = new SqlCommand($"UPDATE {tableName} SET RotatorList = @RotatorList", msSqlConnection)){
                    string activeWeeklyRotatorsListString = "[" + string.Join(",", activeWeeklyRotatorsList) + "]";
                    updateRowCommand.Parameters.AddWithValue("@RotatorList", string.Join(",", activeWeeklyRotatorsListString));
                    updateRowCommand.ExecuteNonQuery();
                }
            }catch (Exception ex){
                Log.Error($"Error opening MySQL connection: {ex.Message}");
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

}