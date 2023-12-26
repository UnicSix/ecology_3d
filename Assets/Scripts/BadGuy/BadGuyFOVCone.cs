using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BadGuyFov : MonoBehaviour
{
    private BadGuy badguy;
    private QueryTriggerInteraction _triggerInteraction;
    private float _priorFreshment = 0.9f;
    public Material VisionConeMaterial;
    [SerializeField] private float VisionRange;
    [SerializeField] private float VisionAngle;
    public LayerMask obstacleMask;//layer with objects that obstruct the enemy view, like walls, for example
    public LayerMask targetMask;//layer with objects that obstruct the enemy view, like walls, for example
    [SerializeField] private int VisionConeResolution;//the vision cone will be made up of triangles, the higher this value is the pretier the vision cone will be
    Mesh VisionConeMesh;
    MeshFilter MeshFilter_;
    //Create all of these variables, most of them are self explanatory, but for the ones that aren't i've added a comment to clue you in on what they do
    //for the ones that you dont understand dont worry, just follow along
    
    // public UnityVector3Event onFootprintLockedOn;
    // public UnityVector3Event onPplInsight;
    void Start()
    {
        transform.AddComponent<MeshRenderer>().material = VisionConeMaterial;
        MeshFilter_ = transform.AddComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
        VisionAngle = 90f;
        VisionAngle *= Mathf.Deg2Rad;
        VisionRange = 25f;
        VisionConeResolution = 20;
        _triggerInteraction = QueryTriggerInteraction.Collide;
        badguy = GetComponentInParent<BadGuy>();
        if (badguy == null)
        {
            Debug.Log("badGuy: " + badguy.tag);
        }
        if (gameObject.CompareTag("BadGuy"))
        {
            VisionAngle = 30f;
            VisionAngle *= Mathf.Deg2Rad;
            VisionRange = 45;
        }
    }

    //draw the fov cone, return the position of the most fresh spotted footprint
    void Update()
    {
        DrawVisionCone();//calling the vision cone function everyframe just so the cone is updated every frame
    }

    void DrawVisionCone()//this method creates the vision cone mesh
    {
        int[] triangles = new int[(VisionConeResolution - 1) * 3];
        Vector3[] Vertices = new Vector3[VisionConeResolution + 1];
        Vertices[0] = Vector3.zero;
        float Currentangle = -VisionAngle / 2;
        float angleIncrement = VisionAngle / (VisionConeResolution - 1);
        float Sine;
        float Cosine;
        float maxDist=0;
        float maxFreshment = 0;
        float lockThreshold = 0.5f;
        RaycastHit[] footprints;

            for (int i = 0; i < VisionConeResolution; i++)
            {
                Sine = Mathf.Sin(Currentangle);
                Cosine = Mathf.Cos(Currentangle);
                Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
                Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
                if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, VisionRange, obstacleMask, _triggerInteraction))
                {
                    Vertices[i + 1] = VertForward.normalized * hit.distance;
                    if (hit.collider.gameObject.CompareTag("GoodGuy"))
                    {
                        badguy.SetGuyPos(hit.collider.transform.position);
                        badguy.SetSeenGuy(true);
                        // Debug.Log("see guy");
                    }
                }
                else
                {
                    Vertices[i + 1] = VertForward.normalized * VisionRange;
                }
                Currentangle += angleIncrement;
                
                //collect footprint using RaycastAll
                footprints = Physics.RaycastAll(transform.position, RaycastDirection, VisionRange, targetMask,
                    _triggerInteraction);
                foreach (RaycastHit footprint in footprints)
                {
                    if (footprint.collider.gameObject.CompareTag("GoodPrint"))
                    {
                        float freshment = footprint.collider.transform.localScale.x / 0.5f;
                        maxFreshment = freshment > maxFreshment ? freshment : maxFreshment;
                        if (maxFreshment >= lockThreshold)
                        {
                            // onFootprintLockedOn.Invoke(footprint.transform.TransformPoint(footprint.transform.position));
                            badguy.SetFootprintPos(footprint.transform.position);
                            //Debug.Log(footprint.transform.position);
                            badguy.SetSeenFootprint(true);
                            // Debug.Log("hit print:"+footprint.transform.TransformPoint(footprint.transform.position));
                        }
                    }
                }

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
// [Serializable]
// public class UnityVector3Event : UnityEvent<Vector3>{}
