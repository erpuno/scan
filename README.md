SCAN: Scanning Agent
====================

`МІА: Сканування` is a simple Windows tray application as a scanning service for `МІА: Документообіг` clients.

![Screenshot](/Resources/screenshot.png)

It supports unread messages counter and native system notifications:

![Screenshot](/Resources/messaging.png)

Features
--------

* .NET Framework 4.6 Target for Zero Dependency
* TWAIN 32/64-bit protocol version 2.5
* Windows Tray Application
* System Notification
* Multiple Scan-Profile per data source
* Empira C# PDF SDK
* Kodak Alaris C# TWAIN SDK
* INFOTECH F# WebSocket server
* Xamarin C# MQTT client
* <a href="https://crm.erp.uno">МІА: Документообіг</a> Companion Application

Build
-----

Having Windows is the only prerequisite:

```
> C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe mia-agent.csproj
```

Connect
-------

Connect from JavaScript console:

```
> ws = new WebSocket("ws://127.0.0.1:50220")
> ws.onmessage = function (evt) { console.log(evt.data); }
> ws.send('PING')
```

Credits
-------

* Andrii Zadorozhnii
* Maksym Sokhatskyi
* Siegmentation Fault
