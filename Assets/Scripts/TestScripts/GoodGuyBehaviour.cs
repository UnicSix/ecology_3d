using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class WorkParameters
{
    public int task_index { get; set; }
    public bool arrived { get; set; }
    public WorkParameters()
    {
        Reset();
    }
    public void Reset()
    {
        task_index = -1;
        arrived = false;
    }
}


public class GoodGuyBehaviour : MonoBehaviour
{

    public VisionCone fovcone;
    public float sus;
    public int nowStatus; // -1: none, -2: meeting
    private float exeuteTime;
    float[] statusValues = new float[4];
    float[] statusGainProportion = new float[4];
    float[] statusTimer = new float[4]{ 0f, 0f, 0f, 0f };
    GameObject[] taskPoints;
    Transform alarmPoint;

    public GameObject deadBody;
    private WorkerStatusHandler statusBar;
    private NavMeshAgent navAgent;
    private int agentStuckTimes;
    private float agentRemainingDis;
    private float printTimeElapse;
    private WorkParameters workParams;
    [SerializeField] private float energyFactor = 20.0f;
    [SerializeField] private float printTimeGap = 0.15f;
    [SerializeField] public GameObject footprint;
    // private bool arrive = false;
    // private bool back = false; // temp

    void Start()
    {
        ResetTimer();
        ResetAgentDtection();
        sus = 0.0f;
        exeuteTime = 0.0f;
        workParams = new WorkParameters();
        navAgent = GetComponent<NavMeshAgent>();
        taskPoints = GameObject.FindGameObjectsWithTag("Task");
        GameObject[] alarmPoints = GameObject.FindGameObjectsWithTag("Alarm");
        alarmPoint = alarmPoints[Random.Range(0, alarmPoints.Length)].transform;
        GameObject StatusBarObj = GetComponentInChildren<WorkerStatusHandler>().gameObject;
        statusBar = StatusBarObj.GetComponent<WorkerStatusHandler>(); // Do not use it at Start
        deadBody = Resources.Load<GameObject>("PreFab/MyPreFab/GoodGuyDead");
        if (navAgent == null) Debug.LogError("Unable to find NavMeshAgent");
        if (statusBar == null) Debug.LogError("Unable to find WorkerStatusHandler");
        if (footprint == null) footprint = Resources.Load<GameObject>("PreFab/FootPrintGood");

        // Status Initialize
        nowStatus = -1;
        statusValues[0] = Random.Range(0.4f, 0.6f); statusGainProportion[0] = 50f;
        statusValues[1] = Random.Range(0.6f, 1.0f); statusGainProportion[1] = 40f;
        statusValues[2] = Random.Range(0.0f, 0.6f); statusGainProportion[2] = 8f;
        statusValues[3] = Random.Range(0.0f, 0.2f); statusGainProportion[3] = 2f;
    }
    void Update()
    {
        UpdatePrams();
        if (nowStatus >= 0 && nowStatus <= statusValues.Length) {
            switch (nowStatus) {
                case 0: 
                    nowStatus = Idle(exeuteTime);
                    break;
                case 1: 
                    nowStatus = Work(exeuteTime);
                    break;
                case 2:
                    nowStatus = Panic(exeuteTime);
                    break;
                case 3:
                    nowStatus = Report(findBody: false);
                    break;
            }
        }
        else if (nowStatus == -2) { // Table Meeting
            statusBar.Select(-2);
            CutAgentPath();
            ResetTimer();
        }
        else { // Select an Action and distribute energy
            nowStatus = status_select(statusValues);
            statusBar.Select(nowStatus);

            float energy = statusValues[nowStatus];
            energy *= Random.Range(0.1f, 1.0f);
            exeuteTime = energy * energyFactor;
            distribute_energy(nowStatus, energy);
        }

        // Scan body every update
        ScanBody();
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
    private int Idle(float time = -1.0f, float speed = 4.0f) // Loitering without intention
    { 
        statusTimer[0] += Time.deltaTime;
        if (!navAgent.hasPath) {
            ResetAgentDtection();
            Vector3 randomDestination = RandomNavmeshLocation(range: 50.0f);
            navAgent.speed = speed;
            navAgent.SetDestination(randomDestination);
        }
        else if (DetectAgentStuck(maxStuckTimes: 1000000)) CutAgentPath();

        if (statusTimer[0] >= time && time > 0) {
            ResetTimer(0);
            CutAgentPath();
            return -1;
        }
        return 0;
    }
    private int Work(float time = -1.0f, float efficiency = 1.0f) // Find Task Positon at work(or wreck)
    {
        if (workParams.task_index == -1) {
            int get_index = -1;
            while (get_index == workParams.task_index){
                get_index = Random.Range(0, taskPoints.Length);
            }
            workParams.task_index = get_index;
        }

        ProgressStatusHandler handler = taskPoints[workParams.task_index].GetComponentInChildren<ProgressStatusHandler>();
        if (!workParams.arrived) {
            Vector3 end_pos = taskPoints[workParams.task_index].transform.position;
            end_pos.y = navAgent.transform.position.y;
            float distanceToPos = Vector3.Distance(navAgent.transform.position, end_pos);

            if (handler.IsOccupied() && distanceToPos <= 12.0f) {
                workParams.task_index = -1;
                CutAgentPath();
            }
            else
                workParams.arrived = ToPosition(end_pos);
            if (!workParams.arrived && distanceToPos <= 0.5f)
                workParams.arrived = true;
        }
        else {
            FacePosition(GetNavMeshProjection(taskPoints[workParams.task_index].transform.Find("Facing").position));
            ToPosition(taskPoints[workParams.task_index].transform.position);
            if (handler.IncreaseWorkProgress(efficiency * Time.deltaTime / 10.0f)) {
                workParams.task_index = -1;
                workParams.arrived = false;
                CutAgentPath();
            }
            else handler.Occupying();
            statusTimer[1] += Time.deltaTime;
        }

        if (statusTimer[1] >= time) {
            if (workParams.task_index != -1)
                handler.Leave();
            workParams.Reset();
            CutAgentPath();
            ResetTimer(1);
            return -1;
        }
        return 1;
    }
    private int Panic(float time = -1.0f, float speed = 30.0f) // Run away from any guy
    {
        statusTimer[1] += Time.deltaTime;
        if (!navAgent.hasPath) {
            ResetAgentDtection();
            Vector3 randomDestination = RandomNavmeshLocation(range: 15.0f, minAngleTurn: 100.0f);
            navAgent.speed = speed;
            navAgent.SetDestination(randomDestination);
            // if FOV see Guy, CutAgentPath()
        }
        else if (DetectAgentStuck(maxStuckTimes: 100000)) CutAgentPath();

        if (statusTimer[1] >= time) {
            ResetTimer(1);
            CutAgentPath();
            return -1;
        }
        return 2;
    }
    private int Report(bool findBody = false, float speed = 12.0f)
    {
        if (findBody) {
            GameObject.Find("MasterControl").GetComponent<Control>().Meeting();
            Debug.Log("Call meeting !!!");
            return -2;
        }
        else {
            if (!ToPosition(alarmPoint.position, 12.0f)) {
                return 3;
            }
            else {
                GameObject.Find("MasterControl").GetComponent<Control>().Meeting();
                Debug.Log("Call meeting !!!");
                return -2;
            }
        }
    }
    private void ScanBody()
    {
        // FOV check body
        // if find: Report(findBodyL true);
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
        else if (DetectAgentStuck(maxStuckTimes: 1) && distanceToDest <= 0.1f) return true;
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
    private Vector3 RandomNavmeshLocation(float range, float minAngleTurn = -1.0f)
    {
        Vector3 randomDir = Vector3.zero;
        do
        {
            randomDir = Random.insideUnitSphere * range;
            randomDir.y = 0;
            float angle = Vector3.Angle(transform.forward, randomDir);

            if (angle >= minAngleTurn) break;
        } while (true);

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
        statusBar.update_sus(sus);
        statusBar.update_idle(statusValues[0]);
        statusBar.update_work(statusValues[1]);
        statusBar.update_panic(statusValues[2]);
        statusBar.update_alarm(statusValues[3]);
        printTimeElapse += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            GameObject newDeadBody = deadBody;
            newDeadBody.transform.position = new Vector3(transform.position.x, -2.7f, transform.position.z);
            Instantiate(newDeadBody);
            if (newDeadBody != null)
                Debug.Log("ins");
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
    }
}
