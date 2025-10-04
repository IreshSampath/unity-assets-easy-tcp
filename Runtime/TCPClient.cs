using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System;
using GAG.EasyUIConsole;

namespace GAG.EasyTCP
{
    public class TCPClient : MonoBehaviour
    {
        public static TCPClient Instance;

        public static event Action<string> OnMessageReceived;
        public static void RaiseOnMessageReceived(string msg) => OnMessageReceived?.Invoke(msg);

        TcpClient _client;
        NetworkStream _stream;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async void ConnectServer()
        {
            _client = new TcpClient();

            try
            {
                string ip = PlayerPrefs.GetString("ServerIP", "No saved IP");
                int port = PlayerPrefs.GetInt("Port", 5000);

                EasyUIConsoleManager.Instance.EasyWarning($"Trying to connect {ip}:{port} ...");

                await _client.ConnectAsync(ip, port); // Attempt connection

                if (_client.Connected)
                {
                    _stream = _client.GetStream();

                    EasyUIConsoleManager.Instance.EasyHiglight("Connected to Server!");

                    // Start listening for server messages
                    StartListening();
                }
                else
                {
                    EasyUIConsoleManager.Instance.EasyError("Connection failed. Server not ready.");
                }
            }
            catch (SocketException ex)
            {
                // This happens when server is offline / firewall blocked / wrong IP
                EasyUIConsoleManager.Instance.EasyError("Server not ready. server is offline / firewall blocked / wrong IP");
                EasyUIConsoleManager.Instance.EasyError(ex.Message);
            }
            catch (System.Exception ex)
            {
                EasyUIConsoleManager.Instance.EasyError("Unexpected error: " + ex.Message);
            }
        }

        public void DisconnectServer()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
            EasyUIConsoleManager.Instance.EasyError("Disconnected from server.");
        }

        async void StartListening()
        {
            byte[] buffer = new byte[1024];

            while (_client != null && _client.Connected)
            {
                try
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        EasyUIConsoleManager.Instance.EasyWarning("Disconnected from server.");

                        break; // server closed connection
                    }

                    string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    ReceiveMessageFromServer(msg);
                }
                catch (System.Exception ex)
                {
                    EasyUIConsoleManager.Instance.EasyError("Error while reading from server: " + ex.Message);

                    break;
                }
            }
        }

        void ReceiveMessageFromServer(string msg)
        {
            EasyUIConsoleManager.Instance.EasyHiglight("Server message: " + msg);

            RaiseOnMessageReceived(msg);
        }

        public void SendMessageToServer(string msg)
        {
            if (_stream == null)
            {
                EasyUIConsoleManager.Instance.EasyError("Cannot send message. Not connected to server.");

                return;
            }
            EasyUIConsoleManager.Instance.EasyLog("Sending: " + msg);

            byte[] data = Encoding.UTF8.GetBytes(msg);
            _stream.Write(data, 0, data.Length);
        }
    }
}
