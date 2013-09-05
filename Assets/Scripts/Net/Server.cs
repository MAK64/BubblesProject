using UnityEngine;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;

/// <summary>
/// Содержит весь код сервера
/// </summary>
public class Server : BaseClientServer
{
    private int localPort;
    private IPAddress localIp;
    TcpListener listener;
    Socket socket;
    
    public void StartServer(string _ipAddress, int port)
    {
        localPort = port;
        if (IPAddress.TryParse(_ipAddress,out localIp))
        {
            listener = new TcpListener(localIp, localPort);
            listener.Start();
        }
        else
        {
                Debug.LogError("Ip not valid");
        }
        
    }

    public void StopServer()
    {
        if (socket != null)
        {
            socket.Close();
            socket = null;
        }
        listener.Stop();
        listener = null;
    }


    public override IEnumerator Update()
    {
        isUpdateStarted = true;
        if (socket != null && socket.Connected)
        {
            try
            {
                //принимаем сообщения с клиента
                if (socket.Available > 0)
                {
                    int receive = socket.Receive(dataBuffer);
                    if (receive > 0)
                    {
                        string recievedString = Encoding.ASCII.GetString(dataBuffer);
                        Debug.Log("Server recieve " + recievedString);
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

            try
            {
                //Посылаем сообщение на клиент                 
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
        else
        {
            try
            {
                if ( listener.Pending())
                {
                    socket = listener.AcceptSocket();
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

