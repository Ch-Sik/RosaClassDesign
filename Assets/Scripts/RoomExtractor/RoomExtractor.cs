using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomExtractor : MonoBehaviour
{
    public Tilemap tilemap;

    public int tilePixelSize = 16;                                      //타일의 픽셀 크기 (16x16)
    //public float pixelsPerUnit = 16f;                                   //스프라이트의 Pixel Per Unit 값
    //public FilterMode filterMode = FilterMode.Point;                    //텍스처의 필터 모드
    [HideInInspector] public string savePath = "Assets/TilemapSprite";

#if UNITY_EDITOR
    [Button]
    public void ConvertTilemapToSprite(string fileName)
    {
        Texture2D texture = GenerateTexture2DFromTilemap();

        Sprite sprite = ConvertTextureToSprite(texture);

        string path = $"{savePath}/{fileName}.png";
        Debug.Log(path);
        SaveSprite(texture, path);
    }

    public Texture2D GenerateTexture2DFromTilemap()
    {
        BoundsInt bounds = tilemap.cellBounds;
        int width = bounds.size.x * tilePixelSize;
        int height = bounds.size.y * tilePixelSize;
        Texture2D texture = new Texture2D(width, height);

        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(cellPosition);

                if (tile != null)
                {
                    Sprite sprite = tilemap.GetSprite(cellPosition);
                    if (sprite != null)
                    {
                        Texture2D tileTexture = sprite.texture;
                        Rect spriteRect = sprite.textureRect;

                        for (int ty = 0; ty < tilePixelSize; ty++)
                        {
                            for (int tx = 0; tx < tilePixelSize; tx++)
                            {
                                int pixelX = (x - bounds.xMin) * tilePixelSize + tx;
                                int pixelY = (y - bounds.yMin) * tilePixelSize + ty;

                                Color tilePixel = Color.white;
                                texture.SetPixel(pixelX, pixelY, tilePixel);
                            }
                        }
                    }
                }
                else
                {
                    for (int ty = 0; ty < tilePixelSize; ty++)
                    {
                        for (int tx = 0; tx < tilePixelSize; tx++)
                        {
                            int pixelX = (x - bounds.xMin) * tilePixelSize + tx;
                            int pixelY = (y - bounds.yMin) * tilePixelSize + ty;
                            texture.SetPixel(pixelX, pixelY, Color.clear);
                        }
                    }
                }
            }
        }

        texture.Apply();
        return texture;
    }

    public Sprite ConvertTextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), tilePixelSize);
    }

    public void SaveSprite(Texture2D texture, string path)
    {
        if (texture == null)
        {
            Debug.LogError("텍스처 없음.");
            return;
        }

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("경로 없음.");
            return;
        }

        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();

        Texture2D loadedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (loadedTexture == null)
        {
            Debug.LogError("로드된 텍스처 없음.");
            return;
        }
        //loadedTexture.filterMode = filterMode;


        Sprite sprite = Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height), new Vector2(0.5f, 0.5f));   //, pixelsPerUnit

        string spritePath = path.Replace(".png", ".asset");
        AssetDatabase.CreateAsset(sprite, spritePath);
        AssetDatabase.SaveAssets();
    }
#endif
}
