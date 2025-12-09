API Usage Examples

This document provides practical examples for using the WebLedger API through REST API calls and CLI commands.
Table of Contents

    Authentication Basics

    Category Management

    Type Management

    Entry Operations

    Query Operations

    Error Handling

    CLI Usage Examples

    Integration Examples

Authentication Basics

WebLedger uses custom HTTP headers for authentication. All API requests must include:
http

wl-access: your-access-key
wl-secret: your-secret-key

Obtaining Initial Credentials
bash

# Configure CLI for MySQL connection
echo '{
  "target": "mysql",
  "host": "server=localhost;port=3306;database=ledger;user=ledger-bot;password=your-password;"
}' > config.json

# Run CLI and generate credentials
cd cli
dotnet run

# In the CLI:
> ls-acc          # View existing accesses
> grant root      # Create new access named "root"

Save the output:
text

Access: root
Secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w

Category Management
Create Categories with Different Hierarchies
bash

# Create top-level categories
curl -X PUT "http://localhost:5143/ledger/category" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Food",
    "description": "All food-related expenses"
  }'

curl -X PUT "http://localhost:5143/ledger/category" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Transportation",
    "description": "Transportation costs"
  }'

curl -X PUT "http://localhost:5143/ledger/category" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Utilities",
    "description": "Monthly utility bills"
  }'

# Create sub-categories
curl -X PUT "http://localhost:5143/ledger/category" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Groceries",
    "parent": "Food",
    "description": "Supermarket purchases"
  }'

curl -X PUT "http://localhost:5143/ledger/category" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Dining",
    "parent": "Food",
    "description": "Restaurant meals"
  }'

curl -X PUT "http://localhost:5143/ledger/category" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Fuel",
    "parent": "Transportation",
    "description": "Gasoline and diesel"
  }'

curl -X PUT "http://localhost:5143/ledger/category" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Electricity",
    "parent": "Utilities",
    "description": "Electric power bills"
  }'

List All Categories
bash

curl -X GET "http://localhost:5143/ledger/categories" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w"

Type Management
Create Types with Default Categories
bash

# Food-related types
curl -X PUT "http://localhost:5143/ledger/type" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Milk",
    "defaultCategory": "Groceries",
    "description": "Milk and dairy products"
  }'

curl -X PUT "http://localhost:5143/ledger/type" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Pizza",
    "defaultCategory": "Dining",
    "description": "Pizza delivery or restaurant"
  }'

# Transportation types
curl -X PUT "http://localhost:5143/ledger/type" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gasoline",
    "defaultCategory": "Fuel",
    "description": "Car fuel purchases"
  }'

# Utility types
curl -X PUT "http://localhost:5143/ledger/type" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Electric Bill",
    "defaultCategory": "Electricity",
    "description": "Monthly electricity bill"
  }'

# Income types
curl -X PUT "http://localhost:5143/ledger/type" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Salary",
    "defaultCategory": "Income",
    "description": "Regular salary payment"
  }'

curl -X PUT "http://localhost:5143/ledger/type" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Freelance",
    "defaultCategory": "Income",
    "description": "Freelance work payment"
  }'

Entry Operations
Create Entries with Different Categories
bash

# Expense entries (negative amounts)
curl -X POST "http://localhost:5143/ledger/entry" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "Milk",
    "category": "Groceries",
    "amount": -4.50,
    "givenTime": "2024-01-15T09:30:00Z",
    "description": "Organic milk from supermarket"
  }'

curl -X POST "http://localhost:5143/ledger/entry" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "Pizza",
    "category": "Dining",
    "amount": -25.99,
    "givenTime": "2024-01-15T19:45:00Z",
    "description": "Friday night pizza delivery"
  }'

curl -X POST "http://localhost:5143/ledger/entry" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "Gasoline",
    "category": "Fuel",
    "amount": -65.75,
    "givenTime": "2024-01-16T14:20:00Z",
    "description": "Full tank at gas station"
  }'

curl -X POST "http://localhost:5143/ledger/entry" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "Electric Bill",
    "category": "Electricity",
    "amount": -120.50,
    "givenTime": "2024-01-17T00:00:00Z",
    "description": "January electricity bill"
  }'

