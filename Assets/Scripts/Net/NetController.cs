using UnityEngine;
using System.Net;
using JsonFx.Json;

/// <summary>
/// Класс отвечает за все клиент-серверное соединение
/// </summary>
public class NetController : MonoBehaviour
{
    private static NetController singleton = null;

    private Server server;
    private Client client;

    void Awake()
    {
        singleton = this;
    }

    void Update()
    {
        if (server != null && !server.isUpdateStarted)
        {
            StartCoroutine(server.Update());
        }

        if (client != null && !client.isUpdateStarted)
        {
            StartCoroutine(client.Update());
        }

        if (server != null)
        {
            ProcessRecieveMessages(server);
        }
        else if (client != null)
        {
            ProcessRecieveMessages(client);
        }
    }

    public static void CreateServer()
    {
        singleton.server = new Server();
        singleton.server.StartServer(GetLocalIpAddress(),ConfigDictionary.Config.socketConnectPort);
        Debug.Log("Server Created");
    }

    public static void StopServer()
    {
        if (singleton.server != null)
        {
            singleton.server.StopServer();
            singleton.server = null;
        }
        Debug.Log("Server stoped");
    }


    public static string GetLocalIpAddress()
    {
        foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            return ip.ToString();
        }
        return string.Empty;
    }

    public static void ConnectToNetGame(string _ip, int _port)
    {
        singleton.client = new Client();
        if (singleton.client.Connect(_ip, _port))
        {
            SendMessage(NetMessageCode.RequestGameState, null);
        }

    }

    public static void DisconnectNet()
    {
        if (singleton.client != null)
        {
            singleton.client.Disconnect();
            singleton.client = null;
        }

    }

    string curMessage = string.Empty;
    private void ProcessRecieveMessages(BaseClientServer clientServer)
    {
        for (int i = 0; i < clientServer.CountRecievedData(); i++)
        {
            curMessage = clientServer.GetRecievedData();
            if (curMessage == string.Empty)
            {
                continue;
            }
            NetMessage netMessage = JsonReader.Deserialize<NetMessage>(curMessage);
            if (netMessage != null)
            {
                switch ((NetMessageCode) netMessage.messType)
                {
                    case NetMessageCode.RequestGameState:
                    {
                        SendMessage(NetMessageCode.GetGameState, null);
                        break;
                    }
                    case NetMessageCode.GetGameState:
                    {
                        GameStateProxy proxy = JsonReader.Deserialize<GameStateProxy>(netMessage.message);
                        GameController.StartNetWatchGame(proxy);
                        break;
                    }
                    case NetMessageCode.NewBubble:
                    {
                        if (GameController.curGameMode == GameMode.netView)
                        {
                            NewBubbleProxy proxy = JsonReader.Deserialize<NewBubbleProxy>(netMessage.message);
                            GameController.CreateNetWatchBubble(proxy);
                        }
                        break;
                    }
                    case NetMessageCode.UserDestroyBubble:
                    {
                        if (GameController.curGameMode == GameMode.netView)
                        {
                            UserDestroyBubbleProxy proxy = JsonReader.Deserialize<UserDestroyBubbleProxy>(netMessage.message);
                            GameController.DestroyNetWatchBuble(proxy);
                        }
                        break;
                    }
                    case NetMessageCode.NewLevel:
                    {
                        if (GameController.curGameMode == GameMode.netView)
                        {
                            NewLevelProxy proxy = JsonReader.Deserialize<NewLevelProxy>(netMessage.message);
                            GameController.NewNetWatchLevel(proxy);
                        }
                        break;
                    }
                }
            }
        }

    }

    public static void SendMessage(NetMessageCode mesCode, object message)
    {
        NetMessage newMessage = new NetMessage();
        newMessage.messType = (int)mesCode;
        switch (mesCode)
        {
            case NetMessageCode.RequestGameState:
                {
                    newMessage.message = string.Empty;
                    break;
                }
            case NetMessageCode.GetGameState:
                {
                    newMessage.message = JsonWriter.Serialize(GameController.GetGameState());
                    break;
                }
            case NetMessageCode.NewBubble:
                {
                    newMessage.message = JsonWriter.Serialize((NewBubbleProxy)message);
                    break;
                }
            case NetMessageCode.UserDestroyBubble:
                {
                    newMessage.message = JsonWriter.Serialize((UserDestroyBubbleProxy)message);
                    break;
                }
            case NetMessageCode.NewLevel:
                {
                    newMessage.message = JsonWriter.Serialize((NewLevelProxy)message);
                    break;
                }
        }
        if (singleton.server != null)
        {
            string messageString = JsonWriter.Serialize(newMessage);
            //Debug.Log("Send:" + messageString);
            singleton.server.AddDataToSend(messageString);
        }
        if (singleton.client != null)
        {
            string messageString = JsonWriter.Serialize(newMessage);
            //Debug.Log("Send:" + messageString);
            singleton.client.AddDataToSend(messageString);
        }
    }

    
}

public enum NetMessageCode
{
    GetGameState = 1,
    RequestGameState = 2,
    NewLevel = 3,
    NewBubble = 4,
    UserDestroyBubble = 5,
    
}
