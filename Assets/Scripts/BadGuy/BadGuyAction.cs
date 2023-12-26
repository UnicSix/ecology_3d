using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.Windows.WebCam;

public class BadGuy: MonoBehaviour, IVisionTrigger, IMoveable
{
    public float sus;
    private NavMeshHit hit;
    [Range(10f,50f)] public float range = 20.0f;
    [SerializeField] private GameObject footprint;
    public GameObject[] taskPoints;
    private float printTime;
    private float eraseVisionInfo = 0;
    private float killEnergy = 0;
    private float wreckEnergy = 0;
    private float trackEnergy = 0;
    private float killThreshold = 15f;
    private float trackThreshold = 3f;
    private float wreckThreshold = 7f;
    public int nowStatus;
    public MurdererStatusHandler statusBar;
    private const float energyLimit=100f;
    
    public List<Vector3> taskPositions;
    
    public BadGuyStateMachine StateMachine { get; set; }
    public TrackState trackState { get; set; }
    public IdleState idleState { get; set; }
    public RoamState roamState { get; set; }
    public KillState killState { get; set; }
    public WreckState wreckState { get; set; }
    // private float randMin=100f;
    // private float randMax=0f;

    private void Awake()
    {
        StateMachine = new BadGuyStateMachine();
        trackState = new TrackState(this, StateMachine);
        idleState = new IdleState(this, StateMachine);
        roamState = new RoamState(this, StateMachine);
        killState = new KillState(this, StateMachine);
        wreckState = new WreckState(this, StateMachine);

        killEnergy = 40;
        trackEnergy = 90;
        wreckEnergy = 10;
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        taskPoints = GameObject.FindGameObjectsWithTag("Task");
        taskPositions = new List<Vector3>();
        foreach(GameObject taskPoint in taskPoints)
        {
            taskPositions.Add(taskPoint.transform.position);
        }
        if (footprint == null)
            footprint = Resources.Load<GameObject>("PreFab/FootPrintBad");
        printTime = 0f;
        
        StateMachine.Initialize(idleState);
        // StateMachine.Initialize(idleState);
    }

    void Update()
    {
        printTime += Time.deltaTime;
        eraseVisionInfo += Time.deltaTime;
        if (printTime > 0.25f)
        {
            GameObject newprint = footprint;
            newprint.transform.position = transform.position;
            Instantiate(newprint);
            printTime = 0f;
        }

        if (eraseVisionInfo > 0.25f)
        {
            SetSeenGuy(false);
            SetSeenFootprint(false);
        }
        StateMachine.CurBadGuyState.FrameUpdate();
        
        statusBar.update_sus(sus);
        statusBar.update_kill(killEnergy);
        statusBar.update_track(trackEnergy);
        statusBar.update_wreck(wreckEnergy);
        
    }

    public void CutAgentPath()
    {
        agent.ResetPath();
        agent.speed = 0;
    }
    public bool seenFootprint { get; set; }
    public Vector3 footprintPos { get; set; }
    public bool seenGuy { get; set; }
    public Vector3 guyPos { get; set; }

    public void SetSeenFootprint(bool seen)
    {
        seenFootprint = seen;
    }

    public void SetSeenGuy(bool seen)
    {
        seenGuy = seen;
    }

    public void SetFootprintPos(Vector3 pos)
    {
        footprintPos = pos;
    }

    public void SetGuyPos(Vector3 pos)
    {
        guyPos = pos;
    }

    public NavMeshAgent agent { get; set; }
    // public void MoveToPos(Vector3 pos)
    // {
        // throw new System.NotImplementedException();
    // }

    public void setKillEnergy(float val)
    {
        if(killEnergy<100)
            killEnergy += val;
    }
    public void setTrackEnergy(float val)
    {
        if(trackEnergy<100)
            trackEnergy += val;
    }
    public void setWreckEnergy(float val)
    {
        if(wreckEnergy<100)
            wreckEnergy += val;
    }
    public float getTrackEnergy()
    {
        return trackEnergy;
    }
    public float getWreckEnergy()
    {
        return wreckEnergy;
    }
    public float getKillEnergy()
    {
        return killEnergy;
    }

    public float getKillThreshold()
    {
        return killThreshold;
    }
    public float getTrackThreshold()
    {
        return trackThreshold;
    }
    public float getWreckThreshold()
    {
        return wreckThreshold;
    }
}