# Income entries (positive amounts)
curl -X POST "http://localhost:5143/ledger/entry" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "Salary",
    "category": "Income",
    "amount": 3500.00,
    "givenTime": "2024-01-15T00:00:00Z",
    "description": "Monthly salary deposit"
  }'

curl -X POST "http://localhost:5143/ledger/entry" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "Freelance",
    "category": "Income",
    "amount": 850.00,
    "givenTime": "2024-01-18T10:00:00Z",
    "description": "Web development project payment"
  }'

Query Operations
Query by Date Range
bash

# Query entries from January 1 to January 31, 2024
curl -X POST "http://localhost:5143/ledger/select" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "startTime": "2024-01-01T00:00:00Z",
    "endTime": "2024-01-31T23:59:59Z",
    "limit": 100
  }'

# Query entries from last week
curl -X POST "http://localhost:5143/ledger/select" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "startTime": "2024-01-08T00:00:00Z",
    "endTime": "2024-01-14T23:59:59Z"
  }'

# Query entries from specific date
curl -X POST "http://localhost:5143/ledger/select" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "startTime": "2024-01-15T00:00:00Z",
    "endTime": "2024-01-15T23:59:59Z"
  }'

Query with Multiple Filters
bash

# Query Food category entries in January
curl -X POST "http://localhost:5143/ledger/select" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "startTime": "2024-01-01T00:00:00Z",
    "endTime": "2024-01-31T23:59:59Z",
    "categories": ["Food", "Groceries", "Dining"],
    "limit": 50
  }'

# Query specific types only
curl -X POST "http://localhost:5143/ledger/select" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "types": ["Milk", "Pizza", "Gasoline"],
    "limit": 30
  }'

# Query with pagination
curl -X POST "http://localhost:5143/ledger/select" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "limit": 20,
    "offset": 0
  }'

# Get next page
curl -X POST "http://localhost:5143/ledger/select" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "limit": 20,
    "offset": 20
  }'

Error Handling
Common Error Scenarios and Best Practices
1. Authentication Errors
bash

# Wrong access key
curl -X GET "http://localhost:5143/ledger/categories" \
  -H "wl-access: wrong-key" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w"

# Expected response: 401 Unauthorized

2. Missing Required Fields
bash

# Missing required 'amount' field
curl -X POST "http://localhost:5143/ledger/entry" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "Test",
    "category": "Test",
    "givenTime": "2024-01-15T10:30:00Z"
  }'

# Expected response: 400 Bad Request with error message

3. Invalid Data Types
bash

# Amount is string instead of number
curl -X POST "http://localhost:5143/ledger/entry" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "Test",
    "category": "Test",
    "amount": "not-a-number",
    "givenTime": "2024-01-15T10:30:00Z"
  }'

# Expected response: 400 Bad Request

4. Non-existent Category
bash

# Category doesn't exist
curl -X POST "http://localhost:5143/ledger/entry" \
  -H "wl-access: root" \
  -H "wl-secret: 5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "Test",
    "category": "NonExistentCategory",
    "amount": 100,
    "givenTime": "2024-01-15T10:30:00Z"
  }'

# Expected response: 400 Bad Request - "Category not found"

Error Handling Script Example
bash

#!/bin/bash
# Robust API calling with error handling

API_URL="http://localhost:5143"
ACCESS="root"
SECRET="5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w"
MAX_RETRIES=3

make_api_call() {
    local endpoint=$1
    local method=$2
    local data=$3
    local retry_count=0
    
    while [ $retry_count -lt $MAX_RETRIES ]; do
        response=$(curl -s -w "\n%{http_code}" \
            -X "$method" \
            -H "wl-access: $ACCESS" \
            -H "wl-secret: $SECRET" \
            -H "Content-Type: application/json" \
            -d "$data" \
            "$API_URL/$endpoint" 2>/dev/null)
        
        http_code=$(echo "$response" | tail -n1)
        body=$(echo "$response" | sed '$d')
        
        case $http_code in
            200|201)
                echo "$body"
                return 0
                ;;
            400)
                echo "Error: Bad request - $body" >&2
                return 1
                ;;
            401)
                echo "Error: Unauthorized - check your credentials" >&2
                return 1
                ;;
            429)
                echo "Warning: Rate limited, retrying..." >&2
                sleep 2
                retry_count=$((retry_count + 1))
                continue
                ;;
            5*)
                echo "Warning: Server error ($http_code), retrying..." >&2
                sleep $((2 ** retry_count))
                retry_count=$((retry_count + 1))
                continue
                ;;
            *)
                echo "Error: Unexpected status code $http_code" >&2
                return 1
                ;;
        esac
    done
    
    echo "Error: Max retries exceeded" >&2
    return 1
}

