using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoodGuy : MonoBehaviour
{
    private Vector3 goal;
    private NavMeshAgent _agent;
    private NavMeshHit hit;
    private List<Vector3> taskPositions;
    [SerializeField] private AnimationClip[] myClips;
    private Animator animator;
    [Range(10f,50f)]
    public float range = 30.0f;
    public Camera Cam;
    void Start()
    {
        animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        Cam = Camera.main;
        taskPositions = new List<Vector3>();

        // Assuming task objects are tagged as "TaskObject"
        GameObject[] taskObjects = GameObject.FindGameObjectsWithTag("Task");

        foreach (GameObject taskObject in taskObjects)
        {
            taskPositions.Add(taskObject.transform.position);
        }
        foreach (Vector3 pos in taskPositions)
        {
            Debug.Log("Task Object Position: " + pos);
        }
    }

    void Update()
    {
        Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            goal= Input.mousePosition;
            if( Physics.Raycast(ray, out hit) )
            {
                Debug.Log(hit.point);
                Vector3 dest = new Vector3(hit.point.x, hit.point.y+1.5f, hit.point.z);
                _agent.destination = dest; 
            }
        }

        if (_agent.remainingDistance == 0f)
        {
            Walk();
        }
    }

    void Walk()
    {
        // Sample the position. If within 'range' of the sourcePosition, it finds the nearest point on a NavMesh
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * range;
        if (NavMesh.SamplePosition(randomPoint, out hit, range, NavMesh.AllAreas))
        {
            _agent.destination = randomPoint; 
        }
    }
}
