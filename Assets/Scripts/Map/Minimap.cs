using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Sprite GetSceneSprite(Vector2Int size, List<Vector2Int> tiles)
    {
        Texture2D texture = new Texture2D(size.x, size.y);

        for (int y = 0; y < texture.height; y++)
            for (int x = 0; x < texture.width; x++)
                if (tiles.Contains(new Vector2Int(x, y)))
                    texture.SetPixel(x, y, Color.white);
                else
                    texture.SetPixel(x, y, new Color(0, 0, 0, 0));

        texture.filterMode = FilterMode.Point;
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    }
}
