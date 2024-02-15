using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MiniMapSector : MonoBehaviour
{
    public Image image;

    public void SetMapSector(SO_SceneMapData data, float scale = 3)
    {
        image.sprite = GetSceneSprite(data.size, data.tiles);
        RectTransform rect = image.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(data.size.x, data.size.y);
        rect.localScale = new Vector2(scale, scale);
        rect.anchoredPosition = new Vector2(
                                    data.anchor.x,
                                    data.anchor.y) * scale;
    }

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

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        return sprite;
    }
}
