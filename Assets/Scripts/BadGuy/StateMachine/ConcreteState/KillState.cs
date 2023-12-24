using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class KillState : BadGuyState
{
    private Vector3 guyPos;
    private float cdCounter;
    private float killCdTime;
    public KillState(BadGuy badguy, BadGuyStateMachine badguyStateMachine) : base(badguy, badguyStateMachine)
    {
        killCdTime = 5f;
        cdCounter = 0f;
    }

    public override void EnterState(Vector3 pos)
    {
        base.EnterState();
        guyPos = pos;
        badguy.agent.angularSpeed = 300f;
        badguy.agent.speed = 30f;
        badguy.agent.acceleration = 12f;
        badguy.agent.SetDestination(badguy.guyPos);
        badguy.seenGuy = false;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        // if (badguy.seenGuy)
        // {
        //     Debug.Log("Kill to Kill");
        //     badguy.StateMachine.ChangeState(badguy.killState, badguy.guyPos);
        // }
        // else if (badguy.seenFootprint)
        // {
        //     Debug.Log("Kill to Track");
        //     badguy.StateMachine.ChangeState(badguy.trackState, badguy.guyPos);
        // }
        if (cdCounter > 0)
        {
            cdCounter-= Time.deltaTime;
        }
        if (Vector3.Distance(badguy.transform.position, guyPos)<=3f && badguy.seenGuy && cdCounter<=0)
        {
            Kill();
            badguy.StateMachine.ChangeState(badguy.idleState);
            cdCounter = killCdTime;
        }

        if (badguy.agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            badguy.StateMachine.ChangeState(badguy.idleState);
        }
    }

    private void Kill()
    {
        Debug.Log("kill");
        GameObject weapon = Resources.Load<GameObject>("PreFab/Weapons/Hammer");
        weapon.transform.localPosition = Vector3.forward * 1.3f + Vector3.up * 2.8f + Vector3.left * 0f;
        weapon.transform.localRotation = Quaternion.Euler(90f, 0f, 90f );
        Object.Instantiate(weapon, badguy.transform);
    }
}
