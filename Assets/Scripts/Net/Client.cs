using UnityEngine;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Содержит весь код клиента
/// </summary>
public class Client : BaseClientServer
{
    TcpClient clientTCP;
    Socket socket;
    private const int maxAttemptToConnect = 10;

    public Client()
    {
        clientTCP = new TcpClient();
    }

    public bool Connect(string ip, int port)
    {
        for (int i = 0; i < maxAttemptToConnect; i++)
        {
            try
            {
                clientTCP.Connect(ip, port);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
            if (!clientTCP.Connected) continue;
            socket = clientTCP.Client;
            return true;
        }
        return false;
    }

    public void Disconnect()
    {
        if (socket != null)
        {
            socket.Close();
        }
        clientTCP.Close();
        clientTCP = null;
    }

    public override IEnumerator Update()
    {
        isUpdateStarted = true;
        if (clientTCP.Connected)
        {
            //Принимаем данные
            try
            {
                if (socket.Available > 0)
                {
                    int recieve = socket.Receive(dataBuffer);
                    if (recieve > 0)
                    {
                        string recievedString = Encoding.ASCII.GetString(dataBuffer);
                        Debug.Log("Client recieve " + recievedString);
                        if (recievedString != string.Empty)
                        {
                            recievedDataQueue.Enqueue(recievedString);
                            ClearDataBuffer();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            //Посылаем данные
            try
            {
                while (sendDataQueue.Count > 0)
                {
                    socket.Send(Encoding.ASCII.GetBytes(sendDataQueue.Dequeue()));
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        yield return new WaitForEndOfFrame();
        isUpdateStarted = false;
    }
}

