using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowControl : MonoBehaviour
{
    [SerializeField] public GameObject Goodguy_Prefab;
    [SerializeField] public GameObject Badguy_Prefab;
    [SerializeField] public Vector3 spawnCenter = new Vector3(0f, 5.0f, 0f);
    [SerializeField] public float spawnCircleRad = 5.0f;
    [SerializeField] public int numGoodguy = 10;
    private List<GameObject> goodGuysList = new List<GameObject>();
    void Start()
    {
        Generate_Creatures();
    }
    void Update()
    {

    }
    void Generate_Creatures()
    {
        if (Goodguy_Prefab != null)
        {
            for (int i=0; i<numGoodguy; i++)
            {
                float angle = i * (360f / numGoodguy); // Calculate the angle of each spawning point on the circle
                float x = spawnCenter.x + spawnCircleRad * Mathf.Cos(Mathf.Deg2Rad * angle);
                float z = spawnCenter.z + spawnCircleRad * Mathf.Sin(Mathf.Deg2Rad * angle);
                Vector3 spawnPos = new Vector3(x, spawnCenter.y, z);
                GameObject goodGuy = Instantiate(Goodguy_Prefab, spawnPos, Quaternion.identity);
                goodGuysList.Add(goodGuy);
            }
        }
        else
        {
            Debug.LogError("(GoodGuy PreFab is NULL)");
        }
    }
}
