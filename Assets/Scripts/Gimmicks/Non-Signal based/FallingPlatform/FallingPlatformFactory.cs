using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformFactory : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] float startDelay = 0f;
    [SerializeField] float spawnInterval = 3f;
    [SerializeField] GameObject preview;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(preview);
        DOTween.Sequence().AppendInterval(startDelay).AppendCallback(
            () => {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    Instantiate(prefab, transform.position, Quaternion.identity);
                })
                .AppendInterval(spawnInterval)
                .SetLoops(-1);
            }
        );
    }
}
