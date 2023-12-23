using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BadGuyState
{
    protected BadGuy badguy;
    protected BadGuyStateMachine badguyStateMachine;

    public BadGuyState(BadGuy badguy, BadGuyStateMachine badguyStateMachine)
    {
        this.badguy = badguy;
        this.badguyStateMachine = badguyStateMachine;
    }

    public virtual void EnterState(){}
    public virtual void EnterState(Vector3 pos){}
    public virtual void ExitState(){}
    public virtual void FrameUpdate(){}
    public virtual void PhysicsUpdate(){}
    

}