# Usage example
echo "Testing connection..."
if make_api_call "ledger/categories" "GET" ""; then
    echo "Connection successful"
else
    echo "Connection failed"
    exit 1
fi

CLI Usage Examples
Basic Configuration
json

{
  "target": "http",
  "host": "http://localhost:5143",
  "access": "root",
  "secret": "5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w"
}

Create Entries with CLI
bash

# Add entries with different categories
> add Milk -4.50 "Organic milk" --category Groceries
> add Pizza -25.99 "Friday night" --category Dining
> add Gasoline -65.75 "Full tank" --category Fuel
> add "Electric Bill" -120.50 "January bill" --category Electricity
> add Salary 3500.00 "Monthly deposit" --category Income
> add Freelance 850.00 "Web project" --category Income

Query with CLI
bash

# Query by date range
> select --from 2024-01-01 --to 2024-01-31

# Query specific categories
> select --category Food,Groceries,Dining

# Query with multiple filters
> select --from 2024-01-01 --to 2024-01-15 --category Transportation,Fuel --limit 20

Integration Examples
Python Integration with Error Handling
python

import requests
from datetime import datetime, timedelta
import time

class WebLedgerClient:
    def __init__(self, base_url="http://localhost:5143", access=None, secret=None):
        self.base_url = base_url.rstrip('/')
        self.access = access
        self.secret = secret
        self.session = requests.Session()
        
        if access and secret:
            self._update_headers()
    
    def _update_headers(self):
        self.session.headers.update({
            'wl-access': self.access,
            'wl-secret': self.secret,
            'Content-Type': 'application/json'
        })
    
    def _make_request(self, method, endpoint, data=None, retries=3):
        """Make HTTP request with retry logic"""
        url = f"{self.base_url}/{endpoint}"
        
        for attempt in range(retries):
            try:
                response = self.session.request(
                    method=method,
                    url=url,
                    json=data,
                    timeout=30
                )
                
                if response.status_code in [200, 201]:
                    return response
                elif response.status_code == 429:  # Rate limited
                    if attempt < retries - 1:
                        time.sleep(2 ** attempt)  # Exponential backoff
                        continue
                elif response.status_code >= 500:  # Server error
                    if attempt < retries - 1:
                        time.sleep(1 * (attempt + 1))
                        continue
                
                response.raise_for_status()
                
            except requests.exceptions.RequestException as e:
                if attempt == retries - 1:
                    raise e
                time.sleep(1 * (attempt + 1))
        
        return None
    
    def create_entry(self, type_name, category, amount, description="", given_time=None):
        """Create a new ledger entry with validation"""
        # Validate input
        if not type_name or not category:
            raise ValueError("Type and category are required")
        
        try:
            amount = float(amount)
        except ValueError:
            raise ValueError("Amount must be a number")
        
        if given_time is None:
            given_time = datetime.utcnow()
        
        data = {
            "type": type_name,
            "category": category,
            "amount": amount,
            "givenTime": given_time.isoformat() + 'Z',
            "description": description
        }
        
        response = self._make_request("POST", "ledger/entry", data)
        return response.text if response else None
    
    def query_entries_by_date_range(self, start_date, end_date, categories=None):
        """Query entries within date range"""
        data = {
            "startTime": start_date.isoformat() + 'Z',
            "endTime": end_date.isoformat() + 'Z',
            "limit": 1000
        }
        
        if categories:
            data["categories"] = categories if isinstance(categories, list) else [categories]
        
        response = self._make_request("POST", "ledger/select", data)
        return response.json() if response else []
    
    def get_monthly_report(self, year, month):
        """Generate monthly financial report"""
        start_date = datetime(year, month, 1)
        if month == 12:
            end_date = datetime(year + 1, 1, 1) - timedelta(seconds=1)
        else:
            end_date = datetime(year, month + 1, 1) - timedelta(seconds=1)
        
        entries = self.query_entries_by_date_range(start_date, end_date)
        
        report = {
            "month": f"{year}-{month:02d}",
            "total_entries": len(entries),
            "income": 0,
            "expenses": 0,
            "by_category": {},
            "by_type": {}
        }
        
        for entry in entries:
            amount = entry["amount"]
            category = entry["category"]
            type_name = entry["type"]
            
            if amount > 0:
                report["income"] += amount
            else:
                report["expenses"] += abs(amount)
            
            # Track by category
            if category not in report["by_category"]:
                report["by_category"][category] = 0
            report["by_category"][category] += amount
            
            # Track by type
            if type_name not in report["by_type"]:
                report["by_type"][type_name] = 0
            report["by_type"][type_name] += amount
        
        report["balance"] = report["income"] - report["expenses"]
        
        return report

