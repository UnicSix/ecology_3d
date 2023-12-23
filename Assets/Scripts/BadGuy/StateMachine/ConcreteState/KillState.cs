using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillState : BadGuyState
{
    public KillState(BadGuy badguy, BadGuyStateMachine badguyStateMachine) : base(badguy, badguyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        GameObject weapon = Resources.Load<GameObject>("PreFab/Weapons/Hammer");
        weapon.transform.localPosition = Vector3.forward * 1.3f + Vector3.up * 2.8f + Vector3.left * 0f;
        weapon.transform.localRotation = Quaternion.Euler(90f, 0f, 90f );
        Object.Instantiate(weapon, badguy.transform);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (badguy.seenGuy)
        {
            Debug.Log("Kill to Kill");
            badguy.StateMachine.ChangeState(badguy.killState, badguy.guyPos);
        }
        else if (badguy.seenFootprint)
        {
            Debug.Log("Kill to Track");
            badguy.StateMachine.ChangeState(badguy.trackState, badguy.guyPos);
        }
    }
}
