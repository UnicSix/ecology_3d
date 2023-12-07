using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerStatusHandler : MonoBehaviour
{
    public Slider work;
    public Slider panic;
    public Slider alarm;
    private Canvas selfcanvas;
    void Start()
    {
        selfcanvas = GetComponent<Canvas>();
        work.value = 0;
        panic.value = 0;
        alarm.value = 0;
        // value can't be larger than 1 and smaller than zero (auto adjusted)
        HideStatus();
    }
    public void update_work(float val) {
        work.value = val;
    }
    public void update_panic(float val) {
        panic.value = val;
    }
    public void update_alarm(float val) {
        alarm.value = val; 
    }
    public void HideStatus() {
        selfcanvas.enabled = false;
    }
    public void ShowStatus() {
        selfcanvas.enabled = true;
    }
}
