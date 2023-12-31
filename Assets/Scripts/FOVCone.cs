using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class VisionCone : MonoBehaviour
{
    public Material VisionConeMaterial;
    [SerializeField] private float VisionRange;
    [SerializeField] private float VisionAngle;
    public LayerMask obstacleMask;//layer with objects that obstruct the enemy view, like walls, for example
    public LayerMask targetMask;//layer with objects that obstruct the enemy view, like walls, for example
    [SerializeField] private int VisionConeResolution;//the vision cone will be made up of triangles, the higher this value is the pretier the vision cone will be
    Mesh VisionConeMesh;
    MeshFilter MeshFilter_;
    public UnityVector3Event onFootprintLockedOn;

    public bool seenGuy = false;
    public bool seenBody = false;
    //Create all of these variables, most of them are self explanatory, but for the ones that aren't i've added a comment to clue you in on what they do
    //for the ones that you dont understand dont worry, just follow along
    void Start()
    {
        transform.AddComponent<MeshRenderer>().material = VisionConeMaterial;
        MeshFilter_ = transform.AddComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
        VisionAngle = 90f;
        VisionAngle *= Mathf.Deg2Rad;
        VisionRange = 25f;
        VisionConeResolution = 25;
        if (gameObject.CompareTag("BadGuy"))
        {
            VisionAngle = 15f;
            VisionAngle *= Mathf.Deg2Rad;
            VisionRange = 45;
        }
    }

    //draw the fov cone, return the position of the most fresh spotted footprint
    void Update()
    {
        DrawVisionCone();//calling the vision cone function everyframe just so the cone is updated every frame
    }
    
    void FindVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, VisionRange, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < VisionAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    // Add logic here for when a target is in the field of view
                    Debug.Log("Target in view: " + target.name);
                }
            }
        }
    }
    
    void AdjustSphereColliderToCone()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }

        // Set the radius to match the vision range
        sphereCollider.radius = VisionRange;

        // Optionally, adjust the center of the collider
        // For a forward-facing cone, set the center to half the vision range in the forward direction
        sphereCollider.center = transform.position;
    }

    void DrawVisionCone()//this method creates the vision cone mesh
    {
        int[] triangles = new int[(VisionConeResolution - 1) * 3];
            Vector3[] Vertices = new Vector3[VisionConeResolution + 1];
            Vertices[0] = Vector3.zero;
            float Currentangle = -VisionAngle / 2;
            float angleIcrement = VisionAngle / (VisionConeResolution - 1);
            float Sine;
            float Cosine;

            for (int i = 0; i < VisionConeResolution; i++)
            {
                Sine = Mathf.Sin(Currentangle);
                Cosine = Mathf.Cos(Currentangle);
                Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
                Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
                if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, VisionRange, obstacleMask))
                {
                    Vertices[i + 1] = VertForward.normalized * hit.distance;
                    if (hit.collider.gameObject.CompareTag("GoodGuy") || hit.collider.gameObject.CompareTag("BadGuy"))
                    {

                        seenGuy = true;
                    }
                    
                    if (hit.collider.gameObject.CompareTag("DeadBody"))
                    {
                        Debug.Log("seen body");
                        seenBody = true;
                    }
                    // Debug.Log(hit.distance);
                }
                else
                {
                    Vertices[i + 1] = VertForward.normalized * VisionRange;
                }


                Currentangle += angleIcrement;
            }
            for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
            {
                triangles[i] = 0;
                triangles[i + 1] = j + 1;
                triangles[i + 2] = j + 2;
            }
            VisionConeMesh.Clear();
            VisionConeMesh.vertices = Vertices;
            VisionConeMesh.triangles = triangles;
            MeshFilter_.mesh = VisionConeMesh;
    }
}
[Serializable]
public class UnityVector3Event : UnityEvent<Vector3>{}
