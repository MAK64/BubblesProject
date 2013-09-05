using UnityEngine;

/// <summary>
/// Класс генерирует новые шары
/// </summary>
public class BubblesGenerator
{
    private int bubbleNextId = 0;
    private static GameObject bublesRoot = null;
    private Material baseMaterial;
    private Vector3 topLeftCornerScreen;
    private Vector3 topRightCornerScreen;
    private float _maxBubbleSize = 0f;
    public float maxBubbleSize
    {
        get { return _maxBubbleSize; }
    }
    
    public void Setup()
    {
        if (bublesRoot == null)
        {
            bublesRoot = new GameObject("BubblesRoot");
        }
        baseMaterial = new Material(Shader.Find("Bubble"));
        baseMaterial.SetTexture("_Mask", ResourceManager.mask);
        topLeftCornerScreen = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        topRightCornerScreen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,0));        
        _maxBubbleSize = (topRightCornerScreen.x - topLeftCornerScreen.x) / 3f;
    }
    
    public BubbleController GenerateNewBubble(float _smallBubbleSpeed, float _bigBubbleSpeed)
    {
        bubbleNextId++;

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);

        go.name = bubbleNextId.ToString();
        go.layer = 8;
        go.transform.parent = bublesRoot.transform;
        GameObject.Destroy(go.GetComponent<MeshCollider>());
        go.AddComponent<SphereCollider>();

        Material newBubbleMat = new Material(baseMaterial);
        go.renderer.material = newBubbleMat;

        BubbleController controller = go.AddComponent<BubbleController>();

        float newBubbleSize = GenerateSize();
        float newBubbleSpeed = GenerateSpeed(newBubbleSize, _smallBubbleSpeed,_bigBubbleSpeed);        
        go.transform.position = GenerateInitPosition(newBubbleSize);
        controller.Setup(bubbleNextId, newBubbleSize, newBubbleSpeed, ResourceManager.GetRandomTextureFromSet());
        return controller;
    }

    public BubbleController GenerateNewNetWatchBubble(NewBubbleProxy proxy, float _smallBubbleSpeed, float _bigBubbleSpeed )
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = proxy.id.ToString();
        go.layer = 8;
        go.transform.parent = bublesRoot.transform;
        GameObject.Destroy(go.GetComponent<MeshCollider>());
        go.AddComponent<SphereCollider>();
        Material newBubbleMat = new Material(baseMaterial);
        go.renderer.material = newBubbleMat;
        BubbleController controller = go.AddComponent<BubbleController>();
        float newBubbleSize = proxy.size;
        float newBubbleSpeed = GenerateSpeed(newBubbleSize, _smallBubbleSpeed, _bigBubbleSpeed);
        go.transform.position = GenerateInitPosition(proxy.posX, proxy.posY);
        controller.Setup(proxy.id, newBubbleSize, newBubbleSpeed, ResourceManager.GetRandomTextureFromSet());
        return controller;
    }

    private float GenerateSpeed(float newBubbleSize, float _smallBubbleSpeed, float _bigBubbleSpeed)
    {
        float sizeCoef = Mathf.InverseLerp(ConfigDictionary.Config.minBubbleSize, _maxBubbleSize, newBubbleSize);        
        return Mathf.Lerp(_smallBubbleSpeed,_bigBubbleSpeed, sizeCoef);
    }


    private float GenerateSize()
    {
        return UnityEngine.Random.Range(ConfigDictionary.Config.minBubbleSize, _maxBubbleSize);        
    }

    private Vector3 GenerateInitPosition(float newBubbleSize)
    {
        float minPosX = topLeftCornerScreen.x + (newBubbleSize / 2);
        float maxPosX = topRightCornerScreen.x - (newBubbleSize / 2);
        float PosY = topLeftCornerScreen.y + (newBubbleSize / 2);
        return new Vector3(UnityEngine.Random.Range(minPosX, maxPosX), PosY, 0f);
    }

    private Vector3 GenerateInitPosition(float posX, float posY)
    {
        return new Vector3(posX, posY, 0f);
    }

    
}

