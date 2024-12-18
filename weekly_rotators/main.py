import pyodbc
import tomllib
from add_weekly_rotators import add_weekly_rotators
from update_active_weekly_rotators import update_active_weekly_rotators



with open("config.toml", "rb") as file:
    config = tomllib.load(file)
print(config.get('Server'))
# weekly_rotators_hash_list =['2122313384']
# add_weekly_rotators(weekly_rotator_hash_list=weekly_rotators_hash_list, config=config)
update_active_weekly_rotators(config=config)
# update_weekly_rotators_data
"""
This will essentially update the data in WeeklyDataTable by looking at the ActiveWeeklyRotatorsTable and grabbing the list in there. It will then use this list to 
build a json object that will be stored in the  WeeklyDataTable where name=WeeklyRotator
"""

# Then build a stored prcodure that pulls data from the WeeklyRotatorsTable and it will accept parameters name and reeturn the row where that name is at or just the json


