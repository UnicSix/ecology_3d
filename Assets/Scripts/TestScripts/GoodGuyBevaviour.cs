using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoodGuyBevaviour : MonoBehaviour
{
    GameObject[] taskPoints;
    // private int nowStatus = 1;
    // energy = 0.11
    // float[] statusValues = new float[3];
    // float[] statusGain = new float[3];
    private WorkerStatusHandler statusBar;
    private NavMeshAgent navAgent;
    private int agentStuckTimes;
    private float agentRemainingDis;
    private float printTimeElapse;
    private bool arrive = false;
    [SerializeField] private float printTimeGap = 0.15f;
    [SerializeField] public GameObject footprint;
    private bool back = false; // temp

    void Start()
    {
        ResetAgentDtection();
        navAgent = GetComponent<NavMeshAgent>();
        GameObject StatusBarObj = GetComponentInChildren<WorkerStatusHandler>().gameObject;
        statusBar = StatusBarObj.GetComponent<WorkerStatusHandler>(); // Do not use it at Start
        if (navAgent == null) Debug.LogError("Unable to find NavMeshAgent");
        if (statusBar == null) Debug.LogError("Unable to find WorkerStatusHandler");
        if (footprint == null) footprint = Resources.Load<GameObject>("PreFab/FootPrintGood");

        // Find all Task points
        taskPoints = GameObject.FindGameObjectsWithTag("Task");
        if (taskPoints.Length != 0) Debug.Log("Found " + taskPoints.Length + " TaskPoints.");
        else Debug.LogWarning("No TaskPoints found in the scene.");
    }
    void Update()
    {
        UpdatePrams();
        // Idle(5.0f);
        if (!arrive) arrive = ToPosition(taskPoints[0].transform.position, 10f);
        else {
            if (!back){
                Debug.Log("Arrived !!");
                CutAgentPath();
                transform.LookAt(GetNavMeshProjection(new Vector3(4.743f, 0.0f, 15.308f)));
                back = true;
            }
            else {
                ToPosition(new Vector3(8.347f, -4.211149f, -7.73f), 10f);
            }
        }

        if (printTimeElapse >= printTimeGap) LeaveFootPrint(transform.position);
    }
    
    private void Idle(float speed){ // Loitering without intent
        if (!navAgent.hasPath) {
            ResetAgentDtection();
            Vector3 randomDestination = RandomNavmeshLocation(30.0f);
            navAgent.speed = speed;
            navAgent.SetDestination(randomDestination);
        }
        else if (DetectAgentStuck()) CutAgentPath();
    }
    private bool ToPosition(Vector3 dest, float speed)
    {
        Vector3 fixdest = GetNavMeshProjection(dest);
        float distanceToDest = Vector3.Distance(navAgent.transform.position,fixdest);
        if (!navAgent.hasPath) {
            ResetAgentDtection();
            navAgent.SetDestination(fixdest);
            navAgent.speed = speed;
        }
        else if (DetectAgentStuck(maxStuckTimes: 1) && distanceToDest < 0.1) return true;
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
    void CutAgentPath()
    {
        ResetAgentDtection();
        navAgent.ResetPath();
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
        printTimeElapse += Time.deltaTime;
    }
}
