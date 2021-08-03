SCAN: Scanning Agent
====================

`МІА: Сканування` is a simple Windows tray application as a scanning service for `МІА: Документообіг` clients.

![Screenshot](/Resources/screenshot.png?v=1)

Features
--------

* Windows Tray Application
* Zero-dependency
* Single file distribution 
* Small executable (300K)
* Kodak Alaris C# SDK
* TWAIN32 flavour for the sake of coverage
* Built-in F# WebSocket server
* Supports <a href="https://crm.erp.uno">МІА: Документообіг</a>

Build
-----

Having Windows is the only prerequisite:

```
> C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe mia-scan.csproj
```

With intranet Web-Server support:

```
> dotnet build
```

Credits
-------

* Andrii Zadorozhnii
* Maksym Sokhatskyi

