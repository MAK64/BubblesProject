using UnityEngine;

/// <summary>
/// Класс генерирует по 1 одинаковой текстуре каждого из разрешений
/// </summary>
class TexturesGenerator
{
    public TextureHolder GenerateTextures(TexturesGenerationMode mode, int[] resolutions)
    {
        TextureHolder newTextureHolder = new TextureHolder();        
        Color mainColor = Utils.GenerateRandomColor(Color.white);
        Color secondColor = Utils.GenerateRandomColor(mainColor);
        newTextureHolder.mainColor = mainColor;
        newTextureHolder.secondColor = secondColor;
        switch (mode)
        {
            case TexturesGenerationMode.LinearRamp:            
                {
                    for (int i = 0; i < resolutions.Length; i++)
                    {
                        newTextureHolder.AddTexture(GetLinearRampTexture(resolutions[i], mainColor, secondColor));
                    }
                    break;
                }
            case TexturesGenerationMode.CircularRamp:
                {
                    for (int i = 0; i < resolutions.Length; i++)
                    {
                        newTextureHolder.AddTexture(GetCircularRampTexture(resolutions[i], mainColor, secondColor));
                    }
                    break;
                }

        }
        return newTextureHolder;
    }

    private Texture2D GetLinearRampTexture(int _resolution, Color _upBorder, Color _buttomBorder)
    {
        Texture2D newTex = new Texture2D(_resolution, _resolution, TextureFormat.ARGB32, false);
        Color tmpCol;
        for (int x = 0; x < newTex.width; x++)
        {
            float percent = (float)x / (float)newTex.width;
            tmpCol = Color.Lerp(_upBorder, _buttomBorder, percent);
            for (int y = 0; y < newTex.height; y++)
            {
                newTex.SetPixel(x, y, tmpCol);
            }
        }
        newTex.Apply(false, true);
        return newTex;
    }

    private Texture2D GetCircularRampTexture(int _resolution, Color _center, Color _border)
    {
        Texture2D newTex = new Texture2D(_resolution, _resolution, TextureFormat.ARGB32, false);
        Vector2 center = new Vector2(newTex.width / 2, newTex.width / 2);
        Vector2 curPos = new Vector2();
        for (int x = 0; x < newTex.width; x++)
        {
            for (int y = 0; y < newTex.width; y++)
            {
                curPos.x = x;
                curPos.y = y;
                float percent = (Vector2.Distance(curPos, center) / newTex.width);
                newTex.SetPixel(x, y, Color.Lerp(_center, _border, percent));
            }
        }
        newTex.Apply(false, true);
        return newTex;
    }

}

public enum TexturesGenerationMode
{
    LinearRamp,
    CircularRamp
}