# Usage example
client = WebLedgerClient(
    base_url="http://localhost:5143",
    access="root",
    secret="5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w"
)

# Create entries with different categories
entries_to_create = [
    ("Milk", "Groceries", -4.50, "Organic milk"),
    ("Pizza", "Dining", -25.99, "Friday dinner"),
    ("Gasoline", "Fuel", -65.75, "Gas station"),
    ("Salary", "Income", 3500.00, "Monthly salary"),
    ("Freelance", "Income", 850.00, "Web project")
]

for type_name, category, amount, description in entries_to_create:
    try:
        entry_id = client.create_entry(type_name, category, amount, description)
        print(f"Created entry: {entry_id}")
    except Exception as e:
        print(f"Failed to create entry: {e}")

# Query by date range
start_date = datetime(2024, 1, 1)
end_date = datetime(2024, 1, 31)
entries = client.query_entries_by_date_range(start_date, end_date)
print(f"Found {len(entries)} entries in January 2024")

# Generate monthly report
report = client.get_monthly_report(2024, 1)
print(f"Monthly Report for {report['month']}:")
print(f"  Income: ${report['income']:.2f}")
print(f"  Expenses: ${report['expenses']:.2f}")
print(f"  Balance: ${report['balance']:.2f}")

Shell Script for Batch Operations
bash

#!/bin/bash
# batch_operations.sh - Batch create and query operations

CONFIG_FILE="$HOME/.webledger/config"
API_URL="http://localhost:5143"

# Load configuration
if [ ! -f "$CONFIG_FILE" ]; then
    echo "Error: Configuration file not found"
    echo "Create $CONFIG_FILE with:"
    echo "export WL_ACCESS='root'"
    echo "export WL_SECRET='your-secret-key'"
    exit 1
fi

source "$CONFIG_FILE"

if [ -z "$WL_ACCESS" ] || [ -z "$WL_SECRET" ]; then
    echo "Error: WL_ACCESS and WL_SECRET must be set"
    exit 1
fi

# Function to call API
api_call() {
    local endpoint=$1
    local method=$2
    local data=$3
    
    response=$(curl -s -w "\n%{http_code}" \
        -X "$method" \
        -H "wl-access: $WL_ACCESS" \
        -H "wl-secret: $WL_SECRET" \
        -H "Content-Type: application/json" \
        -d "$data" \
        "$API_URL/$endpoint")
    
    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | sed '$d')
    
    echo "$body"
    return $http_code
}

# Batch create entries from CSV file
create_entries_from_csv() {
    local csv_file=$1
    
    if [ ! -f "$csv_file" ]; then
        echo "Error: CSV file not found: $csv_file"
        return 1
    fi
    
    success_count=0
    fail_count=0
    
    while IFS=',' read -r type category amount date description; do
        # Clean up fields
        type=$(echo "$type" | tr -d '"')
        category=$(echo "$category" | tr -d '"')
        amount=$(echo "$amount" | tr -d '"')
        date=$(echo "$date" | tr -d '"')
        description=$(echo "$description" | tr -d '"')
        
        # Create entry
        data="{
            \"type\": \"$type\",
            \"category\": \"$category\",
            \"amount\": $amount,
            \"givenTime\": \"${date}T12:00:00Z\",
            \"description\": \"$description\"
        }"
        
        if api_call "ledger/entry" "POST" "$data" >/dev/null; then
            echo "✓ Created: $type - $category - $amount"
            success_count=$((success_count + 1))
        else
            echo "✗ Failed: $type - $category - $amount"
            fail_count=$((fail_count + 1))
        fi
        
        # Small delay to avoid rate limiting
        sleep 0.5
        
    done < "$csv_file"
    
    echo "Batch creation completed:"
    echo "  Success: $success_count"
    echo "  Failed: $fail_count"
}

