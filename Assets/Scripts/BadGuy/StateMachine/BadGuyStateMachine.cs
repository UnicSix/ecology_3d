using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BadGuyStateMachine
{
    public BadGuyState CurBadGuyState{ get; set; }

    public void Initialize(BadGuyState startingState)
    {
        CurBadGuyState = startingState;
        CurBadGuyState.EnterState();
    }

    public void ChangeState(BadGuyState newState)
    {
        CurBadGuyState.ExitState();
        CurBadGuyState = newState;
        CurBadGuyState.EnterState();
    }

    public void ChangeState(BadGuyState newState, Vector3 newPos)
    {
        CurBadGuyState.ExitState();
        CurBadGuyState = newState;
        CurBadGuyState.EnterState(newPos);
    }
    
}
