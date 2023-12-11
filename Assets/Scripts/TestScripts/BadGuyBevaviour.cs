using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BadGuyBevaviour : MonoBehaviour
{
    // private int nowStatus = 0;
    // float[] statusValues = new float[4];
    // float[] statusGain = new float[4];
    private MurdererStatusHandler statusBar;
    private UnityEngine.AI.NavMeshAgent navAgent;
    private int agentStuckTimes;
    private float agentRemainingDis;
    private float printTimeElapse;
    [SerializeField] private float printTimeGap = 0.15f;
    [SerializeField] public GameObject footprint;

    void Start()
    {
        ResetAgentDtection();
        navAgent = GetComponent<NavMeshAgent>();
        GameObject StatusBarObj = GetComponentInChildren<MurdererStatusHandler>().gameObject;
        statusBar = StatusBarObj.GetComponent<MurdererStatusHandler>(); // Do not use it at Start
        if (navAgent == null) Debug.LogError("Unable to find NavMeshAgent");
        if (statusBar == null) Debug.LogError("Unable to find MurdererStatusHandler");
        if (footprint == null) footprint = Resources.Load<GameObject>("PreFab/FootPrintBad");

    }
    void Update()
    {
        UpdatePrams();
        Idle(5.0f);
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
