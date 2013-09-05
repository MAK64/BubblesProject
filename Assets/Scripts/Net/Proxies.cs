// Файл содержит описание проксей общения клиента с сервером


/// <summary>
/// Общая обертка над всеми сообщениями
/// </summary>
public class NetMessage
{
    public int messType;
    public string message;
}

public class GameStateProxy
{
    public int level;
    public int score;
    public float timer;
    public NewBubbleProxy[] bubbles;
    public float bigBubbleSpeed;
    public float smallBubbleSpeed;
        
}

public class UserDestroyBubbleProxy
{
    public int id;
    public int resScore;

    public UserDestroyBubbleProxy()
    {
    }
    public UserDestroyBubbleProxy(int _id, int _resScore)
    {
        id = _id;
        resScore = _resScore;
    }
}

public class NewLevelProxy
{
    public int score;
    public NewBubbleProxy[] bubbles;
}

public class NewBubbleProxy
{
    public int id;
    public float posX;
    public float posY;
    public float size;
}

