import sqlite3

def add_new_column(db_path, table_name, column_name, column_type, default_value=None):
    # Connect to the SQLite database
    connection = sqlite3.connect(db_path)
    cursor = connection.cursor()

    # Add the new column to the table
    try:
        cursor.execute(f"ALTER TABLE {table_name} ADD COLUMN {column_name} {column_type} DEFAULT {default_value}")
        print(f"New column '{column_name}' added successfully.")
    except sqlite3.Error as e:
        print(f"Error adding new column: {e}")

    # Commit the changes and close the connection
    connection.commit()
    connection.close()


db_path = "/Volumes/BCBDRIVE/ETSU_Fall_23/S_E_1/ScavengeRUs/ScavengeRUs/ScavengeRUs/ScavengeRUS.db"
table_name = "AspNetUsers"
column_name = "TeamCodes"
column_type = "STRING"  # Adjust the data type as needed
default_value = None  # Optional, set a default value if needed

add_new_column(db_path, table_name, column_name, column_type, default_value)
