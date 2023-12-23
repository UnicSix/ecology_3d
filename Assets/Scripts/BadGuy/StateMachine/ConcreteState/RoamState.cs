using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class RoamState : BadGuyState
{
    private float _sampleRange=50f;
    private float _sampleDegree;
    private NavMeshHit _hit;
    public RoamState(BadGuy badguy, BadGuyStateMachine badguyStateMachine) : base(badguy, badguyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        badguy._agent.angularSpeed = 250f;
        badguy._agent.speed = 8f;
        Vector3 randomPoint = badguy.transform.position + Random.insideUnitSphere * _sampleRange;
        while(!NavMesh.SamplePosition(randomPoint, out _hit, _sampleRange, NavMesh.AllAreas))
        {
            randomPoint = badguy.transform.position + Random.insideUnitSphere * _sampleRange;
        }
        badguy._agent.SetDestination(randomPoint);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        if (badguy.seenGuy)
        {
            badguy._agent.ResetPath();
            Debug.Log("Roam to Kill");
            badguy.StateMachine.ChangeState(badguy.killState, badguy.guyPos);
        }
        else if (badguy.seenFootprint)
        {
            badguy._agent.ResetPath();
            Debug.Log("Roam to Track");
            badguy.StateMachine.ChangeState(badguy.trackState, badguy.footprintPos);
        }
        else if(!badguy._agent.hasPath)
        {
            Debug.Log("Roam to Idle");
            badguy.StateMachine.ChangeState(badguy.idleState);
        }
    }
}
