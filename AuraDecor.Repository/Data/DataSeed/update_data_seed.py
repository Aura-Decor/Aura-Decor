"""
Python script to add CreatedAt and UpdatedAt properties to JSON data seed files
This script adds timestamp properties to all entities in the data seed files
"""

import json
import os
import random
from datetime import datetime, timedelta, timezone

def generate_random_dates():
    """Generate realistic random CreatedAt and UpdatedAt dates"""
    # Random date within the last 6 months
    days_back = random.randint(0, 180)
    hours_back = random.randint(0, 24)
    minutes_back = random.randint(0, 60)
    
    # Create the created date
    created_date = datetime.now(timezone.utc) - timedelta(
        days=days_back, 
        hours=hours_back, 
        minutes=minutes_back
    )
    
    # Updated date is sometime between created date and now
    max_days_forward = min(days_back, 180)
    days_forward = random.randint(0, max_days_forward)
    updated_date = created_date + timedelta(days=days_forward)
    
    # Ensure updated date is not in the future
    if updated_date > datetime.now(timezone.utc):
        updated_date = datetime.now(timezone.utc)
    
    return created_date, updated_date

def update_json_file(file_path):
    """Update a JSON file with CreatedAt and UpdatedAt properties"""
    print(f"Updating {file_path}...")
    
    try:
        # Read the JSON file
        with open(file_path, 'r', encoding='utf-8') as file:
            data = json.load(file)
        
        # Add CreatedAt and UpdatedAt to each item
        for item in data:
            created_date, updated_date = generate_random_dates()
            
            # Add the properties in ISO 8601 format
            item['createdAt'] = created_date.isoformat()
            item['updatedAt'] = updated_date.isoformat()
        
        # Write back to file with proper formatting
        with open(file_path, 'w', encoding='utf-8') as file:
            json.dump(data, file, indent=2, ensure_ascii=False)
        
        print(f"✓ Successfully updated {file_path}")
        return True
        
    except Exception as e:
        print(f"✗ Failed to update {file_path}: {str(e)}")
        return False

def main():
    """Main function to update all JSON data seed files"""
    print("Starting to update JSON data seed files with CreatedAt and UpdatedAt properties...")
    print(f"Current date: {datetime.now(timezone.utc).isoformat()}")
    print()
    
    # List of JSON files to update
    json_files = [
        "brands.json",
        "categories.json", 
        "colors.json",
        "styletypes.json",
        "Furniture.json"
    ]
    
    success_count = 0
    total_files = len(json_files)
    
    # Update each JSON file
    for filename in json_files:
        if os.path.exists(filename):
            if update_json_file(filename):
                success_count += 1
        else:
            print(f"⚠ Warning: File not found: {filename}")
    
    print()
    print(f"Completed! Successfully updated {success_count}/{total_files} files.")
    print("Note: Each entity has been given random dates within the last 6 months for more realistic test data.")

if __name__ == "__main__":
    main() 