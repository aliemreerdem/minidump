# Memory Dump App

This is a C# console application that lists running processes on a Windows system, allows the user to filter them by name, and creates a memory dump file for the selected process.

## How to Use

1. Run the application as an administrator.
2. Enter a keyword or character combination to filter the process names (leave empty to list all processes).
3. The application will list the filtered processes along with their IDs and names.
4. Enter the ID of the process you want to create a memory dump for.
5. The application will create a memory dump file in the same directory as the executable with the format `<ProcessName>_<Timestamp>.dmp`.

## Requirements

- Windows operating system
- .NET runtime compatible with the target framework of the application (e.g., .NET 5.0 or .NET 6.0)

## Disclaimer

This application creates a memory dump with full memory information, which may contain sensitive data. Use it responsibly and ensure the dump files are stored securely.

## License

This project is released under the MIT License. See the [LICENSE](LICENSE) file for more information.
