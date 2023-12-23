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
    private bool occupied = false;
    [Range(0f, 1f)] private float progress_val = 0.0f; // Change Private
    public delegate void ShipDurabilityInfluence(float influence);
    public static event ShipDurabilityInfluence OnInfluenceShipDurability;

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
    public bool IncreaseWorkProgress(float val) {
        progress_val += Mathf.Abs(val);
        if (progress_val >= 1.0f) {
            OnInfluenceShipDurability?.Invoke(0.1f);
            progress_val = 0.0f;
            Leave();
            return true;
        }
        return false;
    }
    public bool DecreaseWorkProgress(float val) {
        if (progress_val == 0.0f) return true;
        progress_val -= Mathf.Abs(val);
        if (progress_val <= 0.0f) {
            OnInfluenceShipDurability?.Invoke(-0.25f);
            Leave();
            return true;
        }
        return false;
    }
    public void update_progress(float val) {
        progressImage.fillAmount = Mathf.Clamp01(val);
        progressImage.color = Color.Lerp(redColor, greenColor, progressImage.fillAmount);
    }
    public void Occupying() {
        occupied = true;
    }
    public void Leave() {
        occupied = false;
    }
    public bool IsOccupied() {
        return occupied;
    }
    public void Hide() {
        selfcanvas.enabled = false;
    }
    public void Show() {
        selfcanvas.enabled = true;
    }
}
