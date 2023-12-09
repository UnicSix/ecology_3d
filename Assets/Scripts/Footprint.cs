using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footprint : MonoBehaviour
{
    [Tooltip("this is how long the decal will stay, before it shrinks away totally")]
    public float Lifetime = 5.0f;
    private float mark;
    private Vector3 OrigSize;

    public void Start()
    {
        mark = Time.time;
        OrigSize = this.transform.localScale;
        Lifetime = 5f;
    }
    public void Update()
    {
        float ElapsedTime = Time.time - mark;
        if (ElapsedTime != 0)
        {
            float PercentTimeLeft = (Lifetime - ElapsedTime) / Lifetime;

            this.transform.localScale = new Vector3(OrigSize.x * PercentTimeLeft, OrigSize.y , OrigSize.z * PercentTimeLeft);
            if (ElapsedTime > Lifetime)
            {
                Destroy(this.gameObject);
            }
        }
    }
}