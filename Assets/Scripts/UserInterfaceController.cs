using UnityEngine;

/// <summary>
/// Класс содержит весь интерфейс пользователя
/// </summary>
class UserInterfaceController : MonoBehaviour
{
    private static UserInterfaceController singleton = null;
    private InterfaceMode curMode = InterfaceMode.SplashScreen;

    Rect ipRect = new Rect(0, 0, 150, 50);

    void Awake()
    {
        singleton = this;
        if (ResourceManager.isResourcesLoaded)
        {
            OnResourasLoaded();
        }
        else
        {
            ResourceManager.OnAllResoucesLoadedEvent -= OnResourasLoaded;
            ResourceManager.OnAllResoucesLoadedEvent += OnResourasLoaded;
        }
        myIp = NetController.GetLocalIpAddress();
    }

    private void OnResourasLoaded()
    {
        ResourceManager.OnAllResoucesLoadedEvent -= OnResourasLoaded;

        #region Main menu rects
        newGameRect = Utils.GetRectOffsetCenterScreen(ResourceManager.userInterfaceDataHolder.newGameRect);
        watchNetGameRect = Utils.GetRectOffsetCenterScreen(ResourceManager.userInterfaceDataHolder.watchNetGameRect);
        #endregion

        #region Single Game menu rects
        buttomMountRect = Utils.GetRectOffsetButtomScreen(ResourceManager.userInterfaceDataHolder.buttomMountRect);
        buttomMountRect.width = Screen.width;
        scoreRect = Utils.SumRect(buttomMountRect, ResourceManager.userInterfaceDataHolder.scoreRect);
        levelRect = Utils.SumRect(buttomMountRect, ResourceManager.userInterfaceDataHolder.levelRect);
        levelRect.x = Utils.GetCenterScreenPos().x - levelRect.width / 2;
        pauseRect = Utils.SumRect(buttomMountRect, ResourceManager.userInterfaceDataHolder.pauseRect);
        pauseRect.x = buttomMountRect.width - ResourceManager.userInterfaceDataHolder.pauseRect.x;
        #endregion

        #region Net game rects
        ipAddressNameRect = Utils.GetRectOffsetCenterScreen(ResourceManager.userInterfaceDataHolder.ipAddressNameRect);
        ipAddressRect = Utils.GetRectOffsetCenterScreen(ResourceManager.userInterfaceDataHolder.ipAddressRect);
        connectRect = Utils.GetRectOffsetCenterScreen(ResourceManager.userInterfaceDataHolder.connectRect);
        backRect = Utils.GetRectOffsetCenterScreen(ResourceManager.userInterfaceDataHolder.backRect);
        #endregion

        #region Watch net game gui
        buttomMountRect = Utils.GetRectOffsetButtomScreen(ResourceManager.userInterfaceDataHolder.buttomMountRect);
        buttomMountRect.width = Screen.width;
        scoreRect = Utils.SumRect(buttomMountRect, ResourceManager.userInterfaceDataHolder.scoreRect);
        levelRect = Utils.SumRect(buttomMountRect, ResourceManager.userInterfaceDataHolder.levelRect);
        levelRect.x = Utils.GetCenterScreenPos().x - levelRect.width / 2;
        pauseRect = Utils.SumRect(buttomMountRect, ResourceManager.userInterfaceDataHolder.pauseRect);
        pauseRect.x = buttomMountRect.width - ResourceManager.userInterfaceDataHolder.pauseRect.x;
        #endregion
    }

    void OnGUI()
    {
        switch (curMode)
        {
            case InterfaceMode.SplashScreen:
                {
                    DrawSplashScreenGUI();
                    break;
                }
            case InterfaceMode.MainMenu:
                {
                    DrawMainMenuGUI();
                    break;
                }
            case InterfaceMode.NetSetup:
                {
                    DrawNetSetupGUI();
                    break;
                }
            case InterfaceMode.SingleGame:
                {
                    DrawSingleGameGUI();
                    break;
                }
            case InterfaceMode.WatchNetGame:
                {
                    DrawWatchNetGameGUI();
                    break;
                }
        }
    }

