using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    public GameObject col;
    public GameObject top;
    public float breakDuration = 1.5f;
    public Sprite oriSprite;
    public Sprite breakSprite;

    public void ColBreak()
    {
        col.SetActive(false);
        top.SetActive(false);   //
        gameObject.GetComponent<SpriteRenderer>().sprite = breakSprite;
        StartCoroutine(Restore(breakDuration));
    }

    IEnumerator Restore(float delay)
    {
        yield return new WaitForSeconds(delay);
        col.SetActive(true);
        top.SetActive(true);    //
        gameObject.GetComponent<SpriteRenderer>().sprite = oriSprite;
    }
}
