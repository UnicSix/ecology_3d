using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public interface IMoveable
{
    NavMeshAgent agent { get; set; }
    
    // void MoveToPos(Vector3 pos);

}