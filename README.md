SCAN: Scanning Agent
====================

Simple Windows tray application as scanning service for intranet WebSocket clients.

![Screenshot](/Resources/screenshot.png)

Features
--------

* Windows Tray Application
* Zero-dependency
* Single file distribution 
* Small executable (300K)
* Kodak Alaris C# SDK
* TWAIN32 flavour for the sake of coverage
* Built-in F# WebSocket server
* Supports ERP.UNO CRM File Upload API

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

