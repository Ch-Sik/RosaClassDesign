using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenRoomCurtain : MonoBehaviour
{
    public float fadeTime = 0.5f;
    public int fadeStep = 10;
    public bool destroyOnReveal = false;

    private Tilemap tilemap;
    private BoundsInt curtainArea;
    private int alphaStep;                  // 커튼의 알파 단계. 히든룸에 매우 짧은 시간 들어왔다 나갔을 떄 불투명도가 급격히 변하는 현상 방지용, FadeIn과 FadeOut에서 같이 사용함.

    Coroutine fade = null;
    WaitForSeconds fadeTick = null;       // 코루틴 최적화

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        Collider2D col = GetComponent<Collider2D>();

        // 타일맵 범위 계산
        Vector3Int boundsMin = Vector3Int.FloorToInt(col.bounds.min);
        Vector3Int boundsMax = Vector3Int.CeilToInt(col.bounds.max + new Vector3(0, 0, 1));
        // ↑↑ (boundsMax-boundsMin)의 xyz중 하나이상이 0이면 allPositionsWithin이 동작하지 않음. 
        curtainArea = new BoundsInt(boundsMin, boundsMax - boundsMin);

        alphaStep = fadeStep;
        fadeTick = new WaitForSeconds(fadeTime / fadeStep);

        foreach(Vector3Int point in curtainArea.allPositionsWithin)
        {
            // 타일 컬러 고정 플래그 해제. 타일 컬러 변경을 위해 필요.
            tilemap.SetTileFlags(point, TileFlags.None);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player"))
        { return; }

        if(fade != null)
        {
            StopCoroutine(fade);
        }
        fade = StartCoroutine(FadeOut());

        IEnumerator FadeOut()
        {
            for (; alphaStep >= 0; alphaStep--)
            {
                foreach (Vector3Int point in curtainArea.allPositionsWithin)
                {
                    tilemap.SetColor(point, new Color(1, 1, 1, alphaStep / (float)fadeStep));
                }
                yield return fadeTick;
            }
            if(destroyOnReveal)
                Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (destroyOnReveal)
            return;     // 한번 밝혀지면 그대로 유지되어야 하는 경우, fadeIn을 굳이 하지 않음.

        if (!collision.CompareTag("Player"))
        { return; }

        if (fade != null)
        {
            StopCoroutine(fade);
        }
        fade = StartCoroutine(FadeIn());
        IEnumerator FadeIn()
        {
            for (; alphaStep <= fadeStep; alphaStep++)
            {
                foreach (Vector3Int point in curtainArea.allPositionsWithin)
                {
                    tilemap.SetColor(point, new Color(1, 1, 1, alphaStep / (float)fadeStep));
                }
                yield return fadeTick;
            }
        }
    }
}
