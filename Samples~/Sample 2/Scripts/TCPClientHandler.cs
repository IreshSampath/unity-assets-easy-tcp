using System.Net;
using TMPro;
using UnityEngine;
using GAG.EasyUIConsole;
using GAG.EasyTCP;

public class TCPClientHandler : MonoBehaviour
{
    public static TCPClientHandler Instance;

    public TMP_InputField IPInput;
    public TMP_InputField PortInput;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //PlayerPrefs.DeleteAll(); // For testing purpose only
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void OnEnable()
    {
        TCPClient.OnMessageReceived += ReceiveMessage;
    }
    void OnDisable()
    {
        TCPClient.OnMessageReceived -= ReceiveMessage;
    }

    void Start()
    {
        if (PlayerPrefs.GetString("ServerIP") == "" || PlayerPrefs.GetInt("Port") == 0)
        {
            EasyUIConsoleManager.Instance.EasyError("No IP or Port saved. Please set them in the UI.");
            return;
        }

        ConnectServer();
    }

    public void LoadNetworkData()
    {
        IPInput.text = PlayerPrefs.GetString("ServerIP", "No Saved IP");
        PortInput.text = PlayerPrefs.GetInt("Port", 5000).ToString();
    }

    public void SaveServerConfigurations()
    {
        string ip = IPInput.text?.Trim();
        string port = PortInput.text?.Trim();

        if (!string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(port))
        {
            if (IPAddress.TryParse(ip, out IPAddress ipAddress))
            {
                PlayerPrefs.SetString("ServerIP", ipAddress.ToString());

                if (int.TryParse(port, out int portNumber))
                {
                    PlayerPrefs.SetInt("Port", portNumber);

                    ConnectServer();
                }
                else
                {
                    EasyUIConsoleManager.Instance.EasyError("Invalid port number");
                }
            }
            else
            {
                if (!IPAddress.TryParse(ip, out _))
                {
                    EasyUIConsoleManager.Instance.EasyError("Invalid IP address");
                }
            }
        }
        else
        {
            EasyUIConsoleManager.Instance.EasyError("IP or Port is empty");
        }
    }

    public void ConnectServer()
    {
        TCPClient.Instance.ConnectServer();
    }

    public void DisconnectServer()
    {
        TCPClient.Instance.DisconnectServer();
    }

    public void SendMessageToServer(string msg)
    {
        TCPClient.Instance.SendMessageToServer(msg);
    }

    void ReceiveMessage(string msg)
    {
        EasyUIConsoleManager.Instance.EasyHiglight(msg);
    }
}