# Generate monthly summary
generate_monthly_summary() {
    local year=$1
    local month=$2
    
    start_date="${year}-${month}-01T00:00:00Z"
    if [ "$month" -eq 12 ]; then
        next_year=$((year + 1))
        end_date="${next_year}-01-01T00:00:00Z"
    else
        next_month=$((month + 1))
        end_date="${year}-${next_month}-01T00:00:00Z"
    fi
    
    data="{
        \"startTime\": \"$start_date\",
        \"endTime\": \"$end_date\",
        \"limit\": 1000
    }"
    
    response=$(api_call "ledger/select" "POST" "$data")
    
    if [ $? -eq 200 ]; then
        total_income=0
        total_expense=0
        
        while read -r line; do
            amount=$(echo "$line" | grep -o '"amount":[^,]*' | cut -d':' -f2)
            if (( $(echo "$amount > 0" | bc -l) )); then
                total_income=$(echo "$total_income + $amount" | bc -l)
            else
                total_expense=$(echo "$total_expense - $amount" | bc -l)
            fi
        done < <(echo "$response" | jq -c '.[]')
        
        balance=$(echo "$total_income - $total_expense" | bc -l)
        
        echo "Monthly Summary for ${year}-${month}:"
        echo "  Total Income: \$$(printf "%.2f" $total_income)"
        echo "  Total Expense: \$$(printf "%.2f" $total_expense)"
        echo "  Balance: \$$(printf "%.2f" $balance)"
    else
        echo "Error: Failed to generate summary"
    fi
}

# Main execution
echo "WebLedger Batch Operations"
echo "=========================="

# Example: Create entries from CSV
# CSV format: type,category,amount,date,description
echo "Creating entries from CSV..."
create_entries_from_csv "entries.csv"

# Example: Generate summary for January 2024
echo ""
echo "Generating monthly summary..."
generate_monthly_summary 2024 1

Best Practices
1. Secure Credential Management
bash

# Store credentials in environment variables
export WL_ACCESS="root"
export WL_SECRET="5jH!3NNF$HQs@KV2Q427HnN0RXgCeA$w"

# Use in scripts (never hardcode)
curl -H "wl-access: $WL_ACCESS" -H "wl-secret: $WL_SECRET" ...

2. Input Validation
python

def validate_entry_data(type_name, category, amount):
    """Validate entry data before sending to API"""
    errors = []
    
    if not type_name or not isinstance(type_name, str):
        errors.append("Type must be a non-empty string")
    
    if not category or not isinstance(category, str):
        errors.append("Category must be a non-empty string")
    
    try:
        amount = float(amount)
        if amount == 0:
            errors.append("Amount cannot be zero")
    except (ValueError, TypeError):
        errors.append("Amount must be a valid number")
    
    return errors

3. Rate Limiting and Retry Logic
python

import time

def make_api_call_with_retry(func, max_retries=3, base_delay=1):
    """Wrapper for API calls with exponential backoff"""
    for attempt in range(max_retries):
        try:
            return func()
        except Exception as e:
            if attempt == max_retries - 1:
                raise e
            
            # Exponential backoff
            delay = base_delay * (2 ** attempt)
            time.sleep(delay)
    
    raise Exception("Max retries exceeded")

4. Logging and Monitoring
python

import logging
import json

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

def log_api_call(endpoint, method, data, response, status_code):
    """Log API calls for debugging and monitoring"""
    log_data = {
        "timestamp": time.time(),
        "endpoint": endpoint,
        "method": method,
        "request_data": data,
        "response": response[:500] if response else None,
        "status_code": status_code
    }
    
    if status_code >= 400:
        logger.error(f"API Error: {json.dumps(log_data)}")
    else:
        logger.info(f"API Call: {json.dumps(log_data)}")

These examples provide practical, tested code for common WebLedger operations, covering all requirements specified in issue #20 including creating entries with different categories, querying by date range, error handling, and integration patterns.