    public static void ChangeUserInterfaceMode(InterfaceMode _newMode)
    {
        singleton.curMode = _newMode;
    }

    Rect gameNameRect = new Rect(0,0,100,50);
    Rect screenRect = new Rect(0, 0, 0, 0);
    private void DrawSplashScreenGUI()
    {
        gameNameRect.x = Screen.width/2f - gameNameRect.width/2f;
        gameNameRect.y = Screen.height/2f;
        GUI.Label(gameNameRect, "Bubble rush");

        screenRect.width = Screen.width;
        screenRect.height = Screen.height;
        if (GUI.Button(screenRect, "",GUIStyle.none) && ResourceManager.isResourcesLoaded)
        {
            ChangeUserInterfaceMode(InterfaceMode.MainMenu);            
        }
        
    }
    
    Rect newGameRect;
    Rect watchNetGameRect;
    string myIp = string.Empty;
    private void DrawMainMenuGUI()
    {
        GUI.Label(ipRect, "Local ip:" + myIp);
        
        if (GUI.Button(newGameRect, "New game"))
        {
            GameController.StartGame();
            ChangeUserInterfaceMode(InterfaceMode.SingleGame);
        }

        if (GUI.Button(watchNetGameRect, "Watch net game"))
        {
            ChangeUserInterfaceMode(InterfaceMode.NetSetup);
        }
    }

    Rect buttomMountRect;
    Rect scoreRect;
    Rect pauseRect;
    Rect levelRect;
    private void DrawSingleGameGUI()
    {
        GUI.DrawTexture(buttomMountRect, ResourceManager.userInterfaceDataHolder.buttomMount);
        GUI.Label(scoreRect, "Score:" + GameController.score);
        GUI.Label(levelRect,string.Format( "Level:{0} Timer:{1}",GameController.curLevel,Mathf.FloorToInt(GameController.levelTimer)));

        if (GUI.Button(pauseRect, "Exit"))
        {
            GameController.StopGame();
            ChangeUserInterfaceMode(InterfaceMode.MainMenu);
        }
    }

    Rect ipAddressNameRect;
    Rect ipAddressRect;
    Rect connectRect;
    Rect backRect;
    string ipAddressInput = string.Empty;
    private void DrawNetSetupGUI()
    {
        GUI.Label(ipRect, "Local ip:" + myIp);

        GUI.Label(ipAddressNameRect, "Enter Ip:");
        ipAddressInput = GUI.TextField(ipAddressRect, ipAddressInput,12);

        if (GUI.Button(connectRect, "Connect") && ipAddressInput != string.Empty)
        {
            Debug.Log("Begin connect");
            NetController.ConnectToNetGame(ipAddressInput,ConfigDictionary.Config.socketConnectPort);
        }

        if (GUI.Button(backRect, "Back"))
        {
            ChangeUserInterfaceMode(InterfaceMode.MainMenu);
            GameController.StopGame();
        }
    }

    private void DrawWatchNetGameGUI()
    {
        GUI.DrawTexture(buttomMountRect, ResourceManager.userInterfaceDataHolder.buttomMount);
        GUI.Label(scoreRect, "Score:" + GameController.score);
        GUI.Label(levelRect, string.Format("Level:{0} Timer:{1}", GameController.curLevel, Mathf.FloorToInt(GameController.levelTimer)));

        if (GUI.Button(pauseRect, "Stop"))
        {
            ChangeUserInterfaceMode(InterfaceMode.MainMenu);
            GameController.StopNetWatchGame();
        }
    }
}

public enum InterfaceMode
{
    SplashScreen,
    MainMenu,
    NetSetup,
    SingleGame,
    WatchNetGame
}
