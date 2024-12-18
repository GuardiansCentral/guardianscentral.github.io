# Helper function to convert hash to ID
def convert_hash_to_id(hash_value):
    id = int(hash_value)
    if (id & (1 << (32 - 1))) != 0:
        id = id - (1 << 32)
    return id