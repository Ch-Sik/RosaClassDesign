using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpriteButtonUI : MonoBehaviour
{
    public Image image;

    public Color enableColor;
    public Color disableColor;

    private void Start() { if (image == null) image = GetComponent<Image>(); image.color = disableColor; }
    public void OnIn() { image.color = enableColor; }
    public void OnOut() { image.color = disableColor; }
}
