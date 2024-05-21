using Microsoft.Data.SqlClient;
using System.Text;
using Serilog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class MilestoneDefinitionJson{
    public class DisplayProperties{
        public string? description { get; set; }
        public string? name { get; set; }
        public string? icon { get; set; }
        public bool? hasIcon { get; set; }
    }

    public class Activity{
        public long? activityHash { get; set; }
    }

    public class Root{
        public DisplayProperties? displayProperties { get; set; }
        public string? friendlyName { get; set; }
        public Activity[]? activities { get; set; }
        public long? hash { get; set; }
        public long? index { get; set; }
        public bool? redacted { get; set; }
        public bool? blacklisted { get; set; }
    }
}

public class CollectibleDefinitionJson{
    public class DisplayProperties{
        public string? description { get; set; }
        public string? name { get; set; }
        public string? icon { get; set; }
        public bool? hasIcon { get; set; }
    }
    public class Root{
        public DisplayProperties? displayProperties { get; set; }
        public long itemHash { get; set; }
    }
}

public class ActivityDefinitionJson{
    public class OriginalDisplayProperties{
        public string? description { get; set; }
        public string? name { get; set; }
        public string? icon { get; set; }
        public bool? hasIcon { get; set; }
    }

    public class Root{
        public OriginalDisplayProperties? originalDisplayProperties { get; set; }
        public string? pgcrImage { get; set; }
        public long? hash { get; set; }
        public int? activityTypeHash { get; set; }    
    }

}

public class ActivityTypeDefinitionJson{
    public class DisplayProperties{
        public string? name { get; set; }
    }
    
    public class Root{
        public DisplayProperties? displayProperties { get; set; }
    }
}

public class InventoryItemDefinitionJson{
    public class DisplayProperties{
        public string? description { get; set; }
        public string? name { get; set; }
        public string? icon { get; set; }
        public bool? hasIcon { get; set; }
    }

    public class Inventory{
        public long? tierTypeHash { get; set; }
        public string? tierTypeName { get; set; }
    }
    public class SocketEntries{
        public long? socketTypeHash { get; set; }
        public long? singleInitialItemHash { get; set; }
    }
    public class Sockets{
        public SocketEntries[]? socketEntries { get; set; }
    }

    public class Stat{
        public long? statHash { get; set; }
        public int? value { get; set; }
        public int? minimum { get; set; }
        public int? maximum { get; set; }
        public int? displayMaximum { get; set; }
    }
    public class Stats{
        public Dictionary<long, Stat>? stats { get; set; }
    }
    public class Root{
        public DisplayProperties? displayProperties { get; set; }
        public long[]? itemCategoryHashes { get; set; }
        public long[]? damageTypeHashes { get; set; }
        public string? screenshot { get; set; }
        public string? secondarySpecial { get; set; }
        public string? secondaryIcon { get; set; }
        public string? itemTypeDisplayName { get; set; }
        public string? flavorText { get; set; }
        public string? itemTypeAndTierDisplayName { get; set; }
        public Inventory? inventory { get; set; }
        public Sockets? sockets { get; set; }
        public Stats? stats { get; set; }
    }
}

// C# Mapping for all damage types in destiny 2
public class DamageTypeMapping{
    public Dictionary<int, (string Icon, string Name)> Mapping { get; }

    public DamageTypeMapping(){
        Mapping = new Dictionary<int, (string Icon, string Name)>{
            { unchecked((int)3373582085), ("/common/destiny2_content/icons/DestinyDamageTypeDefinition_3385a924fd3ccb92c343ade19f19a370.png", "Kinetic") },
            { unchecked((int)1847026933), ("/common/destiny2_content/icons/DestinyDamageTypeDefinition_2a1773e10968f2d088b97c22b22bba9e.png", "Solar") },
            { unchecked((int)2303181850), ("/common/destiny2_content/icons/DestinyDamageTypeDefinition_092d066688b879c807c3b460afdd61e6.png", "Arc") },
            { unchecked((int)3454344768), ("/common/destiny2_content/icons/DestinyDamageTypeDefinition_ceb2f6197dccf3958bb31cc783eb97a0.png", "Void") },
            { unchecked((int)151347233), ("/common/destiny2_content/icons/DestinyDamageTypeDefinition_530c4c3e7981dc2aefd24fd3293482bf.png", "Stasis") },
            { unchecked((int)3949783978), ("/common/destiny2_content/icons/DestinyDamageTypeDefinition_b2fe51a94f3533f97079dfa0d27a4096.png", "Strand") }
        };
    }
}

