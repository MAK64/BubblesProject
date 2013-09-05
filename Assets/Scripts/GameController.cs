using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

/// <summary>
/// Класс отвечает за всю игровую логику, содержит 2 режима игры(саму игру и режим наблюдателя)
/// </summary>
public class GameController : MonoBehaviour
{
    private static GameController singleton = null;

    private GameMode _curGameMode = GameMode.none;
    public static GameMode curGameMode
    {
        get
        {
            return singleton._curGameMode;
        }

    }

    private Dictionary<int, BubbleController> bubblesDict = new Dictionary<int, BubbleController>();
        
    private int _score = 0;
    public static int score
    {
        get { return singleton._score; }
    }

    private BubblesGenerator bubblesGenerator;

    public static float maxBubbleSize
    {
        get { return singleton.bubblesGenerator.maxBubbleSize; }
    }

    [SerializeField]
    private float _levelTimer;
    public static float levelTimer
    {
        get { return singleton._levelTimer; }
    }

    [SerializeField]
    private float smallBubbleSpeed = 0f;
    [SerializeField]
    private float bigBubbleSpeed = 0f;

    [SerializeField]
    private float bubbleGenerationDelay = 0f;

    [SerializeField]
    private int _curLevel = 1;
    public static int curLevel
    {
        get { return singleton._curLevel; }
    }

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
    }

    private void OnResourasLoaded()
    {
        ResourceManager.OnAllResoucesLoadedEvent -= OnResourasLoaded;
        bubblesGenerator = new BubblesGenerator();
        bubblesGenerator.Setup();
    }

    public static void StartGame()
    {
        NetController.CreateServer();        
        singleton.DestroyAllBubblesInDict();
        singleton._curGameMode = GameMode.single;
        singleton._curLevel = 1;
        singleton._score = 0;
        ResourceManager.GenerateNewTexturesSet();
        singleton.smallBubbleSpeed = ConfigDictionary.Config.baseSmallBubbleSpeed;
        singleton.bigBubbleSpeed = ConfigDictionary.Config.baseBigBubbleSpeed;
        singleton._levelTimer = ConfigDictionary.Config.levelTime;
        singleton.bubbleGenerationDelay = ConfigDictionary.Config.baseBubbleGenerationDelay;
        singleton.StartCoroutine(singleton.GenerateBubblesForSingleGame());
    }

    public static void StopGame()
    {
        singleton.StopAllCoroutines();
        NetController.StopServer();
        singleton.DestroyAllBubblesInDict();
        singleton._curGameMode = GameMode.none;
    }

    void Update()
    {
        _levelTimer -= Time.deltaTime;
        if (_levelTimer < 0) _levelTimer = 0;
    }

    IEnumerator GenerateBubblesForSingleGame()
    {   
        if (_levelTimer == 0)
        {
            NextLevel();
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(GenerateBubblesForSingleGame());
        }
        else
        {
            BubbleController newBubble = bubblesGenerator.GenerateNewBubble(smallBubbleSpeed, bigBubbleSpeed);
            NetController.SendMessage(NetMessageCode.NewBubble, newBubble.GenerateNewBubbleProxy());
            bubblesDict.Add(newBubble.id, newBubble);
            float nextBubbleTimeDelay =
                UnityEngine.Random.Range(bubbleGenerationDelay - ConfigDictionary.Config.bubbleGenerationDelayRandom,
                    bubbleGenerationDelay + ConfigDictionary.Config.bubbleGenerationDelayRandom);
            yield return new WaitForSeconds(nextBubbleTimeDelay);
            StartCoroutine(GenerateBubblesForSingleGame());
        }
    }

    private void NextLevel()
    {
        _curLevel++;
        ResourceManager.GenerateNewTexturesSet();
        smallBubbleSpeed += ConfigDictionary.Config.nextLevelIncreaseBubbleSpeed;
        bigBubbleSpeed += ConfigDictionary.Config.nextLevelIncreaseBubbleSpeed;
        _levelTimer = ConfigDictionary.Config.levelTime;
        bubbleGenerationDelay -= ConfigDictionary.Config.nextLevelIncreaseBubbleGenerationDelay;
        bubbleGenerationDelay = Mathf.Clamp(bubbleGenerationDelay, 0.5f, 1000);
        NewLevelProxy proxy = new NewLevelProxy();
        proxy.score = score;
        List<NewBubbleProxy> bubbles = new List<NewBubbleProxy>();
        foreach (KeyValuePair<int, BubbleController> bubble in singleton.bubblesDict)
        {
            bubbles.Add(bubble.Value.GenerateNewBubbleProxy());
        }
        proxy.bubbles = bubbles.ToArray<NewBubbleProxy>();
        NetController.SendMessage(NetMessageCode.NewLevel, proxy);
    }

    RaycastHit hit;
    public static void GamerHitScreen(Vector3 position)
    {
        if (singleton == null)
        {
            Debug.LogError("Singleton = null");
            return;
        }
        if (singleton._curGameMode == GameMode.netView)
        {
            return;
        }

        if (Physics.Raycast(Camera.main.ScreenPointToRay(position), out singleton.hit, 100f))
        {
            if (singleton.hit.collider.gameObject.layer == 8)
            {
                int hittedBubbleId = int.Parse(singleton.hit.collider.gameObject.name);
                if (singleton.bubblesDict.ContainsKey(hittedBubbleId))
                {
                    singleton.AddScore(singleton.bubblesDict[hittedBubbleId].GetBubbleSize());
                    singleton.bubblesDict[hittedBubbleId].GamerHitBubble();
                    UserDestroyBubbleProxy proxy = new UserDestroyBubbleProxy(hittedBubbleId,score);
                    NetController.SendMessage(NetMessageCode.UserDestroyBubble, proxy);
                }
            }
        }
    }

    private void AddScore(float bubbleSize)
    {
        _score += GetScoreBySize(bubbleSize);
    }

    public static int GetScoreBySize(float bubbleSize)
    {
        return Mathf.FloorToInt((1 / bubbleSize) * ConfigDictionary.Config.bubbleSizeToScoreCoef);
    }

    public static void RemoveBubbleFromDict(int _id)
    {
        singleton.bubblesDict.Remove(_id);
    }

    private void DestroyAllBubblesInDict()
    {
        foreach (KeyValuePair<int, BubbleController> bubble in bubblesDict)
        {
            Destroy(bubble.Value.gameObject);
        }
        bubblesDict.Clear();
    }

    public static GameStateProxy GetGameState()
    {
        GameStateProxy proxy = new GameStateProxy();
        proxy.level = curLevel;
        proxy.score = score;
        proxy.timer = levelTimer;
        proxy.smallBubbleSpeed = singleton.smallBubbleSpeed;
        proxy.bigBubbleSpeed = singleton.bigBubbleSpeed;
        
        List<NewBubbleProxy> bubbles = new List<NewBubbleProxy>();
        foreach (KeyValuePair<int, BubbleController> bubble in singleton.bubblesDict)
        {
            bubbles.Add(bubble.Value.GenerateNewBubbleProxy());
        }
        proxy.bubbles = bubbles.ToArray<NewBubbleProxy>();
        return proxy;
    }
    
    #region Net watch game mode
    public static void StartNetWatchGame(GameStateProxy proxy)
    {
        if (singleton._curGameMode != GameMode.netView)
        {
            UserInterfaceController.ChangeUserInterfaceMode(InterfaceMode.WatchNetGame);
            singleton._curGameMode = GameMode.netView;
            singleton.DestroyAllBubblesInDict();
            singleton._curGameMode = GameMode.netView;
            singleton._curLevel = proxy.level;
            singleton._score = proxy.score;
            ResourceManager.GenerateNewTexturesSet();
            singleton.smallBubbleSpeed = proxy.smallBubbleSpeed;
            singleton.bigBubbleSpeed = proxy.bigBubbleSpeed;
            singleton._levelTimer = proxy.timer;
            for (int i = 0; i < proxy.bubbles.Length; i++)
            {
                BubbleController newBubble = singleton.bubblesGenerator.GenerateNewNetWatchBubble(proxy.bubbles[i],
                    singleton.smallBubbleSpeed, singleton.bigBubbleSpeed);
                singleton.bubblesDict.Add(newBubble.id, newBubble);
            }
        }
    }

    public static void StopNetWatchGame()
    {
        singleton._curGameMode = GameMode.none;
        NetController.DisconnectNet();
        singleton.DestroyAllBubblesInDict();
    }

    public static void CreateNetWatchBubble(NewBubbleProxy proxy)
    {
        if (singleton._curGameMode == GameMode.netView)
        {
            BubbleController newBubble = singleton.bubblesGenerator.GenerateNewNetWatchBubble(proxy, singleton.smallBubbleSpeed, singleton.bigBubbleSpeed);
            singleton.bubblesDict.Add(newBubble.id, newBubble);
        }
    }

    public static void DestroyNetWatchBuble(UserDestroyBubbleProxy proxy)
    {
        if (singleton.bubblesDict.ContainsKey(proxy.id))
        {
            singleton.AddScore(singleton.bubblesDict[proxy.id].GetBubbleSize());
            singleton.bubblesDict[proxy.id].GamerHitBubble();
        }
        else
        {
            Debug.LogError("Dictionary not contains bubble id=" + proxy.id);
        }
        singleton._score = proxy.resScore;
    }

    public static void NewNetWatchLevel(NewLevelProxy proxy)
    {
        singleton._curLevel++;
        ResourceManager.GenerateNewTexturesSet();
        singleton.smallBubbleSpeed += ConfigDictionary.Config.nextLevelIncreaseBubbleSpeed;
        singleton.bigBubbleSpeed += ConfigDictionary.Config.nextLevelIncreaseBubbleSpeed;
        singleton._levelTimer = ConfigDictionary.Config.levelTime;
        singleton.bubbleGenerationDelay -= ConfigDictionary.Config.nextLevelIncreaseBubbleGenerationDelay;
        singleton.bubbleGenerationDelay = Mathf.Clamp(singleton.bubbleGenerationDelay, 0.5f, 1000);
        singleton.DestroyAllBubblesInDict();
        for (int i = 0; i < proxy.bubbles.Length; i++)
        {
            BubbleController newBubble = singleton.bubblesGenerator.GenerateNewNetWatchBubble(proxy.bubbles[i], singleton.smallBubbleSpeed, singleton.bigBubbleSpeed);
            singleton.bubblesDict.Add(newBubble.id, newBubble);
        }
    }
    #endregion
}


public enum GameMode
{
    none,
    single,
    netView
}
