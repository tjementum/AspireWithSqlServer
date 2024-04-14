# Connection to SQL Server database started using Aspire Host fails on macOS

This project demonstrates how to use .NET Aspire with SQL Server and Entity Framework. Specifically, it shows how to workaround a bug in SQLClient that, when used together with .NET Aspire, fails with strange exceptions if the connection to the Database is made before the SQL server is fully started. See this [GitHub issue on the Aspire project](https://github.com/dotnet/aspire/issues/1023).

## The Error:

```
Microsoft.Data.SqlClient.SqlException (0x80131904): A connection was successfully established with the server, but then an error occurred during the pre-login handshake. (provider: TCP Provider, error: 0 - 20)
```

<details>

<summary>See the full stack trace</summary>

```
Microsoft.Data.SqlClient.SqlException (0x80131904): A connection was successfully established with the server, but then an error occurred during the pre-login handshake. (provider: TCP Provider, error: 0 - 20)
---> System.ComponentModel.Win32Exception (203): Unknown error: 203
    at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
    at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
    at Microsoft.Data.SqlClient.TdsParserStateObject.ThrowExceptionAndWarning(Boolean callerHasConnectionLock, Boolean asyncClose)
    at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniError(TdsParserStateObject stateObj, UInt32 error)
    at Microsoft.Data.SqlClient.TdsParserStateObject.ReadSniSyncOverAsync()
    at Microsoft.Data.SqlClient.TdsParserStateObject.TryReadNetworkPacket()
    at Microsoft.Data.SqlClient.TdsParser.ConsumePreLoginHandshake(SqlConnectionEncryptOption encrypt, Boolean trustServerCert, Boolean integratedSecurity, Boolean& marsCapable, Boolean& fedAuthRequired, Boolean tlsFirst, String serverCert)
    at Microsoft.Data.SqlClient.TdsParser.Connect(ServerInfo serverInfo, SqlInternalConnectionTds connHandler, Boolean ignoreSniOpenTimeout, Int64 timerExpire, SqlConnectionString connectionOptions, Boolean withFailover)
    at Microsoft.Data.SqlClient.SqlInternalConnectionTds.AttemptOneLogin(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean ignoreSniOpenTimeout, TimeoutTimer timeout, Boolean withFailover)
    at Microsoft.Data.SqlClient.SqlInternalConnectionTds.LoginNoFailover(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
    at Microsoft.Data.SqlClient.SqlInternalConnectionTds.OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance)
    at Microsoft.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, Boolean applyTransientFaultHandling, String accessToken, DbConnectionPool pool)
    at Microsoft.Data.SqlClient.SqlConnectionFactory.CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, Object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningConnection, DbConnectionOptions userOptions)
    at Microsoft.Data.ProviderBase.DbConnectionFactory.CreatePooledConnection(DbConnectionPool pool, DbConnection owningObject, DbConnectionOptions options, DbConnectionPoolKey poolKey, DbConnectionOptions userOptions)
    at Microsoft.Data.ProviderBase.DbConnectionPool.CreateObject(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection)
    at Microsoft.Data.ProviderBase.DbConnectionPool.UserCreateRequest(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection)
    at Microsoft.Data.ProviderBase.DbConnectionPool.TryGetConnection(DbConnection owningObject, UInt32 waitForMultipleObjectsTimeout, Boolean allowCreate, Boolean onlyOneCheckConnection, DbConnectionOptions userOptions, DbConnectionInternal& connection)
    at Microsoft.Data.ProviderBase.DbConnectionPool.TryGetConnection(DbConnection owningObject, TaskCompletionSource`1 retry, DbConnectionOptions userOptions, DbConnectionInternal& connection)
    at Microsoft.Data.ProviderBase.DbConnectionFactory.TryGetConnection(DbConnection owningConnection, TaskCompletionSource`1 retry, DbConnectionOptions userOptions, DbConnectionInternal oldConnection, DbConnectionInternal& connection)
    at Microsoft.Data.ProviderBase.DbConnectionInternal.TryOpenConnectionInternal(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource`1 retry, DbConnectionOptions userOptions)
    at Microsoft.Data.ProviderBase.DbConnectionClosed.TryOpenConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource`1 retry, DbConnectionOptions userOptions)
    at Microsoft.Data.SqlClient.SqlConnection.TryOpen(TaskCompletionSource`1 retry, SqlConnectionOverrides overrides)
    at Microsoft.Data.SqlClient.SqlConnection.Open(SqlConnectionOverrides overrides)
    at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerConnection.OpenDbConnection(Boolean errorsExpected)
    at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenInternal(Boolean errorsExpected)
    at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.Open(Boolean errorsExpected)
    at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerDatabaseCreator.<>c__DisplayClass18_0.<Exists>b__0(DateTime giveUp)
    at Microsoft.EntityFrameworkCore.ExecutionStrategyExtensions.<>c__DisplayClass12_0`2.<Execute>b__0(DbContext _, TState s)
    at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.Execute[TState,TResult](TState state, Func`3 operation, Func`3 verifySucceeded)
    at Microsoft.EntityFrameworkCore.ExecutionStrategyExtensions.Execute[TState,TResult](IExecutionStrategy strategy, TState state, Func`2 operation, Func`2 verifySucceeded)
    at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerDatabaseCreator.Exists(Boolean retryOnNotExists)
    at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerDatabaseCreator.Exists()
    at Microsoft.EntityFrameworkCore.Migrations.HistoryRepository.Exists()
    at Microsoft.EntityFrameworkCore.Migrations.Internal.Migrator.Migrate(String targetMigration)
    at Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.Migrate(DatabaseFacade databaseFacade)
    at AspireWithSqlServer.WebApi.Persistence.EntityFrameworkExtensions.ApplyMigrations(IServiceProvider services) in /Users/thomasjespersen/Developer/Other/AspireWithSqlServer/WebApi/Persistence/EntityFrameworkExtensions.cs:line 33
ClientConnectionId:5f3eeede-a47c-473b-8aaf-f883ccdfee7f
Error Number:203,State:0,Class:20
```

</details>

## This is what i looks like

<img src="AspireConnectionError.gif" alt="Aspire connection to SQL Server">

## Reproduce

1. Clone the project to you local machine
2. Run these commands:
   ```bash
   cd ./AppHost
   dotnet restore
   dotnet run
   ```

Browse to [http://127.0.0.1:15042/ProjectLogs](http://127.0.0.1:15042/ProjectLogs). This is the Aspire log for the WebApi project.

Also notice that the [Swagger UI](http://localhost:5202/swagger/index.html) does not work.

## Workaround

Keep the Aspire AppHost running and start a new terminal

```bash
cd ./WebApi
dotnet run
```

Ignore the error `Hosting failed to start. System.IO.IOException: Failed to bind to address http://127.0.0.1:5202: address already in use.`.

Notice that once the WebApi starts, the [Swagger UI](http://localhost:5202/swagger/index.html) now works.

## Prerequisites

- Docker Desktop
- .NET 8
- .NET Aspire (`dotnet workload install aspire`)

## Environment

Tested on this environment:

- MacBook Pro M1 Max
- macOS Sonoma 14.1.1
