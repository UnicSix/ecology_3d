using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipBarHandler : MonoBehaviour
{
    [SerializeField] private Slider ShipDurability;
    [SerializeField] private Image barImage;
    public Color redColor = Color.red;
    public Color bluecolor = Color.blue;
    void Start()
    {
        ShipDurability.value = 1.0f;
    }

    public void update_bar(float val) {
        ShipDurability.value = Mathf.Clamp01(val);
        barImage.fillAmount = Mathf.Clamp01(val);
        barImage.color = Color.Lerp(redColor, bluecolor, barImage.fillAmount);
    }
}
