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
    [SerializeField] public int numBadguy = 2;
    private List<GameObject> GuysList = new List<GameObject>();
    private List<int> GuysType = new List<int>();
    void Start()
    {
        Generate_Creatures();
    }
    void Update()
    {

    }
    void Generate_Creatures()
    {
        if (Goodguy_Prefab != null && Badguy_Prefab != null) {
            int numGenGoodguys = 0;
            int numGenBadguys = 0;

            for (int i=0; i<(numGoodguy+numBadguy); i++) {
                float angle = i * (360f / (numGoodguy+numBadguy)); // Calculate the angle of each spawning point on the circle
                float x = spawnCenter.x + spawnCircleRad * Mathf.Cos(Mathf.Deg2Rad * angle);
                float z = spawnCenter.z + spawnCircleRad * Mathf.Sin(Mathf.Deg2Rad * angle);
                Vector3 spawnPos = new Vector3(x, spawnCenter.y, z);
                Quaternion rotation = Quaternion.LookRotation(spawnCenter - spawnPos); // Face Center

                GameObject creaturePrefab; // Random sequence
                if (Random.Range(0f, 1f) < 0.5f && numGenGoodguys < numGoodguy) {
                    creaturePrefab = Goodguy_Prefab;
                    GuysType.Add(0);
                    numGenGoodguys++;
                }
                else {
                    if (numGenBadguys < numBadguy) {
                        creaturePrefab = Badguy_Prefab;
                        GuysType.Add(1);
                        numGenBadguys++;
                    }
                    else {
                        creaturePrefab = Goodguy_Prefab;
                        GuysType.Add(0);
                        numGenGoodguys++;
                    }
                }

                GameObject guy = Instantiate(creaturePrefab, spawnPos, rotation); // Both good and bad
                GuysList.Add(guy);
            }
        }
        else Debug.LogError("(PreFab is NULL)");
    }

}
