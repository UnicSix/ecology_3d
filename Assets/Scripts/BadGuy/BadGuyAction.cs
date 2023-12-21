using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows.WebCam;

public class BadGuy : MonoBehaviour
{
    public BadGuyStatus Status;
    private Vector3 goal;
    private NavMeshAgent _agent;
    private NavMeshHit hit;
    private List<Vector3> taskPositions;
    [SerializeField] private AnimationClip[] myClips;
    private Animator animator;
    [Range(10f,50f)] public float range = 20.0f;
    private Camera Cam;
    [SerializeField] public GameObject footprint;
    private float printTime;
    // private float randMin=100f;
    // private float randMax=0f;
    void Start()
    {
        animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        Cam = Camera.main;
        taskPositions = new List<Vector3>();
        if (footprint == null)
            footprint = Resources.Load<GameObject>("PreFab/FootPrintBad");
        printTime = 0f;
        // Roam();
        // Kill();
        // Assuming task objects are tagged as "TaskObject"
        // GameObject[] taskObjects = GameObject.FindGameObjectsWithTag("Task");
        //
        // foreach (GameObject taskObject in taskObjects)
        // {
        //     taskPositions.Add(taskObject.transform.position);
        // }
        // foreach (Vector3 pos in taskPositions)
        // {
        //     Debug.Log("Task Object Position: " + pos);
        // }

        // var status = GetComponent<>();
        // Debug.Log();
    }

    void Update()
    {
        // switch (SelectAction())
        // {
        //     case BadGuyStatus.ROAM:
        //         Roam(); break;
        //     case BadGuyStatus.KILL:
        //         Kill(); break;
        //     case BadGuyStatus.WRECK:
        //         Wreck(); break;
        //     case BadGuyStatus.IDLE:
        //         Idle(); break;
        // }
        printTime += Time.deltaTime;
        if (printTime > 0.25f)
        {
            GameObject newprint = footprint;
            Instantiate(newprint);
            newprint.transform.position = transform.position;
            printTime = 0f;
        }
        
    }

    public void Track(Vector3 pos)
    {
        if (Status.curAction == "roam" || Status.curAction == "track" || Status.curAction == "idle")
        {
            Status.curAction = "track";
            _agent.SetDestination(pos);
            // if(_agent.SetDestination(pos))
            // Debug.Log("tracking"+pos);
        }
    }

    public void PplInsight(Vector3 pplPos)
    {
        Debug.Log(pplPos);
    }
    public void Kill()
    {
        GameObject weapon = Resources.Load<GameObject>("PreFab/Weapons/Hammer");
        weapon.transform.localPosition = Vector3.forward * 1.3f + Vector3.up * 2.8f + Vector3.left * 0f;
        weapon.transform.localRotation = Quaternion.Euler(90f, 0f, 90f );
        Instantiate(weapon, this.transform);
        Status.curAction = "roam";
    }
    //walk randomly look around to find fresh footprints
    public void Roam()
    {
        // Sample the position. If within 'range' of the sourcePosition, it finds the nearest point on a NavMesh
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * range;
        if (NavMesh.SamplePosition(randomPoint, out hit, range, NavMesh.AllAreas))
        {
            _agent.destination = randomPoint; 
        }
    }
    
    void LookAround() {
        float rotationSpeed = 20f; // Rotation speed in degrees per second
        float angle = Mathf.Sin(Time.time) * rotationSpeed; // Creates a back-and-forth rotation
        transform.Rotate(Vector3.up, angle * Time.deltaTime);
    }
    public void Wreck()
    {
        Debug.Log("Wreck");
    }
    public void Camp()
    {
        Debug.Log("Camp");
    }


    int SelectAction()
    {
        float rd = UnityEngine.Random.value;
        float cumulativeProbability=0;
        float[] intentToPossibility = new float[5];
        float sum=0;
        foreach (int i in Status.actionIntent)
            sum += i;
        float accuIntent = 0;
        for(int i=0; i<5; i++)
        {
            accuIntent += Status.actionIntent[i];
            intentToPossibility[i] = accuIntent / sum;
        }

        int nextAction;
        for(nextAction=0; nextAction<5; nextAction++)
        {
            cumulativeProbability += intentToPossibility[nextAction];
            if (rd <= cumulativeProbability)
            {
                Debug.Log("next action: "+nextAction);
                return nextAction;
            }
        }
        return 0;
    }
}

