using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BadGuyState
{
    private float _idleTime = 0f;
    private float _idleLimit = UnityEngine.Random.Range(0.5f, 3f);
    public IdleState(BadGuy badguy, BadGuyStateMachine badguyStateMachine) : base(badguy, badguyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        float rotationSpeed = 2f;
        float maxAngle=90f;
        float angle = Mathf.Cos(rotationSpeed * Time.time) * maxAngle;
        int nextState;
        _idleTime += Time.deltaTime;
        badguy.transform.Rotate(Vector3.up, angle * Time.deltaTime);
        if (_idleTime >= _idleLimit)
        {
            _idleTime = 0;
            // Debug.Log("Idle to Roam");
            badguy.StateMachine.ChangeState(badguy.roamState);
        }

        if (badguy.seenGuy)
        {
            Debug.Log("Idle to Kill");
            badguy.StateMachine.ChangeState(badguy.killState, badguy.guyPos);
        }
        else if (badguy.seenFootprint)
        {
            // Debug.Log("Idle to Track");
            badguy.StateMachine.ChangeState(badguy.trackState, badguy.footprintPos);
        }
    }

    private int NextState()
    {
        float rd = UnityEngine.Random.value;
        float cumulativeProbability=0;
        float[] intentToPossibility = new float[5];
        float sum=0;
        foreach (int i in badguy.actionIntent)
            sum += i;
        float accuIntent = 0;
        for(int i=0; i<5; i++)
        {
            accuIntent += badguy.actionIntent[i];
            intentToPossibility[i] = accuIntent / sum;
        }

        int nextAction;
        for(nextAction=0; nextAction<5; nextAction++)
        {
            cumulativeProbability += intentToPossibility[nextAction];
            if (rd <= cumulativeProbability)
            {
                return nextAction;
            }
        }
        return 0;
    }
}
