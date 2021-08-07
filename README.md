SCAN: Scanning Agent
====================

`МІА: Сканування` is a simple Windows tray application as a scanning service for `МІА: Документообіг` clients.

![Screenshot](/Resources/screenshot.png)

It supports unread messages counter and native system notifications:

![Screenshot](/Resources/messaging.png)

Features
--------

* .NET Framework 4.8 Target for Zero Dependency
* TWAIN32 flavour for legacy scanners support
* Windows Tray Application
* System Notification
* Multiple Scan-Profile per data source
* Small Executable (300K)
* Kodak Alaris C# TWAIN SDK
* INFOTECH F# WebSocket server (port 50220)
* Xamarin MQTT client
* Supports <a href="https://crm.erp.uno">МІА: Документообіг</a>

Build
-----

Having Windows is the only prerequisite:

```
> C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe mia-scan.csproj
```

Credits
-------

* Andrii Zadorozhnii
* Maksym Sokhatskyi
* Siegmentation Fault
