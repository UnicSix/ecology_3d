using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MurdererStatusHandler : MonoBehaviour
{
    public Text Name;
    public Image focus0, focus1, focus2, focus3, focus4;
    public Slider sus;
    public Slider idle;
    public Slider chaos;
    public Slider tailgating;
    public Slider assassinate;
    public Slider killingSpree;
    private Canvas selfcanvas;
    private void Start()
    {
        selfcanvas = GetComponent<Canvas>();
        sus.value = 0;
        idle.value = 0;
        chaos.value = 0;
        tailgating.value = 0;
        assassinate.value = 0;
        killingSpree.value = 0;
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
        idle.value = Mathf.Clamp01(val);
    }
    public void update_chaos(float val) {
        chaos.value = Mathf.Clamp01(val);
    }
    public void update_tailgating(float val) {
        tailgating.value = Mathf.Clamp01(val);
    }
    public void update_assassinate(float val) {
        assassinate.value = Mathf.Clamp01(val);
    }
    public void update_killingSpree(float val) {
        killingSpree.value = Mathf.Clamp01(val); 
    }
    public void Select(int index) {
        Image[] images = { focus0, focus1, focus2, focus3, focus4 };
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