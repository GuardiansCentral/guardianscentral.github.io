import logging
import tomllib
from add_weekly_rotators import add_weekly_rotators
from update_active_weekly_rotators import update_active_weekly_rotators
from update_weekly_data_table import update_weekly_data_table
from datetime import datetime

# Importing config data
with open("config.toml", "rb") as file:
    config = tomllib.load(file)


# Create and configure logger
# Generate a timestamped filename
log_filename = f"weekly_rotators_{datetime.now().strftime('%Y-%m-%d_%H-%M-%S')}.log"

logging.basicConfig(
    filename=log_filename,
    format='%(asctime)s %(message)s',
    datefmt='%Y-%m-%d %H:%M:%S',
    filemode='w',
    level=logging.INFO
)

# Creating an object
logger = logging.getLogger()

# Setting the threshold of logger to DEBUG
logger.setLevel(logging.DEBUG)

# Update this list with any new activity hashes
weekly_rotators_hash_list = [2122313384, 1042180643, 910380154, 3881495763,
                             1441982566, 1374392663, 2381413764, 107319834,
                             2823159265, 2032534090, 1077850348, 4078656646,
                             313828469, 2668737148, 1221538367, 509188661,
                             196691221, 3883295757, 2582501063, 1262462921
                             ]
logger.info(f"Logging to {log_filename}")
try:
    logger.info(f"Starting add weekly rotators method")
    add_weekly_rotators(weekly_rotator_hash_list=weekly_rotators_hash_list, config=config, logger=logger)
except Exception as e:
    logger.error(f"Something went wrong when adding weekly rotators method was running: {e}")

try:
    logger.info(f"Starting update active weekly rotators method")
    update_active_weekly_rotators(config=config, logger=logger)
except Exception as e:
    logger.error(f"Something went wrong when updating active weekly rotators method was running: {e}")

try:
    logger.info(f"Starting update weekly rotators data method")
    update_weekly_data_table(config=config, logger=logger)
except Exception as e:
    logger.error(f"Something went wrong when updating weekly data table was running: {e}")


