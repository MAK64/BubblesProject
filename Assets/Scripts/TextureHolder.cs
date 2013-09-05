using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Класс содержит 1 набор текстур
/// </summary>
[Serializable]
public class TextureHolder
{
    private Dictionary<int,Texture2D> texturesByResolution;
    public Color mainColor;
    public Color secondColor;

    public TextureHolder()
    {
        texturesByResolution = new Dictionary<int, Texture2D>();
        mainColor = Color.white;
        secondColor = Color.white;
    }

    public TextureHolder(Dictionary<int,Texture2D> _textures, Color _mainColor, Color _secondColor)
    {
        texturesByResolution = _textures;
        mainColor = _mainColor;
        secondColor = _secondColor;
    }

    public void AddTexture(Texture2D _texture)
    {
        if (texturesByResolution.ContainsKey(_texture.width))
        {            
            texturesByResolution[_texture.width] = _texture;
        }
        else
        {
            texturesByResolution.Add(_texture.width, _texture);
        }
    }

    public Texture2D GetTextureByResolution(int _resolution)
    {
        if (texturesByResolution.ContainsKey(_resolution))
        {
            return texturesByResolution[_resolution];
        }
        Debug.LogError("Texture with resolution not found " + _resolution);
        return new Texture2D(_resolution, _resolution, TextureFormat.ARGB32, false);
    }
}

