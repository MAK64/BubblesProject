using UnityEngine;

/// <summary>
/// Класс отвечает за обработку нажатий мыши или тапом по экрану(для андройдов и т.д.)
/// </summary>
class UserInputController : MonoBehaviour
{
    void Update()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        GameController.GamerHitScreen(Input.mousePosition);
                    }
                    break;
                }
            case RuntimePlatform.Android:                
            case RuntimePlatform.IPhonePlayer:
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);
                        if (touch.phase == TouchPhase.Began)
                        {
                            GameController.GamerHitScreen(touch.position);
                        }
                    }                    
                    break;
                }
        }        
    }
}

