using UnityEngine;
using System;
/// <summary>
/// Содержит в себе настройки для пользовательского интерфейса, загружается из бандля
/// </summary>
[Serializable]
public class UserInterfaceDataHolder : MonoBehaviour
{
    //main menu
    public Rect newGameRect;
    public Rect watchNetGameRect;

    //game
    public Rect buttomMountRect;
    public Rect scoreRect;
    public Rect pauseRect;
    public Rect levelRect;

    public Texture2D buttomMount;

    //net
    public Rect ipAddressNameRect;
    public Rect portNameRect; 
    public Rect ipAddressRect;
    public Rect portRect;
    public Rect connectRect;
    public Rect backRect;
}

