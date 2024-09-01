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
                    List<long> defaultWeeklyRotatorList = new List<long>;
                    defaultWeeklyRotatorList.Add(1441982566);
                    defaultWeeklyRotatorList.Add(4078656646);
                    defaultWeeklyRotatorList.Add(509188661);
                    string defaultWeeklyRotatorListString = "[" + string.Join(",", defaultWeeklyRotatorList) + "]";
                    Log.Information($"{valueForNameColumn} name in column was not found. Will create the row now and set it to the default rotators");
                    using (SqlCommand addEmptyRowCommand = new SqlCommand($"INSERT INTO {tableName} (NAME, RotatorList) VALUES (@NAME, @RotatorList)",msSqlConnection)){
                        addEmptyRowCommand.Parameters.AddWithValue("@NAME", valueForNameColumn);
                        addEmptyRowCommand.Parameters.AddWithValue("@RotatorList", string.Join(",", defaultWeeklyRotatorListString));
                        addEmptyRowCommand.ExecuteNonQuery();
                    }
                }
            }catch(Exception ex){
                Log.Error(ex.Message);
            }
        }


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