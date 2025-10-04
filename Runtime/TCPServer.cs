using GAG.EasyUIConsole;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace GAG.EasyTCP
{
    public class TCPServer : MonoBehaviour
    {
        public static TCPServer Instance;

        public static event Action<int, string, string> OnMessageReceived;
        public static void RaiseOnMessageReceived(int id, string name, string msg) => OnMessageReceived?.Invoke(id, name, msg);

        [SerializeField] IPAddress _ip;
        [SerializeField] int _port = 5000;

        TcpListener _server;
        Thread _listenerThread;
        readonly object clientLock = new object(); // for thread safety

        readonly List<ConnectedClient> clients = new List<ConnectedClient>();
        int _clientCounter = 0;

        bool _isRunning = true;

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
                return;
            }
        }

        void Start()
        {
            _ip = IPAddress.Any;
            _server = new TcpListener(_ip, _port);
            _server.Start();

            EasyUIConsoleManager.Instance.EasyLog($"Server started IP - {_ip} Port - {_port}");

            //TestLocalClientConnect();

            // Run listening in background thread
            _listenerThread = new Thread(ListenForClients);
            _listenerThread.IsBackground = true;
            _listenerThread.Start();
        }

        void TestLocalClientConnect()
        {
            var testClient = new TcpClient();
            testClient.Connect("127.0.0.1", 5000);
            EasyUIConsoleManager.Instance.EasyHiglight("Local test Client connected!");
        }

        void ListenForClients()
        {
            while (_isRunning)
            {
                try
                {
                    if (!_server.Pending()) { Thread.Sleep(10); continue; }

                    TcpClient client = _server.AcceptTcpClient();
                    var connectedClient = new ConnectedClient(client, ++_clientCounter);

                    lock (clientLock)
                    {
                        clients.Add(connectedClient);
                    }

                    // Start a thread for each client
                    Thread clientThread = new Thread(HandleClientCommunication);
                    clientThread.IsBackground = true;
                    clientThread.Start(connectedClient);

                    UnityMainThreadDispatcher.Enqueue(() =>
                    {
                        EasyUIConsoleManager.Instance.EasyHiglight($"TCP Client connected (ID: {connectedClient.Id})");
                    });

                }
                catch (Exception ex)
                {
                    EasyUIConsoleManager.Instance.EasyError("ListenForClients error: " + ex.Message);
                    break;
                }
            }
        }

        void HandleClientCommunication(object obj)
        {
            var connectedClient = (ConnectedClient)obj;
            TcpClient tcpClient = connectedClient.Client;
            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = new byte[1024];

            while (_isRunning && tcpClient.Connected)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead <= 0) break; // Client disconnected gracefully

                    string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (msg.StartsWith("NAME:"))
                    {
                        string newName = msg.Substring(5);
                        connectedClient.Name = newName;
                    }

                    UnityMainThreadDispatcher.Enqueue(() =>
                    {
                        EasyUIConsoleManager.Instance.EasyLog($"From Client {connectedClient.Name} {connectedClient.Id}: " + msg);
                        RaiseOnMessageReceived(connectedClient.Id, connectedClient.Name, msg);
                    });
                }
                catch
                {
                    break; // Client forcibly disconnected
                }
            }

            // ✅ Clean disconnect
            lock (clientLock)
            {
                clients.Remove(connectedClient);
            }
            tcpClient.Close();

            UnityMainThreadDispatcher.Enqueue(() =>
            {
                EasyUIConsoleManager.Instance.EasyWarning($"Client {connectedClient.Id} disconnected");
            });
        }

        public void SendMessageToAllClients(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            ThreadPool.QueueUserWorkItem(_ =>
            {
                lock (clientLock)
                {
                    foreach (var client in clients.ToArray()) // ToArray to avoid modification during iteration
                    {
                        if (client.Client.Connected)
                        {
                            try
                            {
                                NetworkStream stream = client.Client.GetStream();
                                stream.Write(data, 0, data.Length);
                                UnityMainThreadDispatcher.Enqueue(() =>
                                {
                                    EasyUIConsoleManager.Instance.EasyLog($"Sent to Client {client.Id}: {msg}");
                                });
                            }
                            catch (Exception e)
                            {
                                EasyUIConsoleManager.Instance.EasyError($"Send failed to Client {client.Id}: {e.Message}");
                                clients.Remove(client);
                                client.Client.Close();
                            }
                        }
                    }
                }
            });
        }

        public void SendMessageToClient(int clientId, string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                lock (clientLock)
                {
                    var client = clients.Find(c => c.Id == clientId);

                    if (client != null && client.Client.Connected)
                    {
                        try
                        {
                            NetworkStream stream = client.Client.GetStream();
                            stream.Write(data, 0, data.Length);

                            UnityMainThreadDispatcher.Enqueue(() =>
                            {
                                EasyUIConsoleManager.Instance.EasyLog($"Sent to Client {client.Id}: {msg}");
                            });
                        }
                        catch (Exception e)
                        {
                            EasyUIConsoleManager.Instance.EasyError($"Send failed to Client {client.Id}: {e.Message}");
                            clients.Remove(client);
                            client.Client.Close();
                        }
                    }
                }
            });
        }

        void OnApplicationQuit()
        {
            _isRunning = false;
            _server.Stop();
        }
    }
}
