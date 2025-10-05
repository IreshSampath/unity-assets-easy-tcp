# 🔌 EasyTCP for Unity

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-green.svg)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Mac%20%7C%20Linux%20%7C%20Android%20%7C%20iOS-lightgrey.svg)
![License](https://img.shields.io/badge/License-MIT-blue.svg)

---

## 🚀 Overview

**EasyTCP** is a lightweight Unity networking package designed to simplify **TCP communication** between applications.  
It provides both **Server** and **Client** functionality using **background threads** and **async/await**, ensuring smooth performance without blocking the Unity main thread.

Perfect for:

- ✅ **Multiplayer prototyping**
- ✅ **Remote debugging / telemetry**
- ✅ **Cross-app communication (PC ↔ Mobile ↔ Embedded)**
- ✅ **IoT/game integrations**


---

## 🎮 EasyTCP Demo (Download & Play)

Want to see EasyTCP in action?  
Download and test the demo builds here:

👉 **https://gameartgames.itch.io/easytcp-demo**

[![EasyTCP Demo](https://img.shields.io/badge/Play_on-Itch.io-orange?logo=itchdotio)](https://gameartgames.itch.io/easytcp-demo)

---

## 📦 Installation

### Option A — Install via Unity Package Manager (Git URL)

1. Open **Unity → Window → Package Manager**
2. Click **+** → **Add package from Git URL**
3. Paste:
   https://github.com/IreshSampath/unity-assets-easy-tcp.git
4. Click **Install**
5. Click Import → Samples → Server Sample or Client Sample.
6. Click **+** → **Add package from Git URL**
7. Paste:
   https://github.com/IreshSampath/unity-assets-easy-ui-console.git
8. Click **Install**
9. Click Import → Samples → EasyUIConsole Sample

---

## 🧰 Quick Start

### ✅ Check Samples 

1. Open the TCP Server scene or the TCP Client scene.
2. Check the Handlers scripts (TCPServerHandler.cs/TCPClientHandler.cs).
3. Server-side, IP is any, and Port 5000 is already set up.
4. Client-side, Need to save Server IP and Port


### ✅ Server Setup → Check the "TCPServerHandler.cs"

```csharp
// Receive messages from clients
void OnEnable()
{
    TCPServer.OnMessageReceived += ReceiveMessage;
}

void ReceiveMessage(int id, string name, string msg)
{
    EasyUIConsoleManager.Instance.EasyHiglight($"{id} {name} {msg}");
}

// Send to all connected clients
TCPServer.Instance.SendMessageToAllClients("Hello Clients!");

// Send to a specific client
TCPServer.Instance.SendMessageToClient(clientId, "Private Message");
```

### ✅ Client Setup → Check the "TCPClientHandler.cs"

```csharp
//Need to save Server IP and Port
PlayerPrefs.SetString("ServerIP", ipAddress.ToString());
PlayerPrefs.SetInt("Port", portNumber);

// Connect to the server
TCPClient.Instance.ConnectServer();

// Disconnect when done
TCPClient.Instance.DisconnectServer();

// Receive messages from server
void OnEnable()
{
    TCPServer.OnMessageReceived += ReceiveMessage;
}

void ReceiveMessage(string msg)
{
    EasyUIConsoleManager.Instance.EasyHiglight(msg);
}

// Send messages to server
TCPClient.Instance.SendMessageToServer("Hello Server!");


```
---

⚠️ **IMPORTANT: READ BEFORE CONNECTING!**

To avoid connection failures, make sure:

✅ **Server and Client are on the SAME NETWORK**  
    • Connected to the same Wi-Fi / LAN router  
    • Hotspot / corporate networks may block local access

✅ **Firewall is NOT Blocking Your App (Windows Users)**  
    • Disable Firewall temporarily, or  
    • Add an Allow Rule for your TCP app

✅ **Router Must Allow Device-to-Device Communication**  
    • Some routers block internal ports — change to a home network if needed

✅ **Double-check IP & Port Values**  
    • A single typo can stop the connection!

🟢 **Once all of the above are ✅, you’re good to go!**

---

## 🧪 Samples Included

| Sample                  | Description                                                       |
| ----------------------- | ----------------------------------------------------------------- |
| ✅ EasyTCP Server Sample | Demonstrates accepting multiple clients and broadcasting messages |
| ✅ EasyTCP Client Sample | Shows how to connect, send, and listen for messages               |

---

## ⚙️ Technical Details

| Feature                  | Implementation                            |
| ------------------------ | ----------------------------------------- |
| Threading Model          | Background Threads + Async/Await          |
| Communication            | TCP (Transmission Control Protocol)       |
| Encoding                 | UTF-8 (string-based messaging)            |
| Supported Platforms      | **Windows / Mac / Linux / Android / iOS** |
| UI Dependency (Optional) | EasyUIConsole (for visual debug logs)     |

---

## 📜 License
This project is licensed under the MIT License — free for commercial and personal use.

---

## 🙏 Thank You
Thanks for using EasyTCP!
- Feel free to contribute, report bugs, or request new features.

---

## 👤 Author
Iresh Sampath 🔗 [LinkedIn Profile](https://www.linkedin.com/in/ireshsampath/)
