using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Содержит всю конфигурацию
/// </summary>
public class ConfigDataHolder
{
    public int socketConnectPort = 8888; //Порт для подключения по сети
    public float minBubbleSize = 1f; //Минимальный размер шарика    

    public float baseSmallBubbleSpeed = 5f; // Базовая скорость падения самого мелкого шарика
    public float baseBigBubbleSpeed = 1f; // Базовая скорость падения самого крупного шарика
    public float nextLevelIncreaseBubbleSpeed = 1f; //на сколько увеличиваем скорость со следующим уровнем
    
    public float bubbleSizeToScoreCoef = 10f; // Коеффициент преобразования размера шарика в очки. 
    public float baseBubbleGenerationDelay = 2f; // скорость генерации шаров в секунду
    public float nextLevelIncreaseBubbleGenerationDelay = 0.1f; // на сколько увеличим скорость генерации шаров в секунду

    public float bubbleGenerationDelayRandom = 0.1f; // диапазон, в котором будет варьироватся скорость генерации шаров


    public float levelTime = 30f; //сколько будет длится каждый уровень

    public int countTexturesBySet = 5; //количество текстур в сете
    public int[] texturesResolutions = new int[] { 32, 64, 128, 256 };//разрешения текстур, которые генерятся для шаров
}

