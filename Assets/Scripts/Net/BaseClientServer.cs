using System.Collections.Generic;
using System.Collections;


/// <summary>
/// Базовый класс для клиента и сервера
/// </summary>
public class BaseClientServer
{
    protected Queue<string> recievedDataQueue;
    protected Queue<string> sendDataQueue;
    protected byte[] dataBuffer = new byte[1024];
    public bool isUpdateStarted { get; protected set; }

    public BaseClientServer()
    {
        recievedDataQueue = new Queue<string>();
        sendDataQueue = new Queue<string>();
    }

    public void AddDataToSend(string _data)
    {
        sendDataQueue.Enqueue(_data);
    }

    public string GetRecievedData()
    {
        if (recievedDataQueue.Count == 0)
        {
            return string.Empty;
        }
        return recievedDataQueue.Dequeue();
    }

    public int CountRecievedData()
    {
        return recievedDataQueue.Count;
    }

    public virtual IEnumerator Update()
    {
        yield return null;
    }

    public void ClearDataBuffer()
    {
        dataBuffer = new byte[1024];
    }
}

