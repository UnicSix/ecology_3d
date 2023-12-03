using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowControl : MonoBehaviour
{
    [SerializeField] public GameObject Goodguy_Prefab;
    [SerializeField] public GameObject Badguy_Prefab;
    [SerializeField] public Vector3 spawnPosition; // 
    [SerializeField] public int numGoodguy = 10;

    void Start()
    {
        spawnPosition = new Vector3(0f, 5.0f, 0f);
        Generate_Creatures();
    }
    // void Update()
    // {
    // }
    void Generate_Creatures()
    {
        if (Goodguy_Prefab != null)
        {
            for (int i=0; i<numGoodguy; i++) // Can try generate around the alert table
            {
                GameObject goodGuy = Instantiate(Goodguy_Prefab, spawnPosition, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogError("(GoodGuy PreFab is NULL)");
        }
    }
}
