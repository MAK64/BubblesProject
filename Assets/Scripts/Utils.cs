using UnityEngine;

public class Utils
{
    /// <summary>
    /// Генерирует случайный цвет, гарантируя что новый цвет будет заметно отличаться от предыдущего
    /// </summary>
    /// <param name="_prevColor"></param>
    /// <returns></returns>
    public static Color GenerateRandomColor(Color _prevColor)
    {
        Color newCol = Color.white;
        newCol.r = RandomExeptRange(_prevColor.r, 0.1f);
        newCol.g = RandomExeptRange(_prevColor.g, 0.1f);
        newCol.b = RandomExeptRange(_prevColor.b, 0.1f);        
        return newCol;
    }
    private static float RandomExeptRange(float _var, float _range)
    {
        float newVar;
        for (int i = 0; i < 100; i++)
        {
             UnityEngine.Random.seed += 1;
             newVar = UnityEngine.Random.Range(0f, 1f);
             if (newVar < _var - _range || newVar > _var + _range)
             {
                 return newVar;
             }
        }
        return 0;
    }

    /// <summary>
    /// Получает ректангл с координатами суммы двух координат операндов
    /// </summary>
    /// <param name="a">размер игнорируется</param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Rect SumRect(Rect a, Rect b)
    {
        Rect result = new Rect();
        result.x = a.x + b.x;
        result.y = a.y + b.y;
        result.width = b.width;
        result.height = b.height;
        return result;
    }

    /// <summary>
    /// Возвращает координаты центра экрана
    /// </summary>
    /// <returns></returns>
    public static Vector2 GetCenterScreenPos()
    {
        return new Vector2(Screen.width / 2, Screen.height / 2);
    }
    /// <summary>
    /// Генерирует Rect отнисительно центра экрана
    /// </summary>
    /// <param name="_baseRect"></param>
    /// <returns></returns>
    public static Rect GetRectOffsetCenterScreen(Rect _baseRect)
    {
        Rect outRect = _baseRect;
        outRect.x += GetCenterScreenPos().x;
        outRect.y += GetCenterScreenPos().y;
        return outRect;
    }

    /// <summary>
    /// Генерирует Rect отнисительно центральной нижней точки экрана
    /// </summary>
    /// <param name="_baseRect"></param>
    /// <returns></returns>
    public static Rect GetRectOffsetButtomScreen(Rect _baseRect)
    {
        Rect outRect = _baseRect;
        outRect.y = Screen.height - _baseRect.y;
        return outRect;
    }
}

