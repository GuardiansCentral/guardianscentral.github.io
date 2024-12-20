import pyodbc
import logging
import tomllib
from add_weekly_rotators import add_weekly_rotators
from update_active_weekly_rotators import update_active_weekly_rotators



with open("config.toml", "rb") as file:
    config = tomllib.load(file)
print(config.get('Server'))


# Create and configure logger
logging.basicConfig(filename="newfile.log",
                    format='%(asctime)s %(message)s',
                    filemode='w')

# Creating an object
logger = logging.getLogger()

# Setting the threshold of logger to DEBUG
logger.setLevel(logging.DEBUG)


weekly_rotators_hash_list = [2122313384, 1042180643, 910380154, 3881495763,
                             1441982566, 1374392663, 2381413764, 107319834,
                             2823159265, 2032534090, 1077850348, 4078656646,
                             313828469, 2668737148, 1221538367, 509188661,
                             196691221, 3883295757, 2582501063, 1262462921
                             ]

add_weekly_rotators(weekly_rotator_hash_list=weekly_rotators_hash_list, config=config, logger=logger)
#update_active_weekly_rotators(config=config, logger=logger)
# update_weekly_rotators_data
"""
This will essentially update the data in WeeklyDataTable by looking at the ActiveWeeklyRotatorsTable and grabbing the list in there. It will then use this list to 
build a json object that will be stored in the  WeeklyDataTable where name=WeeklyRotator
"""

# Then build a stored prcodure that pulls data from the WeeklyRotatorsTable and it will accept parameters name and reeturn the row where that name is at or just the json


