using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressStatusHandler : MonoBehaviour
{
    public Image progressImage;
    public Color redColor = Color.red;
    public Color greenColor = Color.green;
    private Canvas selfcanvas;
    public bool occupied = false;
    [Range(0f, 1f)] public float progress_val = 0.0f;

    void Start()
    {
        selfcanvas = GetComponent<Canvas>();
        progressImage.fillAmount = 0;
        progressImage.color = redColor;
        // value can't be larger than 1 and smaller than zero (auto adjusted)
        // Hide();
    }
    private void Update() {
        update_progress(progress_val);
    }
    public void update_progress(float val) {
        progressImage.fillAmount = Mathf.Clamp01(val);
        progressImage.color = Color.Lerp(redColor, greenColor, progressImage.fillAmount);
    }
    public void Hide() {
        selfcanvas.enabled = false;
    }
    public void Show() {
        selfcanvas.enabled = true;
    }
}
