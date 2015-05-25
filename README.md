# TDSBridge

## Description 
A C# library to implement a SQL Server TDS protocol bridge with packet inspection. It basically relays TDS messages to a SQL server and sends the answer back; it's almost completely transparent to applications (it even works with server side forced encryption). The library right now recognizes the AttentionMessage, RPCRequestMessage and SQLBatchMessage. Everything else falls into the generic TDSMessage class. I will add more specialized classes in the future. It comes with a simple console application that can be useful for didactic purposes, but I plan to develop a more elegant interactive console. Developed following the [MS-TDS]: Tabular Data Stream Protocol specification version 12.0. 

Reference:
[MS-TDS] Tabular Data Stream Protocol [http://msdn.microsoft.com/en-us/library/dd304523.aspx](http://msdn.microsoft.com/en-us/library/dd304523.aspx).

Here is a screenshot of the sample console inspector. As you can see, even though I connected to the bridge I can still access SQL Server as normal. You can also see the SQL Batch statement inspected by the library:

![](http://i.imgur.com/FKiuVgb.png)

```sql
SELECT @@SERVERNAME;
SELECT * FROM sys.dm_exec_connections;
```

To use the sample, please specify the listening port (must be available) and the SQL Server TCP endpoint (no browser support right now). For example if I have a SQL Instance running on my PC listening on 1433 this could be the syntax:

![](http://i.imgur.com/4xCWRo2.png)


The resulting connections could be like this diagram:

![](http://i.imgur.com/yV6SgbK.png)

If you need to parse the output you can redirect it to a file (as you will see, SSMS intellisense will bloat your window :) ).

Please feel free to use it in your demo/classes and if you need any help don't hesitate to contact me.
