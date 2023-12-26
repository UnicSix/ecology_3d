using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiHandler : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private Image icon;

    private void Start() {
        Hide();
    }

    public void Show()
    {
        Color color1 = bg.color;
        Color color2 = icon.color;
        color1.a = 1.0f;
        color2.a = 1.0f;
        bg.color = color1;
        icon.color = color2;
    }
    public void Hide()
    {
        Color color1 = bg.color;
        Color color2 = icon.color;
        color1.a = 0.0f;
        color2.a = 0.0f;
        bg.color = color1;
        icon.color = color2;
    }
}
