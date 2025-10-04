using UnityEngine;
using GAG.EasyUIConsole;
using GAG.EasyTCP;

public class TCPServerHandler : MonoBehaviour
{
    void OnEnable()
    {
        TCPServer.OnMessageReceived += ReceiveMessage;
    }

    void OnDisable()
    {
        TCPServer.OnMessageReceived -= ReceiveMessage;
    }

    public void TestSendMessageByID(int id)
    {
        SendMessageToClient(id, $"Hello Client {id}, This is only for you!");
    }

    public void SendMessageToAllClients(string msg)
    {
        TCPServer.Instance.SendMessageToAllClients(msg);
    }

    public void SendMessageToClient(int clientId, string msg)
    {
        TCPServer.Instance.SendMessageToClient(clientId, msg);
    }

    void ReceiveMessage(int id, string name, string msg)
    {
        EasyUIConsoleManager.Instance.EasyHiglight($"{id} {name} {msg}");
    }
}
