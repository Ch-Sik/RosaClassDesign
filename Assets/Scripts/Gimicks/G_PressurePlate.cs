using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class G_PressurePlate : MonoBehaviour
{
    public Sprite active;
    public Sprite inactive;
    public bool isActive = false;
    public int count = 0;

    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void SetSprite()
    {
        if (isActive)
        {
            spriteRenderer.sprite = active;
        }
        else
        {
            spriteRenderer.sprite = inactive;
        }
    }

    bool CheckTag(string tag)
    {
        if(tag == "Player" ||
           tag == "Cube" ||
           tag == "Monster")
            return true;

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CheckTag(collision.tag))
            return;

        count++;

        if (count > 0)
        {
            isActive = true;
            SetSprite();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!CheckTag(collision.tag))
            return;

        count--;

        if (count < 1)
        {
            isActive = false;
            SetSprite();
        }
    }
}