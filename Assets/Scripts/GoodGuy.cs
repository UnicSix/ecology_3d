using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoodGuy : MonoBehaviour
{
    private Vector3 goal;
    private NavMeshAgent _agent;
    private NavMeshHit hit;
    [SerializeField] private AnimationClip[] myClips;
    private Animator animator;
    [SerializeField] private float range = 30.0f;
    public Camera Cam;
    void Start()
    {
        animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
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
                _agent.destination = hit.point; 
            }
        }

        if (_agent.remainingDistance==0f)
        {
            Debug.Log("Stop");
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