// This contains list of inventory item hashes for each rotator
public class WeeklyRotatorsMapping{
    public Dictionary<string, List<long>> Mapping { get; }
    public WeeklyRotatorsMapping(){
        Mapping = new Dictionary<string, List<long>>{
            { "Pit of Heresy", new List<long> 
                { 
                    1395261499,2782847179,208088207,2164448701,3067821200,2813078109,1699964364,175015316,2345799798,1343302889,
                    1496857121,2293199928,3434445392,328467570,2786161293,1721938300,3118392309,4235863403,399547095,2975563522 
                }
            },
            { "The Shattered Throne", new List<long> 
                { 
                    814876684,2465372924,3723679465,355922321,1874424704,2140635451,250721843,1472713738,1478378067,2561756285,788771493,4023744176,2804026582,
                    4008120231,3368092113,3185383401,844097260,1076538039,150052158,757360370,569434520,1394177923
                } 
            },
            { "Prophecy", new List<long> 
                { 
                } 
            },
            { "Grasp of Avarice", new List<long> 
                { 
                } 
            },
            { "Ghost of the Deep", new List<long> 
                { 
                } 
            },
            { "Duality", new List<long> 
                { 
                } 
            },
            { "Spire of the Watcher", new List<long> 
                { 
                } 
            },
            { "Vox Obscura", new List<long> 
                { 
                    46125926,2097055732,4067556514,1248372789,232928045,4096943616,1572896086,3189860891,2240729575,187431790,438108034,
                    2270509928,3957071315,2516879931,4017738218,4144240158,3056950148,3473867303,849255710,119228495,3465627817,693728753,950963812 
                } 
            },
            { "Operation Seraph's Shield", new List<long> 
                { 
                    1473821207,1751893422,3849444474,2978226043,1731355324,1168625549,2302346155,2149683300,4186079026,2272041093,3103325054,1059446290,1525902715,
                    839296981,4270910189,4256213288,648266032,446049665,4116381015,1940895219,1949733158,2974824815,1259550198,2710316218,2471829328,1934312075
                }
            },
            { "Presage", new List<long> 
                { 

                }
            }
        };
    }
}

