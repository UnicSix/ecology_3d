using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MurdererStatusHandler : MonoBehaviour
{
    public Text Name;
    public Image focus0, focus1, focus2;// focus3, focus4;
    public Slider sus;
    [FormerlySerializedAs("idle")] public Slider wreck;
    [FormerlySerializedAs("chaos")] public Slider track;
    [FormerlySerializedAs("tailgating")] public Slider kill;
    private Canvas selfcanvas;
    private void Start()
    {
        selfcanvas = GetComponent<Canvas>();
        sus.value = 0;
        wreck.value = 0;
        track.value = 0;
        kill.value = 0;
        // value can't be larger than 1 and smaller than zero (auto adjusted)
        Hide();
    }
    public void set_name(string str) {
        Name.text = str;
    }
    public void update_sus(float val) {
        sus.value = Mathf.Clamp01(val);
    }
    public void update_idle(float val) {
        wreck.value = Mathf.Clamp01(val);
    }
    public void update_chaos(float val) {
        track.value = Mathf.Clamp01(val);
    }
    public void update_tailgating(float val) {
        kill.value = Mathf.Clamp01(val);
    }
    public void Select(int index) {
        // Image[] images = { focus0, focus1, focus2, focus3, focus4 };
        Image[] images = { focus0, focus1, focus2};
        for (int i = 0; i < images.Length; i++) {
            if (i != index) {
                Color color = images[i].color;
                color.a = 0f;
                images[i].color = color;
            }
            else {
                Color color = images[i].color;
                color.a = 0.5f;
                images[i].color = color;
            }
        }
    }
    public void Hide() {
        selfcanvas.enabled = false;
    }
    public void Show() {
        selfcanvas.enabled = true;
    }
}