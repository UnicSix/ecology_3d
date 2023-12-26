using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    GameObject[] taskPoints;
    GameObject[] SpawnPoints;
    [SerializeField] public GameObject Goodguy_Prefab;
    [SerializeField] public GameObject Badguy_Prefab;
    [SerializeField] public GameObject bodyAlarm;
    [SerializeField] public GameObject ButtonAlarm;
    [SerializeField] private GameObject spaceShipDurabilityHandler;
    [SerializeField] public Vector3 spawnCenter = new Vector3(0f, 5.0f, 0f);
    [SerializeField] public float spawnCircleRad = 5.0f;
    [SerializeField] public int numGoodguy = 10;
    [SerializeField] public int numBadguy = 2;
    [SerializeField] private float spwanSec = 30.0f;
    [SerializeField] public float susThreashold = 0.11f;
    
    private bool isMeeting = false;
    private int guynumber = 0;
    private float spawnTimer;
    private float spaceship_durability;
    
    void Start()
    {
        spawnTimer = 0.0f;
        spaceship_durability = 1.0f;
        taskPoints = GameObject.FindGameObjectsWithTag("Task");
        SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        ProgressStatusHandler.OnInfluenceShipDurability += HandleShipDurability;
        if (taskPoints.Length != 0) {} //Debug.Log("Found " + taskPoints.Length + " TaskPoints.");
        else Debug.LogWarning("No TaskPoints found in the scene.");
        Generate_Guys();
    }
    void Update()
    {
        if (isMeeting) Meeting(transition: false);
        else HandleSpawnGuy();
        // Need to Trigger DisbandMeeting()
        Update_SpaceshipDurability();
    }
    public void Meeting(bool findbody = false, bool transition = true) {
        isMeeting = true;
        GameObject[] goodGuys = GameObject.FindGameObjectsWithTag("GoodGuy");
        GameObject[] badGuys = GameObject.FindGameObjectsWithTag("BadGuy");

        if (transition) {
            if (findbody) {
                StartCoroutine(ActivateAndDeactivate(bodyAlarm));
            } else {
                StartCoroutine(ActivateAndDeactivate(ButtonAlarm));
            }
        }

        foreach (GameObject goodGuy in goodGuys)
            goodGuy.GetComponent<GoodGuyBehaviour>().nowStatus = -2;

        foreach (GameObject badGuy in badGuys)
            badGuy.GetComponent<BadGuy>().nowStatus = -2;

        int total_guys = goodGuys.Length + badGuys.Length;
        List<GameObject> inVoting = new List<GameObject>();
        List<float> inVotingSus = new List<float>();
        for (int i = 0; i < total_guys; i++) {
            float angle = i * (360f / total_guys); // Calculate the angle of each spawning point on the circle
            float x = spawnCenter.x + spawnCircleRad * Mathf.Cos(Mathf.Deg2Rad * angle);
            float z = spawnCenter.z + spawnCircleRad * Mathf.Sin(Mathf.Deg2Rad * angle);
            Vector3 spawnPos = new Vector3(x, spawnCenter.y, z);
            Quaternion rotation = Quaternion.LookRotation(spawnCenter - spawnPos); // Face Center

            if (i < goodGuys.Length) {
                GoodGuyBehaviour behaviour = goodGuys[i].GetComponent<GoodGuyBehaviour>();
                behaviour.CutAgentPath();
                goodGuys[i].transform.position = spawnPos;
                goodGuys[i].transform.rotation = rotation;
                if (behaviour.sus >= susThreashold) {
                    inVoting.Add(goodGuys[i]);
                    inVotingSus.Add(behaviour.sus);
                }
            }
            else {
                BadGuy behaviour = badGuys[i - goodGuys.Length].GetComponent<BadGuy>();
                behaviour.CutAgentPath();
                badGuys[i - goodGuys.Length].transform.position = spawnPos;
                badGuys[i - goodGuys.Length].transform.rotation = rotation;
                if (behaviour.sus >= susThreashold) {
                    inVoting.Add(badGuys[i - goodGuys.Length]);
                    inVotingSus.Add(behaviour.sus);
                }
            }
        }

        // Vote
        StartCoroutine(Voting(inVoting, inVotingSus));
    }
    public void DisbandMeeting() {
        isMeeting = false;
        GameObject[] goodGuys = GameObject.FindGameObjectsWithTag("GoodGuy");
        GameObject[] badGuys = GameObject.FindGameObjectsWithTag("BadGuy");
        foreach (GameObject goodGuy in goodGuys)
            goodGuy.GetComponent<GoodGuyBehaviour>().nowStatus = -1;

        foreach (GameObject badGuy in badGuys)
            badGuy.GetComponent<BadGuy>().nowStatus = -1;
    }

    void Generate_Guys()
    {
        if (Goodguy_Prefab != null && Badguy_Prefab != null) {
            int numGenGoodguys = 0;
            int numGenBadguys = 0;
            guynumber = (numGoodguy + numBadguy);

            for (int i = 0; i < (numGoodguy + numBadguy); i++) {
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
    void HandleShipDurability(float influence) // Event Receive
    {
        spaceship_durability += influence;
        spaceship_durability = Mathf.Clamp01(spaceship_durability);
    }
    void HandleSpawnGuy()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spwanSec) {
            Vector3 facingDirection = SpawnPoints[0].transform.Find("Facing").position - SpawnPoints[0].transform.position;
            float ProbOfGoodGuy = spaceship_durability;
            float ProbOfBadGuy = 1 - ProbOfGoodGuy;
            string guy_name = "GUY-" + guynumber;

            if (Random.Range(0f, 1f) < ProbOfGoodGuy) {
                GameObject guy = Instantiate(Goodguy_Prefab, SpawnPoints[0].transform.position, Quaternion.LookRotation(facingDirection));
                guy.GetComponentInChildren<WorkerStatusHandler>().set_name(guy_name);
            }
            else {
                GameObject guy = Instantiate(Badguy_Prefab, SpawnPoints[0].transform.position, Quaternion.LookRotation(facingDirection));
                guy.GetComponentInChildren<MurdererStatusHandler>().set_name(guy_name);
            }
            spawnTimer = 0.0f;
            guynumber += 1;
            Debug.Log("SPAWN: " + guy_name);
        }
    }
    void Update_SpaceshipDurability()
    {
        spaceship_durability = Mathf.Clamp01(spaceship_durability - 0.01f * Time.deltaTime);
        spaceShipDurabilityHandler.GetComponentInChildren<ShipBarHandler>().update_bar(spaceship_durability); // Update UI bar
    }

    public float getSpaceShipDurability()
    {
        return spaceship_durability;
    }

    private IEnumerator ActivateAndDeactivate(GameObject alarmObject, float sec = 2.0f)
    {
        UiHandler effect = alarmObject.GetComponent<UiHandler>();
        effect.Show();
        yield return new WaitForSeconds(sec);
        effect.Hide();
    }
    private IEnumerator Voting(List<GameObject> guy_lst, List<float> sus_lst, float waitSec1 = 4.0f, float waitSec2 = 1.0f)
    {
        yield return new WaitForSeconds(waitSec1);

        // Sum of sus_lst
        float totalSus = 0.0f;
        foreach (float sus in sus_lst)
        {
            totalSus += sus;
        }

        // RandSelection
        float randomValue = Random.Range(0.0f, totalSus);
        float cumulativeSum = 0.0f;
        GameObject electedGuy = null;

        for (int i = 0; i < sus_lst.Count; i++)
        {
            cumulativeSum += sus_lst[i];
            if (randomValue <= cumulativeSum)
            {
                electedGuy = guy_lst[i];
                break;
            }
        }

        if (electedGuy != null)
        {
            Debug.Log("Kick guy: " + electedGuy.name);
            Destroy(electedGuy);
        }
        else {
            Debug.Log("No guy to Kick");
        }
        
        DisbandMeeting();
        yield return new WaitForSeconds(waitSec2);
    }
}
