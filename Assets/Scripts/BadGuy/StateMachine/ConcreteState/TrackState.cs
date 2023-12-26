using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrackState : BadGuyState
{
    public TrackState(BadGuy badguy, BadGuyStateMachine badguyStateMachine) : base(badguy, badguyStateMachine)
    {
    }

    public override void EnterState(Vector3 pos)
    {
        base.EnterState();
        badguy.statusBar.Select("track");
        badguy.agent.angularSpeed = 120f;
        badguy.agent.speed = 8f;
        badguy.agent.SetDestination(pos);
        badguy.seenFootprint = false;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (badguy.agent.pathStatus == NavMeshPathStatus.PathComplete && !badguy.seenFootprint && !badguy.seenGuy)
        {
            Debug.Log("Track to Idle");
            badguy.StateMachine.ChangeState(badguy.idleState);
        }
        else if (badguy.seenGuy)
        {
            Debug.Log("Track to Kill");
            badguy.StateMachine.ChangeState(badguy.killState, badguy.guyPos);
        }
        else if (badguy.seenFootprint)
        {
            Debug.Log("track to track");
            badguy.StateMachine.ChangeState(badguy.trackState, badguy.footprintPos);
        }

    }
}