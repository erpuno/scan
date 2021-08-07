SCAN: Scanning Agent
====================

`МІА: Сканування` is a simple Windows tray application as a scanning service for `МІА: Документообіг` clients.

![Screenshot](/Resources/screenshot.png)

It supports unread messages counter and native system notifications:

![Screenshot](/Resources/messaging.png)

Features
--------

* Microsoft .NET Framework 4.6 Target (all Windows' after 2015)
* TWAIN 32/64-bit protocol version 2.5
* Multiple Scan-Profiles per Data Source
* Acquire with Duplex and Autoscan enabled as multipage PDF
* Deliver to Web Browser
* Windows Tray Application
* System Notifications
* <a href="https://crm.erp.uno">МІА: Документообіг</a> Companion Application

Dependencies
------------

* Kodak Alaris C# TWAIN SDK (256KB)
* Empira C# PDF SDK (700KB)
* INFOTECH F# WebSocket server (64KB)
* Xamarin C# MQTT client (160KB)

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
