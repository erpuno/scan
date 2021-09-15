MIA: Агент Сканування
=====================

`МІА: Агент Сканування` is a simple Windows tray application as a scanning service for `МІА: Документообіг` clients.

![Screenshot](/Resources/screenshot.png)

It supports unread messages counter and native system notifications:

![Screenshot](/Resources/messaging.png)

Features
--------

The minimal OS supported is Windows 7 SP1. The minimal .NET Framework version embedded in each supported OS is 4.6.2.
The default target platform is set to x86 for TWAIN32 as setting to x64 will reduce the range of supported devices.

* Microsoft .NET Framework 4.6.2 Target (30 March 2016)
* TWAIN 32/64-bit protocol version 2.5
* Multiple Scan-Profiles per Data Source
* Acquiring with Duplex and Autoscan enabled as multipage PDF
* Delivery to Web Browser
* Windows Tray Application
* System Notifications
* <a href="https://crm.erp.uno">МІА: Документообіг</a> Companion Application

Articles
--------

We carefully noted each step of creating `МІА: Сканування`:

* [2021-01-04 F# WebSocket Server](https://tonpa.guru/stream/2021/2021-01-04%20F%23%20WebSocket%20Server.htm)
* [2021-08-09 TWAIN 2](https://tonpa.guru/stream/2021/2021-08-09%20TWAIN%202.htm)

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

Scanners
--------

* Epson DS-530
* Canon DR-C240
* Kodak E1035
* Kodak S2040
* <a href="https://sourceforge.net/projects/twain-samples/files/TWAIN%202%20Sample%20Data%20Source/TWAIN%20DS%202.1.3/">TWAIN Data Source FreeImage Virtual Scanner</a>

API
---

Connect from JavaScript console:

```
> ws = new WebSocket("ws://127.0.0.1:50220")
> ws.onmessage = function (evt) { console.log(evt.data); }
> ws.send('SCAN,DS-530,AUTOSCAN+AUTOFEED')
```

Systems
-------

* Microsoft Windows 10 (20H1,20H2,21H1), Windows 11 (21H2)
* Ubuntu Linux 20.10
* Apple macOS 11.5

Credits
-------

* Artem Sitalo
* Andrii Zadorozhnii
* Maksym Sokhatskyi
* Denys Shkurko
* Siegmentation Fault
