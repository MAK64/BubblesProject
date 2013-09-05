using UnityEngine;

/// <summary>
/// Класс, отвечает за поведение шара в игре
/// </summary>
public class BubbleController : MonoBehaviour
{
    public int id { get; private set; }
    private float speed = 0f;
    private TextureHolder bubbleTexture;
    private Transform bubbleTransform = null;
    private float buttomCornerScreen = 0;

    public void Setup(int _id, float _scale, float _speed, TextureHolder _bubbleTexture)
    {
        id = _id;
        speed = _speed;
        bubbleTexture = _bubbleTexture;
        transform.localScale = new Vector3(_scale, _scale, _scale);
        renderer.material.SetColor("_DiffuseColor", Color.white);
        int resolution = Mathf.FloorToInt((_scale * ConfigDictionary.Config.texturesResolutions.Length) / GameController.maxBubbleSize);
        resolution = Mathf.Clamp(resolution, 0, ConfigDictionary.Config.texturesResolutions.Length - 1);
        renderer.material.SetTexture("_Texture", bubbleTexture.GetTextureByResolution(ConfigDictionary.Config.texturesResolutions[resolution]));
    }
    
    void Awake()
    {
        bubbleTransform = transform;
        buttomCornerScreen = Camera.main.ScreenToWorldPoint(Vector3.zero).y;
    }

    private Vector3 newPosition;
    void Update()
    {
        if (bubbleTransform == null)
        {
            Debug.LogError("bubbleTransform = null");
            return;
        }
        newPosition = bubbleTransform.position;
        newPosition.y -= speed * Time.deltaTime;
        bubbleTransform.position = newPosition;

        if (bubbleTransform.position.y < (buttomCornerScreen + (bubbleTransform.localScale.x/2)))
        {
            ShowExplodeFX();
            Destroy(gameObject);
        }
    }

    public float GetBubbleSize()
    {
        return bubbleTransform.localScale.x;
    }

    public void SetPosition(Vector3 _position)
    {
        if (bubbleTransform == null)
        {
            Debug.LogError("bubbleTransform = null");
            return;
        }

        bubbleTransform.position = _position;
    }

    public Vector3 GetPosition()
    {
        if (bubbleTransform == null)
        {
            Debug.LogError("bubbleTransform = null");
            return Vector3.zero;
        }
        return bubbleTransform.position;
    }

    public void GamerHitBubble()
    {
        ShowScoreAddInfo();
        ShowExplodeFX();
        Destroy(gameObject);
    }

    private void ShowScoreAddInfo()
    {
        GameObject go = (GameObject)Instantiate(ResourceManager.scoreAddInfoPrefab);
        TextMesh tm = go.GetComponent<TextMesh>();
        tm.text = "+" + GameController.GetScoreBySize(bubbleTransform.localScale.x);
        tm.color = bubbleTexture.mainColor;
        go.transform.position = bubbleTransform.position;
        Destroy(go, 1f);
    }

    private void ShowExplodeFX()
    {
        GameObject particlesFX = (GameObject)Instantiate(ResourceManager.bubbleExplosionFXPrefab);
        particlesFX.particleSystem.startColor = bubbleTexture.mainColor;
        particlesFX.transform.position = transform.position;
        Destroy(particlesFX, particlesFX.particleSystem.duration);
        AudioSource.PlayClipAtPoint(ResourceManager.bubbleExplosionFX[UnityEngine.Random.Range(0, (ResourceManager.bubbleExplosionFX.Count - 1))], Vector3.zero);
    }

    void OnDestroy()
    {
        GameController.RemoveBubbleFromDict(id);
    }

    public NewBubbleProxy GenerateNewBubbleProxy()
    {
        NewBubbleProxy proxy = new NewBubbleProxy();
        proxy.id = id;
        proxy.posX = bubbleTransform.position.x;
        proxy.posY = bubbleTransform.position.y;
        proxy.size = bubbleTransform.localScale.x;
        return proxy;
    }
}
