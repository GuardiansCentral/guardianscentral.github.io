from sql_functions import build_weekly_data_table,does_item_exists_in_column,execute_query,fetch_one
import json


def update_weekly_data_table(config, logger):
    logger.info("Starting the process of updating the weekly data table with the new Weekly Rotators")
    logger.info("Checking if the weekly data table exists")
    build_weekly_data_table(config, logger)

    if not does_item_exists_in_column(config=config, logger=logger, table_name="WeeklyDataTable", name_to_check="WeeklyRotatorsData", column_name="DataType"):
        logger.info("The weekly rotators data does not exist in the weekly data table. Attempting to creat an empty row")
        execute_query(query=f"INSERT INTO WeeklyDataTable (DataType, JsonString) VALUES ('WeeklyRotatorsData', '')" , config=config, logger=logger, params=None)
    else:
        logger.info("The weekly rotators data does exist in the weekly data table")

    active_weekly_rotators_row = fetch_one(query=f"SELECT RotatorList FROM ActiveWeeklyRotatorsTable WHERE NAME = 'ActiveWeeklyRotators'",config=config,logger=logger, params=None)
    active_weekly_rotators_string = active_weekly_rotators_row[0]
    rotators_string_list = active_weekly_rotators_string.strip('[]').split(',')
    active_weekly_rotators_list = [int(x.strip()) for x in rotators_string_list]
    logger.info(f"This is the current active weekly rotators list {active_weekly_rotators_list}")


    if not active_weekly_rotators_list:
        logger.debug("The weekly rotator list is empty. Will not update the weekly data table")
        return
    else:
        logger.info(f"Found the weekly rotator list")


    raid_one_found = False
    raid_two_found = False
    dungeon_one_found = False
    dungeon_two_found = False
    exotic_one_found = False
    weekly_rotators_data_dict = {}
    for activity_hash in active_weekly_rotators_list:
        activity_data_row = fetch_one(query=f"SELECT JsonString FROM gcdev.dbo.WeeklyRotatorsTable WHERE Hash = '{activity_hash}'",config=config,logger=logger, params=None)
        activity_data = json.loads(activity_data_row[0])
        logger.info(f"This is the activity data for {activity_hash}: {activity_data}")

        if activity_data.get('activityType') == 'Raid':
            if not raid_one_found:
                raid_one_found = True
                weekly_rotators_data_dict['raid1'] = activity_data
            else:
                raid_two_found = True
                weekly_rotators_data_dict['raid2'] = activity_data
        elif activity_data.get('activityType') == 'Dungeon':
            if not dungeon_one_found:
                dungeon_one_found = True
                weekly_rotators_data_dict['dungeon1'] = activity_data
            else:
                dungeon_two_found = True
                weekly_rotators_data_dict['dungeon2'] = activity_data
        elif activity_data.get('activityType') == 'Story':
            if not exotic_one_found:
                exotic_one_found = True
                weekly_rotators_data_dict['exotic1'] = activity_data

    if raid_one_found and raid_two_found and dungeon_one_found and dungeon_two_found and exotic_one_found:
        json_string = json.dumps(weekly_rotators_data_dict)
        logger.info(f"The weekly rotators data: {json_string}")
        logger.info("Found all the new weekly rotators data. Attempting to update the weekly data table now")
        execute_query(query = f"UPDATE WeeklyDataTable SET JsonString = ? WHERE DataType = 'WeeklyRotatorsData'", config=config, logger=logger, params=(json_string,))
        logger.info("Finished updating the weekly data table with the new Weekly Rotators")
    else:
        logger.debug(f"Failed to find new weekly rotators data will not update the weekly data table")
        logger.debug(f"raid1bool = {raid_one_found}, raid2bool = {raid_two_found}, dungeon1bool = {dungeon_one_found},dungeon2Bool = {dungeon_two_found}, exotic1bool = {exotic_one_found}")
