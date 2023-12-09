using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MurdererStatusHandler : MonoBehaviour
{
    public Slider chaos;
    public Slider tailgating;
    public Slider assassinate;
    public Slider killingSpree;
    private Canvas selfcanvas;
    private void Start()
    {
        selfcanvas = GetComponent<Canvas>();
        chaos.value = 0;
        tailgating.value = 0;
        assassinate.value = 0;
        killingSpree.value = 0;
        // value can't be larger than 1 and smaller than zero (auto adjusted)
        Hide();
    }
    public void update_Chaos(float val) {
        chaos.value = val;
    }
    public void update_tailgating(float val) {
        tailgating.value = val;
    }
    public void update_assassinate(float val) {
        assassinate.value = val;
    }
    public void update_killingSpree(float val) {
        killingSpree.value = val; 
    }
    public void Hide() {
        selfcanvas.enabled = false;
    }
    public void Show() {
        selfcanvas.enabled = true;
    }
}