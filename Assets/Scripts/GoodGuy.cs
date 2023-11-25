using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoodGuy : MonoBehaviour
{
    private Vector3 goal;
    private NavMeshAgent _agent;
    private NavMeshHit hit;
    [SerializeField] private float range = 30.0f;
    public Camera Cam;
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        StartCoroutine(RandomWalk());
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space");
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

    IEnumerator RandomWalk()
    {
        while (true)
        {
            yield return new WaitForSeconds(15.0f + Random.value * 5.0f);
            Walk();
        }
    }
}
