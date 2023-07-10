# WalletApi

WalletApi is an n-tier .NET Core application that provides an API for managing user wallets. It allows users to create wallets, deposit and withdraw funds, and view their transaction history. The application is built using the following technologies: [.NET Core, C#, SQL Server].

## Layers

The WalletApi application follows a typical n-tier architecture, consisting of the following layers:

1. **Wallet.Core**: This layer represents the business logic of the application, including the wallet management, transaction handling, and any other domain-specific operations.
2. **Wallet.Data**: This layer provides data access and persistence functionalities. It includes operations to interact with the database and perform CRUD operations on wallet-related entities.
3. **Wallet.Api**: This layer acts as the presentation layer and exposes the REST API endpoints for clients to interact with the application. It handles incoming requests, validates input, and orchestrates the execution of business logic.

## Prerequisites

To run WalletApi locally, you need to have the following installed on your machine:

- [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/)
- [SQL Server 2019](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Getting Started

To run WalletApi on your local machine, follow these steps:

1. Clone the repository: `git clone https://github.com/coommark/WalletApi.git`
2. Open the solution file (`WalletApi.sln`) in Visual Studio 2019.
3. Set up the database:
   - Open SQL Server Management Studio and connect to your SQL Server instance.
   - Create a new database for WalletApi.
   - Update the connection string in the `appsettings.json` file in the WalletApi project to point to your database.
4. Build the solution in Visual Studio.
5. Run the application by pressing `F5` or clicking the "Start" button in the toolbar.

## API Documentation

For detailed information about the available API endpoints and their usage, please refer to the [API documentation](./API.md).

## License

WalletApi is released under the [MIT License](./LICENSE).

## Contact

If you have any questions or suggestions regarding WalletApi, feel free to contact me.
