import json
from sql_functions import build_weekly_rotator_table,fetch_one,fetch_all,execute_query
from helper_functions import convert_hash_to_id
from mappings import damage_type_mapping, weekly_rotators_mapping

def add_weekly_rotators(weekly_rotator_hash_list, config, logger):
    """Adds weekly rotators to the database"""
    # Check if Weekly Rotators Table Exist and build it if it does not
    try:
        logger.info('Checking if Weekly Rotators Table exists')
        build_weekly_rotator_table(config=config,logger=logger)
    except Exception as e:
        logger.error(f'Failed to run build weekly rotators method: {e}')

    # Loops through Activity Hashes in the weekly rotator hash list and adds a row to SQL Server
    for activity_hash in weekly_rotator_hash_list:

        weekly_rotator_dict = {}

        # Checks if activity already exist inside Weekly Rotators Table and skips iteration if it does
        if fetch_one(query=f"SELECT * FROM WeeklyRotatorsTable Where Hash = {activity_hash}", config=config,logger=logger) is not None:
            logger.info(f"Found weekly rotator {activity_hash} inside the WeeklyRotatorsTable skipping")
            continue

        # Gets Destiny activity definition json string and converts it to a dictionary
        activity_id = convert_hash_to_id(activity_hash)
        destiny_activity_definition_row = fetch_one(query=f"SELECT * FROM DestinyActivityDefinition Where Id = {activity_id}", config=config, logger=logger)
        destiny_activity_definition_dict = json.loads(destiny_activity_definition_row[1])

        # Adds needed information from destiny_activity_definition_row to weekly_rotator_dict
        weekly_rotator_dict['activityName'] = destiny_activity_definition_dict['originalDisplayProperties']['name'].replace(':', '')
        weekly_rotator_dict['iconUrl'] = destiny_activity_definition_dict['originalDisplayProperties']['icon']
        weekly_rotator_dict['pgcrImage'] = destiny_activity_definition_dict['pgcrImage']

        # Gets Destiny activity type definition json string and converts it to a dictionary
        activity_type_id  = convert_hash_to_id(destiny_activity_definition_dict['activityTypeHash'])
        destiny_activity_type_definition_row = fetch_one(query=f'SELECT * FROM DestinyActivityTypeDefinition Where Id = {activity_type_id}',config=config, logger=logger)
        destiny_activity_type_definition_dict = json.loads(destiny_activity_type_definition_row[1])

        # Adds needed information from destiny_activity_type_definition_row to weekly_rotator_dict
        weekly_rotator_dict['activityType'] = destiny_activity_type_definition_dict['displayProperties']['name']

        # Gets a list of the inventory item ids
        inventory_item_id_list = []
        if weekly_rotator_dict['activityName'] in weekly_rotators_mapping:
            """Populates inventory item ids list if the activity name exists inside the weekly rotators mapping"""
            inventory_item_hash_list = weekly_rotators_mapping[weekly_rotator_dict['activityName']]
            for inventory_item_hash in inventory_item_hash_list:
                inventory_item_id_list.append(convert_hash_to_id(inventory_item_hash))
        else:
            """Populates inventory item ids list if the activity name does not exist in the weekly rotators mapping"""
            activity_name = weekly_rotator_dict['activityName'].replace("'", "''")
            destiny_collectible_definition_rows = fetch_all(query=f"SELECT * FROM DestinyCollectibleDefinition WHERE Json LIKE '%{activity_name}%'", config=config, logger=logger)
            for destiny_collectible_definition_row in destiny_collectible_definition_rows:
                destiny_collectible_definition_dict = json.loads(destiny_collectible_definition_row[1])
                inventory_item_id_list.append(convert_hash_to_id(destiny_collectible_definition_dict['itemHash']))

        # Builds List of item dictionaries to append to the Weekly Rotator Dict
        if len(inventory_item_id_list) > 0:
            # Initialize lists for items
            titan_armor = {}
            hunter_armor = {}
            warlock_armor = {}
            weapons = {}
            cosmetics = {}
            catalysts = {}
            for item_id in inventory_item_id_list:
                try:
                    destiny_inventory_item_definition_row = fetch_one(query=f"SELECT * FROM DestinyInventoryItemDefinition WHERE Id = {item_id}", config=config, logger=logger)
                    destiny_inventory_item_definition_dict = json.loads(destiny_inventory_item_definition_row[1])
                except Exception as e:
                    logger.error(f"Failed to fetch inventory item id {item_id}: {e}")
                    continue

                if destiny_inventory_item_definition_dict:
                    inventory_item_name = destiny_inventory_item_definition_dict.get('displayProperties', {}).get('name', "")
                    inventory_item_icon = destiny_inventory_item_definition_dict.get('displayProperties', {}).get('icon', "")
                    inventory_item_screenshot = destiny_inventory_item_definition_dict.get('screenshot', "")
                    inventory_item_secondary_icon = destiny_inventory_item_definition_dict.get('secondaryIcon', "")
                    inventory_item_secondary_special = destiny_inventory_item_definition_dict.get('secondarySpecial', "")
                    inventory_item_type_and_tier_display_name = destiny_inventory_item_definition_dict.get('itemTypeAndTierDisplayName', "")
                    inventory_item_tier_type_name = destiny_inventory_item_definition_dict.get('inventory', {}).get('tierTypeName',"")
                    inventory_item_description = destiny_inventory_item_definition_dict.get('displayProperties', {}).get('description', "")
                    inventory_item_is_craftable = False


                    # Finds Damage type Icon and Name based on the Hash found in dict
                    if destiny_inventory_item_definition_dict.get('damageTypeHashes') is not None and len(destiny_inventory_item_definition_dict['damageTypeHashes']) > 0:
                        inventory_item_damage_type_hash = convert_hash_to_id(destiny_inventory_item_definition_dict['damageTypeHashes'][0])
                    else:
                        inventory_item_damage_type_hash = None

                    if inventory_item_damage_type_hash is not None and inventory_item_damage_type_hash in damage_type_mapping:
                        damage_type_icon = damage_type_mapping[inventory_item_damage_type_hash][0]
                        damage_type_name = damage_type_mapping[inventory_item_damage_type_hash][1]
                    else:
                        damage_type_icon = None
                        damage_type_name = None

                    # Finds RPM stat for any weapon
                    stats = destiny_inventory_item_definition_dict.get('stats', {}).get('stats', {})
                    inventory_item_rpm_stat = stats.get(str(4284893193), {}).get('value', None)

                    # Adds inventory item type to item
                    item_category_hashes = destiny_inventory_item_definition_dict.get('itemCategoryHashes', [])
                    if 22 in item_category_hashes and 20 in item_category_hashes:
                        inventory_item_type = "TitanArmor"
                    elif 23 in item_category_hashes and 20 in item_category_hashes:
                        inventory_item_type = "HunterArmor"
                    elif 21 in item_category_hashes and 20 in item_category_hashes:
                        inventory_item_type = "WarlockArmor"
                    elif 1 in item_category_hashes:
                        inventory_item_type = "Weapon"
                    elif any(hash in item_category_hashes for hash in [19, 42, 43, 39]):  # Emblem, Ship, Sparrow, Ghost
                        inventory_item_type = "Cosmetic"
                    elif 59 in item_category_hashes:  # Catalyst
                        inventory_item_type = "Catalyst"
                    else:
                        inventory_item_type = None

                    inventory_item_intrinsic_trait_list = []
                    if 'sockets' in destiny_inventory_item_definition_dict and destiny_inventory_item_definition_dict['sockets'].get('socketEntries') is not None:
                        for socket_entry in destiny_inventory_item_definition_dict['sockets']['socketEntries']:
                            if socket_entry.get('singleInitialItemHash') == 1961918267:
                                inventory_item_is_craftable = True

                            # Check if the 'socketTypeHash' is 3956125808 (Intrinsic Trait Socket Hash)
                            if socket_entry.get('socketTypeHash') == 3956125808:
                                # Check if 'singleInitialItemHash' has a value (non-null)
                                if socket_entry.get('singleInitialItemHash') is not None:
                                    socket_type_id = convert_hash_to_id(socket_entry.get('singleInitialItemHash'))
                                    inventory_item_intrinsic_trait_list.append(socket_type_id)
                    else:
                        logger.info(f'No socket entries found for {inventory_item_name}')

                    inventory_item_frame_name = None
                    inventory_item_frame_description = None
                    inventory_item_frame_icon = None
                    if len(inventory_item_intrinsic_trait_list) > 0 and inventory_item_type != "Catalyst":
                        destiny_inventory_item_definition_intrinsic_trait_row = fetch_one(query=f"SELECT * FROM DestinyInventoryItemDefinition WHERE Id = {inventory_item_intrinsic_trait_list[0]}", config=config, logger=logger)
                        destiny_inventory_item_definition_intrinsic_trait_dict = json.loads(destiny_inventory_item_definition_intrinsic_trait_row[1])

                        inventory_item_frame_name = destiny_inventory_item_definition_intrinsic_trait_dict.get('displayProperties').get('name', '')
                        inventory_item_frame_description = destiny_inventory_item_definition_intrinsic_trait_dict.get('displayProperties').get('description', '')
                        inventory_item_frame_icon = destiny_inventory_item_definition_intrinsic_trait_dict.get('displayProperties').get('icon', '')
                    else:
                        logger.info(f'Item {inventory_item_name} does not need frame details')

                    if inventory_item_type in ["TitanArmor", "HunterArmor", "WarlockArmor"]:
                        inner_dictionary = {
                            "icon": inventory_item_icon,
                            "screenshot": inventory_item_screenshot,
                            "typeAndTierDisplayName": inventory_item_type_and_tier_display_name,
                            "tierTypeName": inventory_item_tier_type_name
                        }
                        if inventory_item_type == "TitanArmor":
                            titan_armor.update({inventory_item_name: inner_dictionary})
                        elif inventory_item_type == "HunterArmor":
                            hunter_armor.update({inventory_item_name: inner_dictionary})
                        elif inventory_item_type == "WarlockArmor":
                            warlock_armor.update({inventory_item_name: inner_dictionary})
                    elif inventory_item_type == "Weapon":
                        inner_dictionary = {
                            "icon": inventory_item_icon,
                            "screenshot": inventory_item_screenshot,
                            "typeAndTierDisplayName": inventory_item_type_and_tier_display_name,
                            "tierTypeName": inventory_item_tier_type_name,
                            "damageTypeName": damage_type_name,
                            "damageTypeIcon": damage_type_icon,
                            "isCraftable": inventory_item_is_craftable,
                            "rpmStat": inventory_item_rpm_stat,
                            "frameName": inventory_item_frame_name,
                            "frameDescription": inventory_item_frame_description,
                            "frameIcon": inventory_item_frame_icon
                        }
                        # Append flattened weapon data
                        weapons.update({inventory_item_name: inner_dictionary})
                    elif inventory_item_type == "Cosmetic":
                        inner_dictionary = {
                            "icon": inventory_item_icon,
                            "screenshot": inventory_item_screenshot,
                            "secondaryIcon": inventory_item_secondary_icon,
                            "secondarySpecial": inventory_item_secondary_special,
                            "typeAndTierDisplayName": inventory_item_type_and_tier_display_name,
                            "tierTypeName": inventory_item_tier_type_name
                        }
                        # Append flattened cosmetic data
                        cosmetics.update({inventory_item_name: inner_dictionary})
                    elif inventory_item_type == "Catalyst":
                        inner_dictionary = {
                            "icon": inventory_item_icon,
                            "description": inventory_item_description,
                            "typeAndTierDisplayName": inventory_item_type_and_tier_display_name,
                            "tierTypeName": inventory_item_tier_type_name
                        }
                        # Append flattened catalyst data
                        catalysts.update({inventory_item_name: inner_dictionary})
                    else:
                        logger.info(
                            'Inventory item type not recognized therefore inventory items will not be added to this entry'
                        )

                    weekly_rotator_dict["titanArmor"] = titan_armor
                    weekly_rotator_dict["hunterArmor"] = hunter_armor
                    weekly_rotator_dict["warlockArmor"] = warlock_armor
                    weekly_rotator_dict["cosmetics"] = cosmetics
                    weekly_rotator_dict["catalysts"] = catalysts
                    weekly_rotator_dict["weapons"] = weapons
        else:
            logger.info('Inventory item list is empty')
        logger.info(f"This is the weekly rotator dictionary {weekly_rotator_dict}")

        json_string = json.dumps(weekly_rotator_dict)

        insert_query = "INSERT INTO WeeklyRotatorsTable (Hash, JsonString) VALUES (?, ?)"
        try:
            execute_query(query=insert_query, params=(activity_hash, json_string), config=config, logger=logger)
            logger.info(f"{activity_hash} was successfully inserted")
        except Exception as e:
            logger.info(f"{activity_hash} was not successfully inserted: {e}")











