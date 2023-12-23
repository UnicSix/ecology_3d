using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackState : BadGuyState
{
    public TrackState(BadGuy badguy, BadGuyStateMachine badguyStateMachine) : base(badguy, badguyStateMachine)
    {
    }

    public override void EnterState(Vector3 pos)
    {
        base.EnterState();
        badguy._agent.angularSpeed = 120f;
        badguy._agent.speed = 5f;
        badguy._agent.SetDestination(pos);
        Debug.Log("Track"+pos);
        badguy.seenFootprint = false;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (badguy._agent.isStopped)
        {
            Debug.Log("Track to Idle");
            badguy.StateMachine.ChangeState(badguy.idleState);
        }
        else if (badguy.seenFootprint)
        {
            badguy.StateMachine.ChangeState(badguy.trackState, badguy.footprintPos);
        }
    }
}