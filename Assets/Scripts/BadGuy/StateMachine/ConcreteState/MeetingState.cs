using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeetingState : BadGuyState
{
    public MeetingState(BadGuy badguy, BadGuyStateMachine badguyStateMachine) : base(badguy, badguyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        badguy.agent.ResetPath();
        Debug.Log("meeting");
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (badguy.nowStatus == -1)
        {
            Debug.Log("exit meeting");
            badguy.StateMachine.ChangeState(badguy.idleState);
        }
    }
    
}