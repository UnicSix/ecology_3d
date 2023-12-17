using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwing : MonoBehaviour
{
    private float rotationSpeed = 5f;
    private float rotationDegree = 0f;
    private Vector3 pivot;
    void Update()
    {
        if (rotationDegree < 180f)
        {
            transform.Rotate(rotationSpeed, 0f, 0);
            rotationDegree += rotationSpeed;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}