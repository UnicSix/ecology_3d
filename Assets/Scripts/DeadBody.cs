using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBody : MonoBehaviour
{
    private List<GameObject> guys = new List<GameObject>();
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("BadGuy")||other.gameObject.CompareTag("GoodGuy"))
            guys.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        guys.Remove(other.gameObject);
    }

    public List<GameObject> GetObjectsInTrigger()
    {
        return guys;
    }

    public void bodyReported()
    {
        foreach (GameObject guy in guys)
        {
            if (guy.CompareTag("GoodGuy"))
            {
                guy.GetComponent<GoodGuyBehaviour>().sus+=10f;
            }
            else if (guy.CompareTag("BadGuy"))
            {
                guy.GetComponent<GoodGuyBehaviour>().sus+=10f;
            }
        }
    }
}
