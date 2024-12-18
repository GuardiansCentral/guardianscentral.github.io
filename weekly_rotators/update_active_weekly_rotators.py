from sql_functions import table_exists,build_active_weekly_rotator_table

def update_active_weekly_rotators(config):
    """Adds active weekly rotators to the database"""
    # Check if active weekly Rotators Table Exist and build it if it does not
    build_active_weekly_rotator_table(config)







#For this update the rotatorschedule table. The sequence should match the sequence for this episode. We are just changeing the sequence numbers to match bungies schedule instead
#of matching the release order of the activites. We will then be grabbing Raids and Dungeons in pairs and the single Story