// Main Class that builds Weekly Rotators Table
public class WeeklyRotatorsTable{
    public static void BuildWeeklyRotatorsTable(PublicMilestonesResponse.RootObject PublicMilestonesObject, string Server, string Database, string UserId, string Password){
        // Create SQL Server connection
        string sqlConnectionString = $"Server={Server};Database={Database};TrustServerCertificate=True;Uid={UserId};Pwd={Password};";
        Log.Information(sqlConnectionString);
        string tableName = "WeeklyRotatorsTable";
        // Dictionary to store weekly rotator milestones
        Dictionary<string, PublicMilestonesResponse.Milestone> rawWeeklyRotatorsDictionary = new Dictionary<string, PublicMilestonesResponse.Milestone>();

        // Loops through all milestones and checks for the 3 Weekly Rotators and adds them to a list
        if (PublicMilestonesObject.Response != null) {
            foreach (KeyValuePair<string, PublicMilestonesResponse.Milestone> milestoneKVP in PublicMilestonesObject.Response){
                if (milestoneKVP.Value.Activities != null) {
                    foreach (PublicMilestonesResponse.Activity activity in milestoneKVP.Value.Activities){
                        if(activity.ChallengeObjectiveHashes != null){
                            if(activity.ChallengeObjectiveHashes.Count > 0){ 
                                // Check if the key exist already if it does do not add it. This will eliminate the Legend versions from the dict
                                if (!rawWeeklyRotatorsDictionary.ContainsKey(milestoneKVP.Key)){
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


        long? activityId = null;
        long? activityTypeId = null;
        string? activityName = null;
        long? milestoneHash = null;
        long? milestoneHashInTable = null;
        List<Dictionary<string, object>> weapons = new List<Dictionary<string, object>>();
        List<Dictionary<string, object>> cosmetics = new List<Dictionary<string, object>>();
        List<Dictionary<string, object>> catalysts = new List<Dictionary<string, object>>();
        List<Dictionary<string, object>> hunterArmor = new List<Dictionary<string, object>>();
        List<Dictionary<string, object>> warlockArmor = new List<Dictionary<string, object>>();
        List<Dictionary<string, object>> titanArmor = new List<Dictionary<string, object>>();
        using (SqlConnection msSqlConnection = new SqlConnection(sqlConnectionString)){
            try{
                msSqlConnection.Open();
                bool doesTableExist = TableExists(connection:msSqlConnection, tableName:tableName);
                if(doesTableExist == false){
                    Log.Information($"{tableName} not found");
                    using (SqlCommand createTableCommand = new SqlCommand($"CREATE TABLE {tableName} (MilestoneHash BIGINT, Json VARCHAR(MAX))", msSqlConnection)) {
                        createTableCommand.ExecuteNonQuery();
                    }
                }
                // Empty dictionary to add table information to
                Dictionary<string, object> weeklyRotatorTableJson = new Dictionary<string, object>();
                // Loops through found weekly rotators
                foreach(KeyValuePair<string, PublicMilestonesResponse.Milestone> weeklyRotatorKVP in rawWeeklyRotatorsDictionary){
                    weapons.Clear();
                    hunterArmor.Clear();
                    warlockArmor.Clear();
                    titanArmor.Clear();
                    cosmetics.Clear();
                    catalysts.Clear();
                    weeklyRotatorTableJson.Clear();
                    var milestoneId = unchecked((int) weeklyRotatorKVP.Value.MilestoneHash);
                    // Query to find weekly rotator milestone by Id
                    using (SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM DestinyMilestoneDefinition Where Id = {milestoneId}", msSqlConnection)){
                        using (SqlDataReader reader = sqlCommand.ExecuteReader()){
                            while (reader.Read()){
                                // Log the response using Serilog
                                Log.Information("SQL Query Response: {@Response}", new{
                                    LogId = reader["Id"],
                                    LogJson = reader["json"].ToString(),
                                });
                                string? milestoneDefinitionJson = reader["json"].ToString();
                                MilestoneDefinitionJson.Root? milestoneDefinitionRoot = null;
                                if (milestoneDefinitionJson != null) {
                                    milestoneDefinitionRoot = JsonConvert.DeserializeObject<MilestoneDefinitionJson.Root>(milestoneDefinitionJson);
                                } else {
                                    Log.Warning("milestoneDefinitionJson is null. Unable to deserialize.");
                                }
                                milestoneHash = milestoneDefinitionRoot?.hash;
                                long? activityHash = null;
                                if (milestoneDefinitionRoot != null && milestoneDefinitionRoot.activities != null && milestoneDefinitionRoot.activities.Length > 0) {
                                    activityHash = milestoneDefinitionRoot?.activities[0].activityHash;
                                }

                                if(activityHash != null) {
                                    activityId =  unchecked((int) activityHash);
                                }
                                
                                string? iconUrl = milestoneDefinitionRoot?.displayProperties?.icon;
                                if (iconUrl != null) {
                                    weeklyRotatorTableJson.Add("iconUrl", iconUrl);
                                }
                            }
                        }
                    }

                    // Query to check if rotator already exist and sets a variable = to the has that was found
                    using (SqlCommand checkIfRowExistCommand = new SqlCommand($"SELECT * FROM {tableName} Where MilestoneHash = {milestoneHash}", msSqlConnection)){
                          using (SqlDataReader rowExistreader = checkIfRowExistCommand.ExecuteReader()){
                            while (rowExistreader.Read()){
                                milestoneHashInTable = rowExistreader["MilestoneHash"] as long?;
                            }
                        }
                    }

                    // If rotator exist it skips this index of the loop
                    if(milestoneHash == milestoneHashInTable){
                        Log.Information($"{milestoneHash} is already in table");
                        continue;
                    }

                    // Query to get Activity Definition using activityId
                    using (SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM DestinyActivityDefinition Where Id = {activityId}", msSqlConnection)){
                        using (SqlDataReader reader = sqlCommand.ExecuteReader()){
                            while (reader.Read()){
                                string? activityDefinitionJson = reader["json"].ToString();
                                ActivityDefinitionJson.Root? activityDefinitionRoot = null;
                                if(activityDefinitionJson != null){
                                    activityDefinitionRoot = JsonConvert.DeserializeObject<ActivityDefinitionJson.Root>(activityDefinitionJson);
                                }
                                activityName = activityDefinitionRoot?.originalDisplayProperties?.name;
                                activityName = activityName?.Replace(":", "");
                                string? pgcrImage = activityDefinitionRoot?.pgcrImage;
                                int? activityTypeHash = activityDefinitionRoot?.activityTypeHash;
                                if(activityTypeHash != null){
                                    activityTypeId = unchecked((int) activityTypeHash);
                                }
                                if(activityName != null){
                                    weeklyRotatorTableJson.Add("activityName",activityName);
                                }
                                if(pgcrImage != null){
                                    weeklyRotatorTableJson.Add("pcgrImage",pgcrImage);
                                }
                            }
                        }
                    }
                    
                    // Query to get activityType Id 
                    using (SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM DestinyActivityTypeDefinition Where Id = {activityTypeId}", msSqlConnection)){
                        using (SqlDataReader reader = sqlCommand.ExecuteReader()){
                            while (reader.Read()){
                                string? activityTypeDefinitionJson = reader["json"].ToString();
                                ActivityTypeDefinitionJson.Root? activityTypeDefinitionRoot = null;
                                if(activityTypeDefinitionJson != null){
                                    activityTypeDefinitionRoot = JsonConvert.DeserializeObject<ActivityTypeDefinitionJson.Root>(activityTypeDefinitionJson);
                                }
                                string? activityType = activityTypeDefinitionRoot?.displayProperties?.name;
                                if(activityType != null){
                                    weeklyRotatorTableJson.Add("activityType", activityType);
                                }
                            }
                        }

                    }
                    List<long> inventoryItemIdList = new List<long>();
                    var weeklyRotatorsMapping = new WeeklyRotatorsMapping();
                    // Query to get all items associated with an activiy names
                    if(weeklyRotatorsMapping.Mapping.ContainsKey(activityName ?? string.Empty)){
                        var itemHashes = weeklyRotatorsMapping.Mapping[activityName];
                        foreach(long itemHash in itemHashes){
                            long inventoryItemId = unchecked((int)itemHash);
                            inventoryItemIdList.Add(inventoryItemId);
                        }
                    }else{
                        using (SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM DestinyCollectibleDefinition WHERE Json LIKE @activityName", msSqlConnection)){
                            sqlCommand.Parameters.AddWithValue("@activityName", "%" + activityName + "%");
                            using (SqlDataReader reader = sqlCommand.ExecuteReader()){
                                while (reader.Read()){
                                    string? collectibleDefinitionJson = reader["json"].ToString();
                                    CollectibleDefinitionJson.Root? collectibleDefinitionRoot = null;
                                    if(collectibleDefinitionJson != null){
                                        collectibleDefinitionRoot = JsonConvert.DeserializeObject<CollectibleDefinitionJson.Root>(collectibleDefinitionJson)!;
                                    }
                                    long? itemHash = collectibleDefinitionRoot?.itemHash;
                                    long inventoryItemId = unchecked((int) itemHash);
                                    inventoryItemIdList.Add(inventoryItemId);
                                }
                            }
                        }
                    }

                    // Loops through all inventory item ids found for the activity
                    List<long> inventoryItemIntrinsicTraitList = new List<long>();
                    foreach(long itemId in inventoryItemIdList){
                        inventoryItemIntrinsicTraitList.Clear();
                        string inventoryItemName = "";
                        string inventoryItemIcon = "";
                        string inventoryItemScreenShot = "";
                        string inventoryItemSecondaryIcon = "";
                        string inventoryItemSecondarySpecial  = "";
                        string inventoryItemTypeAndTierDisplayName =  "";
                        string inventoryItemTierTypeName = "";
                        string inventoryItemIsCraftable = "";
                        string inventoryItemType = "";
                        string damageTypeIcon = "";
                        string damageTypeName = "";
                        string inventoryItemRpmStat = "";
                        string inventoryItemDescription = "";
                        // Finds the inventory item and sets information for it
                        using (SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM DestinyInventoryItemDefinition WHERE Id = {itemId}", msSqlConnection)){
                            using (SqlDataReader reader = sqlCommand.ExecuteReader()){
                                while (reader.Read()){
                                    string? inventoryItemDefinitionJson = reader["json"].ToString();
                                    InventoryItemDefinitionJson.Root? inventoryItemDefinitionRoot = null;
                                    if (inventoryItemDefinitionJson != null){
                                        inventoryItemDefinitionRoot = JsonConvert.DeserializeObject<InventoryItemDefinitionJson.Root>(inventoryItemDefinitionJson);
                                    }
                                    if (inventoryItemDefinitionRoot != null){
                                        inventoryItemName = inventoryItemDefinitionRoot.displayProperties?.name ?? "";
                                        inventoryItemIcon = inventoryItemDefinitionRoot.displayProperties?.icon ?? "";
                                        inventoryItemScreenShot = inventoryItemDefinitionRoot.screenshot ?? "";
                                        inventoryItemSecondaryIcon = inventoryItemDefinitionRoot.secondaryIcon ?? "";
                                        inventoryItemSecondarySpecial = inventoryItemDefinitionRoot.secondarySpecial ?? "";
                                        inventoryItemTypeAndTierDisplayName = inventoryItemDefinitionRoot.itemTypeAndTierDisplayName ?? "";
                                        inventoryItemTierTypeName = inventoryItemDefinitionRoot.inventory?.tierTypeName ?? "";
                                        inventoryItemDescription = inventoryItemDefinitionRoot.displayProperties.description ?? "";
                                        inventoryItemIsCraftable = "false";
                                        if (inventoryItemDefinitionRoot.sockets?.socketEntries != null){
                                            foreach (var socketEntry in inventoryItemDefinitionRoot.sockets.socketEntries){
                                                if (socketEntry.singleInitialItemHash == 1961918267){ // 1961918267 is a Empty DeepSight Socket Hash
                                                    inventoryItemIsCraftable = "true";
                                                }
                                                if (socketEntry.socketTypeHash == 3956125808){ // Is the Intrinsic Trait Socket Hash
                                                    if (socketEntry.singleInitialItemHash.HasValue) { // Check if the value is not null
                                                        long socketTypeId = unchecked((int) socketEntry.singleInitialItemHash);
                                                        inventoryItemIntrinsicTraitList.Add(socketTypeId); // Add the non-null value
                                                    }
                                                }
                                            }
                                        }
                                        int inventoryDamageTypeHash = -1;
                                        // Checks if the array is null and sets it to an impossible hash if it is
                                        if (inventoryItemDefinitionRoot.damageTypeHashes != null && inventoryItemDefinitionRoot.damageTypeHashes.Length > 0){
                                            inventoryDamageTypeHash = (int)inventoryItemDefinitionRoot.damageTypeHashes[0];
                                        }
                                        //Checks the mapping to set variables
                                        var damageTypeMapping = new DamageTypeMapping();
                                        if(damageTypeMapping.Mapping.TryGetValue(inventoryDamageTypeHash, out var result)){
                                            damageTypeIcon = result.Icon;
                                            damageTypeName = result.Name;
                                        }
                                        if (inventoryItemDefinitionRoot.stats != null && inventoryItemDefinitionRoot.stats.stats != null){
                                            foreach(var kvp in inventoryItemDefinitionRoot.stats.stats) {
                                                if (kvp.Key.Equals(4284893193)) { // 4284893193 is the Stat Hash for RPM
                                                    inventoryItemRpmStat = kvp.Value.value.ToString();
                                                }
                                            }
                                        }
                                                            
                                        if (inventoryItemDefinitionRoot.itemCategoryHashes?.Contains(22) == true && inventoryItemDefinitionRoot.itemCategoryHashes.Contains(20)){
                                            inventoryItemType = "TitanArmor";
                                        } else if (inventoryItemDefinitionRoot.itemCategoryHashes?.Contains(23) == true && inventoryItemDefinitionRoot.itemCategoryHashes.Contains(20)){
                                            inventoryItemType = "HunterArmor";
                                        } else if (inventoryItemDefinitionRoot.itemCategoryHashes?.Contains(21) == true && inventoryItemDefinitionRoot.itemCategoryHashes.Contains(20)){
                                            inventoryItemType = "WarlockArmor";
                                        } else if (inventoryItemDefinitionRoot.itemCategoryHashes?.Contains(1) == true){
                                            inventoryItemType = "Weapon";
                                        } else if (inventoryItemDefinitionRoot.itemCategoryHashes?.Contains(19) == true  || inventoryItemDefinitionRoot.itemCategoryHashes?.Contains(42)  == true || inventoryItemDefinitionRoot.itemCategoryHashes?.Contains(43)  == true || inventoryItemDefinitionRoot.itemCategoryHashes?.Contains(39) == true){ //Emblem, Ship, Sparrow, Ghost
                                            inventoryItemType = "Cosmetic";
                                        } else if (inventoryItemDefinitionRoot.itemCategoryHashes?.Contains(59) == true  ){ // Catalyst
                                            inventoryItemType = "Catalyst";
                                        }
                                    }
                                }
                            }
                        }

                        // This is for weapons. It checks if inventory item has an intrinisic frame and then looks for information on it. It only looks for information from the first trait in the list
                        // Other traits are left out
                        string inventoryItemFrameName = "";
                        string inventoryItemFrameDescription = "";
                        string inventoryItemFrameIcon = "";
                        if(inventoryItemIntrinsicTraitList.Count > 0 && inventoryItemType != "Catalyst"){
                            using (SqlCommand findFrameInfo = new SqlCommand($"SELECT * FROM DestinyInventoryItemDefinition WHERE Id = {inventoryItemIntrinsicTraitList[0]}", msSqlConnection)){
                                using (SqlDataReader findFrameInfoReader = findFrameInfo.ExecuteReader()){
                                    while (findFrameInfoReader.Read()){
                                        string? inventoryItemDefinitionJson = findFrameInfoReader["json"].ToString();
                                        InventoryItemDefinitionJson.Root? inventoryItemDefinitionRoot = null;
                                        if (inventoryItemDefinitionJson != null){
                                            inventoryItemDefinitionRoot = JsonConvert.DeserializeObject<InventoryItemDefinitionJson.Root>(inventoryItemDefinitionJson);
                                        }
                                        inventoryItemFrameName = inventoryItemDefinitionRoot.displayProperties?.name ?? "";
                                        inventoryItemFrameDescription = inventoryItemDefinitionRoot.displayProperties?.description ?? "";
                                        inventoryItemFrameIcon = inventoryItemDefinitionRoot.displayProperties?.icon ?? "";
                                    }
                                }
                            }
                        }

                        // Starts building dictionary/Json object string
                        Dictionary<string, object> itemObject = new Dictionary<string, object>();
                        Dictionary<string, string> innerDictionary = new Dictionary<string, string>();
                        if(inventoryItemType == "TitanArmor" || inventoryItemType == "HunterArmor" || inventoryItemType == "WarlockArmor"){
                            innerDictionary = new Dictionary<string, string>{
                                { "name", inventoryItemName },
                                { "icon", inventoryItemIcon },
                                { "screenshot", inventoryItemScreenShot },
                                { "typeAndTierDisplayName", inventoryItemTypeAndTierDisplayName },
                                { "tierTypeName", inventoryItemTierTypeName }
                            };
                        }else if(inventoryItemType == "Weapon"){
                            innerDictionary = new Dictionary<string, string>{
                                { "name", inventoryItemName },
                                { "icon", inventoryItemIcon },
                                { "screenshot", inventoryItemScreenShot },
                                { "typeAndTierDisplayName", inventoryItemTypeAndTierDisplayName },
                                { "tierTypeName", inventoryItemTierTypeName },
                                { "damageTypeName", damageTypeName },
                                { "damageTypeIcon", damageTypeIcon },
                                { "isCraftable", inventoryItemIsCraftable },
                                { "rpmStat", inventoryItemRpmStat },
                                { "frameName", inventoryItemFrameName },
                                { "frameDescription", inventoryItemFrameDescription },
                                { "frameIcon", inventoryItemFrameIcon }
                            };
                        }else if(inventoryItemType == "Cosmetic"){
                            innerDictionary = new Dictionary<string, string>{
                                { "name", inventoryItemName },
                                { "icon", inventoryItemIcon },
                                { "screenshot", inventoryItemScreenShot },
                                { "secondaryIcon", inventoryItemSecondaryIcon },
                                { "secondarySpecial", inventoryItemSecondarySpecial },
                                { "typeAndTierDisplayName", inventoryItemTypeAndTierDisplayName },
                                { "tierTypeName", inventoryItemTierTypeName }
                            };
                        }else if(inventoryItemType == "Catalyst"){
                            innerDictionary = new Dictionary<string, string>{
                                { "name", inventoryItemName },
                                { "icon", inventoryItemIcon },
                                { "description", inventoryItemDescription },
                                { "typeAndTierDisplayName", inventoryItemTypeAndTierDisplayName },
                                { "tierTypeName", inventoryItemTierTypeName }
                            };
                        }

                        // Adds dictionary and item name to an object and then appends that to a list
                        itemObject.Add(inventoryItemName, innerDictionary);
                        if (inventoryItemType != null){
                            switch (inventoryItemType){
                            case "TitanArmor":
                                int titanIndex = 0;
                                if (inventoryItemTypeAndTierDisplayName.Contains("Helmet"))
                                {
                                    titanIndex = Math.Min(0, titanArmor.Count); 
                                }
                                else if (inventoryItemTypeAndTierDisplayName.Contains("Gauntlets"))
                                {
                                    titanIndex = Math.Min(1, titanArmor.Count); 
                                }
                                else if (inventoryItemTypeAndTierDisplayName.Contains("Chest Armor"))
                                {
                                    titanIndex = Math.Min(2, titanArmor.Count);
                                }
                                else if (inventoryItemTypeAndTierDisplayName.Contains("Leg Armor"))
                                {
                                    titanIndex = Math.Min(3, titanArmor.Count);
                                }
                                else if (inventoryItemTypeAndTierDisplayName.Contains("Mark"))
                                {
                                    titanIndex = Math.Min(4, titanArmor.Count);
                                }
                                titanArmor.Insert(titanIndex, itemObject);
                                break;
                            case "HunterArmor":
                                int hunterIndex = 0;
                                if (inventoryItemTypeAndTierDisplayName.Contains("Helmet"))
                                {
                                    hunterIndex = Math.Min(0, hunterArmor.Count); // Ensure index is within bounds
                                }
                                else if (inventoryItemTypeAndTierDisplayName.Contains("Gauntlets"))
                                {
                                    hunterIndex = Math.Min(1, hunterArmor.Count); 
                                }
                                else if (inventoryItemTypeAndTierDisplayName.Contains("Chest Armor"))
                                {
                                    hunterIndex = Math.Min(2, hunterArmor.Count);
                                }
                                else if (inventoryItemTypeAndTierDisplayName.Contains("Leg Armor"))
                                {
                                    hunterIndex = Math.Min(3, hunterArmor.Count);
                                }
                                else if (inventoryItemTypeAndTierDisplayName.Contains("Cloak"))
                                {
                                    hunterIndex = Math.Min(4, hunterArmor.Count);
                                }
                                hunterArmor.Insert(hunterIndex, itemObject);
                                break;
                            case "WarlockArmor":
                                int warlockIndex = 0;
                                if (inventoryItemTypeAndTierDisplayName.Contains("Helmet"))
                                {
                                    warlockIndex = Math.Min(0, warlockArmor.Count);
                                }
                                else if (inventoryItemTypeAndTierDisplayName.Contains("Gauntlets"))
                                {
                                    warlockIndex = Math.Min(1, warlockArmor.Count);
                                }
                                else if (inventoryItemTypeAndTierDisplayName.Contains("Chest Armor"))
                                {
                                    warlockIndex = Math.Min(2, warlockArmor.Count);
                                }
                                else if (inventoryItemTypeAndTierDisplayName.Contains("Leg Armor"))
                                {
                                    warlockIndex = Math.Min(3, warlockArmor.Count);
                                }
                                else if (inventoryItemTypeAndTierDisplayName.Contains("Bond"))
                                {
                                    warlockIndex = Math.Min(4, warlockArmor.Count);
                                }
                                warlockArmor.Add(itemObject);
                                break;
                            case "Weapon":
                                if(inventoryItemTierTypeName.Contains("Exotic")){
                                    weapons.Insert(0, itemObject);
                                }else{ 
                                    weapons.Add(itemObject);
                                }
                                break;
                            case "Cosmetic":
                                if(inventoryItemTierTypeName.Contains("Exotic")){
                                    cosmetics.Insert(0, itemObject);
                                }else{ 
                                    cosmetics.Add(itemObject);
                                }
                                break;

                            case "Catalyst":
                                catalysts.Add(itemObject);
                                break;
                            }
                        }
                    }

                    // Puts together all the list and builds one big json object
                    weeklyRotatorTableJson.Add("titanArmor", titanArmor);
                    weeklyRotatorTableJson.Add("hunterArmor", hunterArmor);
                    weeklyRotatorTableJson.Add("warlockArmor", warlockArmor);
                    weeklyRotatorTableJson.Add("weapons", weapons);
                    weeklyRotatorTableJson.Add("cosmetics", cosmetics);
                    weeklyRotatorTableJson.Add("catalysts", catalysts);

                    // Serializes object to convert to string and inserts it into the table       
                    string jsonString = JsonConvert.SerializeObject(weeklyRotatorTableJson);
                    using (SqlCommand insertRowCommand = new SqlCommand($"INSERT INTO {tableName} (MilestoneHash, Json) VALUES (@MilestoneHash, @Json)", msSqlConnection)){
                        // Set parameter values and execute query
                        insertRowCommand.Parameters.AddWithValue("@MilestoneHash", milestoneHash);
                        insertRowCommand.Parameters.AddWithValue("@Json", jsonString);
                        insertRowCommand.ExecuteNonQuery();
                    }
                    Console.WriteLine(jsonString);
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

}

// Item Categories
// Titan 22
// Hunter 23
// Warlock 21
// Armor 20
// Weapon 1
