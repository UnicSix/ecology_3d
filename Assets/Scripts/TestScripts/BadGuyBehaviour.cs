using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BadGuyBehaviour : MonoBehaviour
{
    public int nowStatus; // -1: none, -2: meeting
    private float exeute_time;
    float[] statusValues = new float[5];
    float[] statusGainProportion = new float[5];
    float[] statusTimer = new float[5]{ 0f, 0f, 0f, 0f, 0f };
    GameObject[] taskPoints;

    private MurdererStatusHandler statusBar;
    private UnityEngine.AI.NavMeshAgent navAgent;
    private int agentStuckTimes;
    private float agentRemainingDis;
    private float printTimeElapse;
    [SerializeField] private float printTimeGap = 0.15f;
    [SerializeField] public GameObject footprint;

    void Start()
    {
        ResetTimer();
        ResetAgentDtection();
        exeute_time = 0.0f;
        navAgent = GetComponent<NavMeshAgent>();
        taskPoints = GameObject.FindGameObjectsWithTag("Task");
        GameObject StatusBarObj = GetComponentInChildren<MurdererStatusHandler>().gameObject;
        statusBar = StatusBarObj.GetComponent<MurdererStatusHandler>(); // Do not use it at Start
        if (navAgent == null) Debug.LogError("Unable to find NavMeshAgent");
        if (statusBar == null) Debug.LogError("Unable to find MurdererStatusHandler");
        if (footprint == null) footprint = Resources.Load<GameObject>("PreFab/FootPrintBad");

        // Status Initialize
        nowStatus = -1;
        statusValues[0] = Random.Range(0.4f, 0.6f); statusGainProportion[0] = 40f;
        statusValues[1] = Random.Range(0.4f, 1.0f); statusGainProportion[1] = 30f;
        statusValues[2] = Random.Range(0.2f, 0.6f); statusGainProportion[2] = 20f;
        statusValues[3] = Random.Range(0.0f, 0.4f); statusGainProportion[3] = 8f;
        statusValues[4] = Random.Range(0.0f, 0.1f); statusGainProportion[4] = 2f;
    }
    void Update()
    {
        UpdatePrams();
        if (nowStatus >= 0 && nowStatus < statusValues.Length) {
            // switch nowStatus:
            //     case 0:
            //     case 1:
            //     case 2:
            //     case 3:
            //     case 4:
        }
        else if (nowStatus == -2) { // Table Meeting
            statusBar.Select(-2);
            CutAgentPath();
            ResetTimer();

        }
        else {  // Select an Action and distribute energy
            nowStatus = status_select(statusValues);
            statusBar.Select(nowStatus);

            float energy = statusValues[nowStatus];
            energy *= Random.Range(0.0f, 1.0f);
            distribute_energy(nowStatus, energy);
        }

        statusBar.Show();
    }
    
    private int status_select(float[] probabilities)
    {
        System.Random rand = new System.Random();
        float sum = 0, cumulativeProb = 0;
        foreach (float p in probabilities) sum += p;

        float randVal = (float) rand.NextDouble() * sum;
        for (int i = 0; i < probabilities.Length; i++) {
            cumulativeProb += probabilities[i];
            if (randVal <= cumulativeProb) return i;
        }
        return probabilities.Length - 1;
    }
    private void distribute_energy(int from, float energy)
    {
        statusValues[from] -= energy;
        float Denominator = 0.0f;
        for (int i = 0; i < statusGainProportion.Length; i++ ) {
            if (i != from) Denominator += statusGainProportion[i];
        }
        for (int i = 0; i < statusGainProportion.Length; i++ ) {
            if (i != from) {
                float gain = energy * (statusGainProportion[i] / Denominator);
                statusValues[i] = Mathf.Clamp01(statusValues[i] + gain);
            }
        }
    }
    private int Idle(float time = -1.0f, float speed = 5.0f) // Loitering without intention
    { 
        statusTimer[0] += Time.deltaTime;
        if (!navAgent.hasPath) {
            ResetAgentDtection();
            Vector3 randomDestination = RandomNavmeshLocation(30.0f);
            navAgent.speed = speed;
            navAgent.SetDestination(randomDestination);
        }
        else if (DetectAgentStuck()) CutAgentPath();

        if (statusTimer[0] >= time && time > 0) {
            ResetTimer(0);
            CutAgentPath();
            return -1;
        }
        return 0;
    }

    // API Functions
    private bool ToPosition(Vector3 dest, float speed = 10f)
    {
        Vector3 fixdest = GetNavMeshProjection(dest);
        float distanceToDest = Vector3.Distance(navAgent.transform.position, fixdest);
        if (!navAgent.hasPath) {
            ResetAgentDtection();
            navAgent.SetDestination(fixdest);
        }
        else if (DetectAgentStuck(maxStuckTimes: 1) && distanceToDest < 0.1) return true;
        if (distanceToDest > 10f) navAgent.speed = speed;
        else navAgent.speed = distanceToDest / 2 + 2.5f;
        return false;
    }
    private Vector3 GetNavMeshProjection(Vector3 position)
    {
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(position, out navHit, 30.0f, NavMesh.AllAreas)) {
            return navHit.position;
        }
        else {
            Debug.LogWarning("Unable to find valid NavMesh position for the target.");
            return position;
        }
    }
    private Vector3 RandomNavmeshLocation(float range)
    {
        Vector3 randomDir = Random.insideUnitSphere * range;
        return GetNavMeshProjection(randomDir + transform.position);
    }
    public void CutAgentPath()
    {
        ResetAgentDtection();
        navAgent.speed = 0f;
        navAgent.ResetPath();
    }
    void ResetTimer(int index = -1)
    {
        if (index >= 0) statusTimer[index] = 0f;
        else
            for (int i = 0; i < statusTimer.Length; i++) statusTimer[i] = 0f;
    }
    void ResetAgentDtection()
    {
        agentStuckTimes = 0;
        agentRemainingDis = 0.0f;;
    }
    bool DetectAgentStuck(float tolerance = 0.05f, int maxStuckTimes = 500)
    {
        // Find navAgent stuck at the same position for to many frame
        if (Mathf.Abs(agentRemainingDis - navAgent.remainingDistance) < tolerance) agentStuckTimes+= 1;
        else agentStuckTimes = 0;
        agentRemainingDis = navAgent.remainingDistance;
        // Debug.Log("remainingDistance :" + agentRemainingDis + " AgentStuckTimes: " + agentStuckTimes);
        if (agentStuckTimes >= maxStuckTimes) return true;
        return false;
    }
    void LeaveFootPrint(Vector3 pos)
    {
        GameObject newprint = footprint;
        Instantiate(newprint);
        newprint.transform.position = pos;
        printTimeElapse = 0f;
    }
    
    void UpdatePrams()
    {
        if (printTimeElapse >= printTimeGap) LeaveFootPrint(transform.position);
        statusBar.update_idle(statusValues[0]);
        statusBar.update_chaos(statusValues[1]);
        statusBar.update_tailgating(statusValues[2]);
        // statusBar.update_assassinate(statusValues[3]);
        // statusBar.update_killingSpree(statusValues[4]);
        printTimeElapse += Time.deltaTime;
    }
}
