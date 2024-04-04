using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicObject : MonoBehaviour
{
    [Space(10), Header("Tweening")]
    [SerializeField]
    private Color tweeningColor = Color.black;
    [SerializeField]
    private float blinkingTime = 0.1f;
    [SerializeField]
    private List<SpriteRenderer> objectSprites = new List<SpriteRenderer>();
    [SerializeField]
    private Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();

    private bool isDestoryByTime = false;
    private PlayerMagic playerMagic;
    public virtual void Init(Vector2 magicPos, float lifeTime,TilemapGroup tilemapGroup, PlayerMagic pm)
    {
        playerMagic = pm;
        StartCoroutine(DestroyByLifetime(lifeTime));
    }

    private IEnumerator DestroyByLifetime(float lifeTime)
    {
        float destroyPreviewTime = 3f;
        float timeTilDestroyReady = lifeTime - destroyPreviewTime;


        Debug.Assert(lifeTime > destroyPreviewTime, "식물 유지 시간이 너무 짧음!");
        yield return new WaitForSeconds(timeTilDestroyReady);

        
        StartBlinking();
        isDestoryByTime = true;

        yield return new WaitForSeconds(destroyPreviewTime);
        isDestoryByTime = false;
        playerMagic.Dequeue();
        Destroy(gameObject);
    }

    /// <summary> 오브젝트 점멸 시작 </summary>
    public void StartBlinking()
    {
        if (objectSprites.Count > 0) return; // 이미 점멸 중일 경우 중복 점멸하지 않음

        objectSprites.AddRange(GetComponentsInChildren<SpriteRenderer>());


        foreach (var renderer in objectSprites)
        {
            originalColors[renderer] = renderer.color;
        }

        foreach (var renderer in objectSprites)
        {
            renderer.DOColor(tweeningColor, blinkingTime).SetLoops(-1, LoopType.Yoyo);
        }

    }

    /// <summary> 오브젝트 점멸 취소 </summary>
    public void StopBlinking()
    {
        if (isDestoryByTime) return; // 삭제 시간일 때 점멸은 멈추지 않음

        foreach (var renderer in objectSprites)
        {
            renderer.DOKill(true);
            renderer.color = originalColors[renderer];
        }
        objectSprites.Clear();
        originalColors.Clear();
    }

    /// <summary> 나중에 오브젝트나 기믹에 의한 제거가 있을 때 OnDestroy 대신 다른 메소드로 사용</summary>
    /*
    private void OnDestroy()
    {
        playerMagic.DestroyByObject(gameObject);
    }
    */
}
