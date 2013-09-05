using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Класс отвечает за загрузку, хранение и очистку всех ресурсов. Также отвечает за генерацию текстур
/// </summary>
public class ResourceManager : MonoBehaviour
{
    private static ResourceManager singleton = null;

    public static bool isResourcesLoaded { get; private set; }
    public static Action OnAllResoucesLoadedEvent;

    private AssetBundle mainResourcesBundle;

    [SerializeField]
    private Texture2D bubblesMaterialMask;
    public static Texture2D mask
    {
        get
        {
            return singleton.bubblesMaterialMask;
        }
    }
        

    [SerializeField]
    private GameObject _scoreAddInfoPrefab;
    public static GameObject scoreAddInfoPrefab
    {
        get
        {
            return singleton._scoreAddInfoPrefab;
        }
    }

    [SerializeField]
    private GameObject _bubbleExplosionFXPrefab;
    public static GameObject bubbleExplosionFXPrefab
    {
        get
        {
            return singleton._bubbleExplosionFXPrefab;
        }
    }

    [SerializeField]
    private UserInterfaceDataHolder _userInterfaceDataHolder;
    public static UserInterfaceDataHolder userInterfaceDataHolder
    {
        get
        {
            return singleton._userInterfaceDataHolder;
        }
    }

    [SerializeField]
    private AudioClip music;

    [SerializeField]
    private List<AudioClip> _bubbleExplosionFX;
    public static List<AudioClip> bubbleExplosionFX
    {
        get
        {
            return singleton._bubbleExplosionFX;
        }
    }

    [SerializeField]
    private List<TextureHolder> curTexturesSet = new List<TextureHolder>();
    private TexturesGenerator textureGenerator = new TexturesGenerator();
    private void Awake()
    {
        singleton = this;
        isResourcesLoaded = false;        
        
    }

    private void OnDestroy()
    {
        mainResourcesBundle.Unload(true);
        mainResourcesBundle = null;
        curTexturesSet = null;
    }

    private void Start()
    {
        StartLoading();
    }

    private void StartLoading()
    {
        mainResourcesBundle = AssetBundle.CreateFromFile(Application.dataPath + "/Resources/MainResouces.unity3d");
        if (mainResourcesBundle == null)
        {
            Debug.LogError("MainResources bundle not loaded");
        }
        OnBundleLoaded(mainResourcesBundle);        
    }

    private void OnBundleLoaded(AssetBundle bundle)
    {
        try
        {
            bubblesMaterialMask = bundle.Load("Mask128", typeof(Texture2D)) as Texture2D;
            _scoreAddInfoPrefab = bundle.Load("ScoreAddInfoPrefab", typeof(GameObject)) as GameObject;
            _bubbleExplosionFXPrefab = bundle.Load("bubbleExplodeFX", typeof(GameObject)) as GameObject;
            GameObject _userInterfaceDataHolderPrefab = bundle.Load("userInterface", typeof(GameObject)) as GameObject;
            _userInterfaceDataHolder = _userInterfaceDataHolderPrefab.GetComponent<UserInterfaceDataHolder>();
            music = bundle.Load("bubble_up", typeof(AudioClip)) as AudioClip;
            _bubbleExplosionFX = new List<AudioClip>();
            _bubbleExplosionFX.Add(bundle.Load("comedy_bubble_pop_001", typeof(AudioClip)) as AudioClip);
            _bubbleExplosionFX.Add(bundle.Load("comedy_bubble_pop_002", typeof(AudioClip)) as AudioClip);
            _bubbleExplosionFX.Add(bundle.Load("comedy_bubble_pop_003", typeof(AudioClip)) as AudioClip);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return;
        }
        isResourcesLoaded = true;

        if (OnAllResoucesLoadedEvent != null)
        {
            OnAllResoucesLoadedEvent();
        }
        PlayMusic();
    }

    private void PlayMusic()
    {
        GameObject go = new GameObject("Music");
        AudioSource aus = go.AddComponent<AudioSource>();
        aus.clip = music;
        aus.loop = true;
        aus.Play();
    }

    public static void GenerateNewTexturesSet()
    {
        singleton.curTexturesSet.Clear();
        TexturesGenerationMode curMode = TexturesGenerationMode.LinearRamp;
        for (int i = 0; i < ConfigDictionary.Config.countTexturesBySet; i++)
        {
            singleton.curTexturesSet.Add(singleton.textureGenerator.GenerateTextures(curMode, ConfigDictionary.Config.texturesResolutions));
            if (curMode == TexturesGenerationMode.LinearRamp)
            {
                curMode = TexturesGenerationMode.CircularRamp;
            }
            else
            {
                curMode = TexturesGenerationMode.LinearRamp;
            }
        }
    }

    public static TextureHolder GetRandomTextureFromSet()
    {
        return singleton.curTexturesSet[UnityEngine.Random.Range(0, (singleton.curTexturesSet.Count - 1))];
    }


}


