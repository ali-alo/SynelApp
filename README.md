# SynelApp - CSV File Validator/Reader and Tabulator

A web application for uploading and validating a CSV file about Employees, and displaying the data in a tabular format using tabulator.js. This application is built with .NET 6; to access the deployed solution, please follow the link here https://ali-alo-synelapp.azurewebsites.net

## Table of Contents
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Tests](#tests)

## Features

- .NET Core 6 application.
- Upload CSV files for validation and display.
- Data validation using FluentValidation.
- Tabular display, filter, search and edit of data using tabulator.js.

## Prerequisites

Before you begin, ensure you have installed [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0).

## Getting Started

To get a local copy of this project up and running, follow these steps:

1. Clone the repository to your local machine:

   ```bash
   git clone https://github.com/ali-alo/SynelApp
   ```

2. Navigate to the app folder

    ```bash
    cd SynelApp/SynelApp
    ```

3. Install required dependencies

    ```bash
    dotnet restore
    ```

4. Set up the database

    ```bash
    dotnet ef database update
    ```

5. Run the app

    ```bash
    dotnet run
    ```

6. Access the app at http://localhost:5055

## Usage

1. Upload a CSV file using drag & drop or upload functionality.

2. The application will validate the CSV data and display it in a tabular format using tabulator.js.

3. Edit the fields of an employee directly inside the table, and the application will save changes.

### Important

Make sure your csv file has columns specified in the requirements data.csv file, otherwise the rows will not be processed. Refer to its reference below.

| Personnel_Records.Payroll_Number | Personnel_Records.Forenames | Personnel_Records.Surname | Personnel_Records.Date_of_Birth | Personnel_Records.Telephone | Personnel_Records.Mobile | Personnel_Records.Address | Personnel_Records.Address_2 | Personnel_Records.Postcode | Personnel_Records.EMail_Home | Personnel_Records.Start_Date |
|----------------------------------|-----------------------------|---------------------------|---------------------------------|-----------------------------|--------------------------|---------------------------|-----------------------------|----------------------------|------------------------------|------------------------------|
| COOP08                           | John                        | William                   | 26/01/1955                      | 12345678                    | 987654231                | 12 Foreman road           | London                      | GU12 6JW                   | nomadic20@hotmail.co.uk      | 18/04/2013                   |
| JACK13                           | Jerry                       | Jackson                   | 11/5/1974                       | 2050508                     | 6987457                  | 115 Spinney Road          | Luton                       | LU33DF                     | gerry.jackson@bt.com         | 18/04/2013                   |

## Tests

This project includes xUnit tests to ensure the functionality works correctly. To run the tests, use the following commands:

1. Navigate to the test folder

    ```bash
    cd ../SynelApp.Tests
    ```

2. Install required dependencies

    ```bash
    dotnet restore
    ```

3. Run the tests

    ```bash
    dotnet test
    ```
    