from sql_functions import table_exists,build_active_weekly_rotator_table, does_item_exists_in_column, execute_query
from weekly_rotators.sql_functions import fetch_one


def update_active_weekly_rotators(config):
    """Adds active weekly rotators to the database"""
    # Check if active weekly Rotators Table Exist and build it if it does not
    build_active_weekly_rotator_table(config)

    if not does_item_exists_in_column(config=config, table_name='ActiveWeeklyRotatorsTable', column_name="NAME", name_to_check="ActiveWeeklyRotators"):
        build_active_weekly_rotator_table(config)
        return
    else:
        print("Active weekly rotators list already exists inside the database")

    current_active_weekly_rotators_row = fetch_one(query=f"SELECT RotatorList FROM ActiveWeeklyRotatorsTable WHERE NAME = 'ActiveWeeklyRotators'",config=config,params=None)
    current_active_weekly_rotators_string = current_active_weekly_rotators_row[0]
    rotators_string_list = current_active_weekly_rotators_string.strip('[]').split(',')
    current_active_weekly_rotators_list = [int(x.strip()) for x in rotators_string_list]
    print(current_active_weekly_rotators_list)

    if not current_active_weekly_rotators_list:
        print("No active weekly rotators found")
        print("Creating new active weekly rotators list")
        build_active_weekly_rotator_table(config)
        return
    else:
        print("Active weekly rotators list exists")

    for activity in current_active_weekly_rotators_list:
        get_activity_query = f"SELECT Sequence,RotatorType,Hash, Name FROM RotatorSchedule WHERE Hash = {activity}"
        activity_row = fetch_one(query=get_activity_query,config=config,params=None)
        print(activity_row)

        sequence = activity_row[0]
        rotator_type = activity_row[1]

        max_sequence = get_max_sequence_by_rotator_type(config=config, activity_type=rotator_type)
        print(max_sequence)









def get_max_sequence_by_rotator_type(config, activity_type):
    query = (
        f"SELECT MAX(Sequence) AS MaxSequence "
        f"FROM RotatorSchedule "
        f"WHERE RotatorType = ?"
    )
    result = fetch_one(query=query, config=config, params=(activity_type,))
    return result[0] if result else None




#For this update the rotatorschedule table. The sequence should match the sequence for this episode. We are just changeing the sequence numbers to match bungies schedule instead
#of matching the release order of the activites. We will then be grabbing Raids and Dungeons in pairs and the single Story





