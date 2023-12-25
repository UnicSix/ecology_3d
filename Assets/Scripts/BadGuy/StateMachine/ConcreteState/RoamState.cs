using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class RoamState : BadGuyState
{
    private float _sampleRange=50f;
    private float _sampleDegree;
    private NavMeshHit _hit;
    
    private float energyScaler = 0.1f;
    private float killEnergy;
    private float trackEnergy;
    private float wreckEnergy;
    private float killThreshold;
    private float trackThreshold;
    private float wreckThreshold;
    public RoamState(BadGuy badguy, BadGuyStateMachine badguyStateMachine) : base(badguy, badguyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        badguy.agent.angularSpeed = 250f;
        badguy.agent.speed = 8f;
        Vector3 randomPoint = badguy.transform.position + Random.insideUnitSphere * _sampleRange;
        while(!NavMesh.SamplePosition(randomPoint, out _hit, _sampleRange, NavMesh.AllAreas))
        {
            randomPoint = badguy.transform.position + Random.insideUnitSphere * _sampleRange;
        }
        badguy.agent.SetDestination(randomPoint);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        if (badguy.seenFootprint && isEnteringState("trackState"))
        {
            badguy.setTrackEnergy(-trackThreshold);
            badguy.StateMachine.ChangeState(badguy.trackState, badguy.footprintPos);
        }
        else if (badguy.seenGuy && isEnteringState("killState"))
        {
            Debug.Log("Idle to Kill");
            badguy.setKillEnergy(-killThreshold);
            badguy.StateMachine.ChangeState(badguy.killState, badguy.guyPos);
        }
        else if(!badguy.agent.hasPath)
        {
            // Debug.Log("Roam to Idle");
            badguy.StateMachine.ChangeState(badguy.idleState);
        }
    }
    private bool isEnteringState(string nextState)
    {
        float rd;
        float energySum = killEnergy + trackEnergy + wreckEnergy;
        rd = Random.Range(0f, energySum);
        switch (nextState)
        {
            case "killState":
                if (rd <= killEnergy && killEnergy<=15f) return true;
                break;
            case "trackState":
                if (rd > killEnergy && rd <= trackEnergy+killEnergy && trackEnergy >= 3f) return true;
                break;
            case "wreckState":
                if (rd > trackEnergy+killEnergy && rd <= energySum && wreckEnergy >= 10f) return true;
                break;
        }

        return false;
    }
}
