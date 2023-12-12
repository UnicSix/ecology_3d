using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoodGuyBevaviour : MonoBehaviour
{
    
    private int nowStatus;
    float[] statusValues = new float[4];
    // float[] statusGain = new float[4];
    float[] statusTimer = new float[4]{ 0f, 0f, 0f, 0f };
    GameObject[] taskPoints;

    private WorkerStatusHandler statusBar;
    private NavMeshAgent navAgent;
    private int agentStuckTimes;
    private float agentRemainingDis;
    private float printTimeElapse;
    [SerializeField] private float printTimeGap = 0.15f;
    [SerializeField] public GameObject footprint;
    // private bool arrive = false;
    // private bool back = false; // temp

    void Start()
    {
        ResetTimer();
        ResetAgentDtection();
        navAgent = GetComponent<NavMeshAgent>();
        taskPoints = GameObject.FindGameObjectsWithTag("Task");
        GameObject StatusBarObj = GetComponentInChildren<WorkerStatusHandler>().gameObject;
        statusBar = StatusBarObj.GetComponent<WorkerStatusHandler>(); // Do not use it at Start
        if (navAgent == null) Debug.LogError("Unable to find NavMeshAgent");
        if (statusBar == null) Debug.LogError("Unable to find WorkerStatusHandler");
        if (footprint == null) footprint = Resources.Load<GameObject>("PreFab/FootPrintGood");

        // Status Initialize
        statusValues[0] = Random.Range(0.4f, 0.6f);
        statusValues[1] = Random.Range(0.6f, 1.0f);
        statusValues[2] = Random.Range(0.0f, 0.6f);
        statusValues[3] = Random.Range(0.0f, 0.2f);
        nowStatus = status_select(statusValues);
        statusBar.Select(nowStatus);
    }
    void Update()
    {
        UpdatePrams();
        if (nowStatus != -1) {
            // switch nowStatus:
            //     case 0:
            //     case 1:
            //     case 2:
            //     case 3:
        }
        else if (nowStatus == -2) { // Table Meeting
            CutAgentPath();
            ResetTimer();
        }
        else { // Select an Action and cost energy
            nowStatus = status_select(statusValues);
            statusBar.Select(nowStatus);
        }

        // if (!arrive) arrive = ToPosition(taskPoints[0].transform.position);
        // else {
        //     if (!back){
        //         CutAgentPath();
        //         FacePosition(GetNavMeshProjection(taskPoints[0].transform.Find("Facing").position));
        //         taskPoints[0].GetComponentInChildren<ProgressStatusHandler>().progress_val+= 0.8f;
        //         back = true;
        //     }
        //     else {
        //         // ToPosition(new Vector3(8.347f, -4.211149f, -7.73f));
        //     }
        // }

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
            if (randVal < cumulativeProb) return i;
        }
        return probabilities.Length - 1;
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
    private int Work(float time = -1.0f, float efficiency = 1.0f) // Find Task Positon at work(or wreck)
    {
        // if on position:
            statusTimer[1] += Time.deltaTime;
            // taskPoints[0].GetComponentInChildren<ProgressStatusHandler>().progress_val += efficiency / 100
        // else:
            // search for empty posotion

        if (statusTimer[1] >= time) {
            ResetTimer(1);
            return -1;
        }
        return 1;
    }
    
    // API Functions
    private bool ToPosition(Vector3 dest, float speed = 10f)
    {
        Vector3 fixdest = GetNavMeshProjection(dest);
        float distanceToDest = Vector3.Distance(navAgent.transform.position,fixdest);
        if (!navAgent.hasPath) {
            ResetAgentDtection();
            navAgent.SetDestination(fixdest);
        }
        else if (DetectAgentStuck(maxStuckTimes: 1) && distanceToDest < 0.1) return true;
        if (distanceToDest > 10f) navAgent.speed = speed;
        else navAgent.speed = distanceToDest / 2 + 2.5f;
        return false;
    }
    private void FacePosition(Vector3 dest) 
    {
        transform.LookAt(dest);
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
    void CutAgentPath()
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
    bool DetectAgentStuck(float tolerance = 0.05f, int maxStuckTimes = 100)
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
        statusBar.update_work(statusValues[1]);
        statusBar.update_panic(statusValues[2]);
        statusBar.update_alarm(statusValues[3]);
        printTimeElapse += Time.deltaTime;
    }
}
