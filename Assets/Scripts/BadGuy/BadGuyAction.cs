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
    [Range(10f,50f)] public float range = 30.0f;
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
        Kill();

        // foreach (int i in Status.actionIntent)
        // {
        //     Debug.Log(i);
        // }

        // Roam();

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
        // Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
        // RaycastHit hit = new RaycastHit();
        printTime += Time.deltaTime;
        // if (Input.GetKeyDown(KeyCode.Mouse0))
        // {
        //     goal= Input.mousePosition;
        //     if( Physics.Raycast(ray, out hit) )
        //     {
        //         Debug.Log(hit.point);
        //         Vector3 dest = new Vector3(hit.point.x, hit.point.y+1.5f, hit.point.z);
        //         _agent.destination = dest; 
        //     }
        // }

            // switch (SelectAction())
            // {
            //     case BadGuyStatus.ROAM:
            //         Roam(); break;
            //     case BadGuyStatus.TRACK:
            //         Track(); break;
            //     case BadGuyStatus.KILL:
            //         Kill(); break;
            //     case BadGuyStatus.CAMP:
            //         Camp(); break;
            //     case BadGuyStatus.WRECK:
            //         Wreck(); break;
            // }
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
        if(_agent.SetDestination(pos))
            Debug.Log("tracking"+pos);
        
    }
    public void Wreck()
    {
        Debug.Log("Wreck");
    }
    public void Camp()
    {
        Debug.Log("Camp");
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

    int SelectAction()
    {
        float rd = UnityEngine.Random.value;
        float cumulativeProbability=0;
        float[] intentToPossibility = new float[5];
        float sum=0;
        foreach (int i in Status.actionIntent)
            sum += i;
        int accuIntent = 0;
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

    public void Kill()
    {
        GameObject weapon = Resources.Load<GameObject>("PreFab/Weapons/Hammer");
        weapon.transform.localPosition = Vector3.forward * 1.3f + Vector3.up * 2.8f + Vector3.left * 0f;
    weapon.transform.localRotation = Quaternion.Euler(0f, 0f, 90f );
        Instantiate(weapon, this.transform);
    }

    void WalkToFootprint(Vector3 footPrintPos)
    {
        
    }
}

