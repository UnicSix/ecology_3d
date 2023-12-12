using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerStatusHandler : MonoBehaviour
{
    public Text Name;
    public Image focus0, focus1, focus2, focus3;
    public Slider sus;
    public Slider idle;
    public Slider work;
    public Slider panic;
    public Slider alarm;
    private Canvas selfcanvas;
    void Start()
    {
        selfcanvas = GetComponent<Canvas>();
        sus.value = 0;
        idle.value = 0;
        work.value = 0;
        panic.value = 0;
        alarm.value = 0;
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
    public void update_work(float val) {
        work.value = Mathf.Clamp01(val);
    }
    public void update_panic(float val) {
        panic.value = Mathf.Clamp01(val);
    }
    public void update_alarm(float val) {
        alarm.value = Mathf.Clamp01(val); 
    }
    public void Select(int index) {
        Image[] images = { focus0, focus1, focus2, focus3 };
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
