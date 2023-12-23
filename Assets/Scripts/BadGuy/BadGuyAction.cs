using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows.WebCam;

public class BadGuy: MonoBehaviour, IVisionTrigger, IMoveable
{
    private NavMeshHit hit;
    private List<Vector3> taskPositions;
    [SerializeField] private AnimationClip[] myClips;
    [Range(10f,50f)] public float range = 20.0f;
    [SerializeField] public GameObject footprint;
    private float printTime;
    public float[] actionIntent;
    
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
        
        actionIntent = new float[5]{10, 10,10,10,10};
    }
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        taskPositions = new List<Vector3>();
        if (footprint == null)
            footprint = Resources.Load<GameObject>("PreFab/FootPrintBad");
        printTime = 0f;
        
        StateMachine.Initialize(idleState);
    }

    void Update()
    {
        printTime += Time.deltaTime;
        if (printTime > 0.25f)
        {
            GameObject newprint = footprint;
            Instantiate(newprint);
            newprint.transform.position = transform.position;
            printTime = 0f;
        }
        StateMachine.CurBadGuyState.FrameUpdate();
        
    }

    public bool seenFootprint { get; set; }
    public Vector3 footprintPos { get; set; }
    public bool seenGuy { get; set; }
    public Vector3 guyPos { get; set; }

    public void SetSeenFootprint(bool seenFootprint)
    {
        seenFootprint = seenFootprint;
    }

    public void SetSeenGuy(bool seenGuy)
    {
        seenGuy = seenGuy;
    }

    public void SetFootprintPos(Vector3 pos)
    {
        footprintPos = pos;
    }

    public void SetGuyPos(Vector3 pos)
    {
        guyPos = pos;
    }

    public NavMeshAgent _agent { get; set; }
    // public void MoveToPos(Vector3 pos)
    // {
        // throw new System.NotImplementedException();
    // }
}

