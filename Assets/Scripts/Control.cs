using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    GameObject[] taskPoints;
    [SerializeField] public GameObject Goodguy_Prefab;
    [SerializeField] public GameObject Badguy_Prefab;
    [SerializeField] public Vector3 spawnCenter = new Vector3(0f, 5.0f, 0f);
    [SerializeField] public float spawnCircleRad = 5.0f;
    [SerializeField] public int numGoodguy = 10;
    [SerializeField] public int numBadguy = 2;
    private float spaceship_durability;
    
    void Start()
    {
        spaceship_durability = 1.0f;
        taskPoints = GameObject.FindGameObjectsWithTag("Task");
        if (taskPoints.Length != 0) {} //Debug.Log("Found " + taskPoints.Length + " TaskPoints.");
        else Debug.LogWarning("No TaskPoints found in the scene.");
        Generate_Guys();
    }
    void Update()
    {
        Summarize_Tasks();
        Update_SpaceshipDurability();
    }
    public void Meeting() {
        GameObject[] goodGuys = GameObject.FindGameObjectsWithTag("GoodGuy");
        GameObject[] badGuys = GameObject.FindGameObjectsWithTag("BadGuy");
        Debug.Log("<MEETING> G:" + goodGuys.Length + " B:" + badGuys.Length);

        foreach (GameObject goodGuy in goodGuys)
            goodGuy.GetComponent<GoodGuyBehaviour>().nowStatus = -2;

        foreach (GameObject badGuy in badGuys)
            badGuy.GetComponent<BadGuyBehaviour>().nowStatus = -2;

        // spawnCenter (Vector3) => 圓心
        // spawnCircleRad (float) => 半徑
        // look at 圓心
        // every Guy.nowStatus = -2;
    }
    void Generate_Guys()
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

                int guyType = 0;
                GameObject creaturePrefab; // Random sequence
                if (Random.Range(0f, 1f) < 0.5f && numGenGoodguys < numGoodguy) {
                    creaturePrefab = Goodguy_Prefab;
                    numGenGoodguys++;
                }
                else {
                    if (numGenBadguys < numBadguy) {
                        creaturePrefab = Badguy_Prefab;
                        numGenBadguys++;
                        guyType = 1;
                    }
                    else {
                        creaturePrefab = Goodguy_Prefab;
                        numGenGoodguys++;
                    }
                }
                string guy_name = "GUY-" + i;
                GameObject guy = Instantiate(creaturePrefab, spawnPos, rotation); // Both good and bad
                if (guyType == 0) guy.GetComponentInChildren<WorkerStatusHandler>().set_name(guy_name);
                else guy.GetComponentInChildren<MurdererStatusHandler>().set_name(guy_name);
            }
        }
        else Debug.LogError("(PreFab is NULL)");
    }
    void Summarize_Tasks()
    {
        for (int i = 0; i < taskPoints.Length; i++) {
            ProgressStatusHandler handler = taskPoints[i].GetComponentInChildren<ProgressStatusHandler>();
            if (handler.progress_val >= 1.0f) {
                spaceship_durability = Mathf.Clamp01(spaceship_durability + 0.1f);
                handler.progress_val = 0.0f;
                handler.occupied = false;
            }
        }
    }
    void Update_SpaceshipDurability()
    {
        spaceship_durability = Mathf.Clamp01(spaceship_durability - 0.01f * Time.deltaTime);
        // Update UI bar
    }
}
