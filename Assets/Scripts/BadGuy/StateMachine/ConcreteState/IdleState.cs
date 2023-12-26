using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : BadGuyState
{
    private float _idleTime = 0f;
    private float _idleLimit = UnityEngine.Random.Range(0.5f, 3f);
    private float _sampleRange=10f;
    private float _sampleDegree;
    private NavMeshHit _hit;

    private float energyScaler = 0.1f;
    private float killEnergy;
    private float trackEnergy;
    private float wreckEnergy;
    private float killThreshold=15;
    private float trackThreshold=3;
    private float wreckThreshold=20;

    private float stateChangeCD = 5f;
    private float stateChangeTimer = 0;
    public IdleState(BadGuy badguy, BadGuyStateMachine badguyStateMachine) : base(badguy, badguyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        killEnergy = badguy.getKillEnergy();
        trackEnergy = badguy.getTrackEnergy();
        wreckEnergy = badguy.getWreckEnergy();
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
        base.FrameUpdate();
        float rotationSpeed = 2f;
        float maxAngle=90f;
        float angle = Mathf.Cos(rotationSpeed * Time.time) * maxAngle;
        int nextState;
        stateChangeTimer += Time.deltaTime;
        _idleTime += Time.deltaTime;
        badguy.transform.Rotate(Vector3.up, angle * Time.deltaTime);
        if (_idleTime >= _idleLimit)
        {
            _idleTime = 0;
            // Debug.Log("Idle to Roam");
            badguy.StateMachine.ChangeState(badguy.roamState);
        }
        
        badguy.setKillEnergy(Time.deltaTime*0.3f);
        badguy.setTrackEnergy(Time.deltaTime*0.5f);
        badguy.setWreckEnergy(Time.deltaTime*0.3f);

        if (stateChangeTimer >= stateChangeCD)
        {
            if (badguy.seenFootprint && isEnteringState("trackState"))
            {
                badguy.setTrackEnergy(-trackThreshold);
                badguy.StateMachine.ChangeState(badguy.trackState, badguy.footprintPos);
            }
            else if (badguy.seenGuy && isEnteringState("killState"))
            {
                badguy.setKillEnergy(-killThreshold);
                badguy.StateMachine.ChangeState(badguy.killState, badguy.guyPos);
            }

            if (GameObject.Find("MasterControl").GetComponent<Control>().getSpaceShipDurability() < 0.5f && isEnteringState("wreckState"))
            {
                badguy.setWreckEnergy(-wreckThreshold);
                badguy.StateMachine.ChangeState(badguy.wreckState);
            }
            stateChangeTimer=0;
        }
    }

    private bool isEnteringState(string nextState)
    {
        float rd;
        float energySum = killEnergy + trackEnergy + wreckEnergy;
        rd = Random.Range(0f, energySum);
        Debug.Log("rd: "+rd);
        Debug.Log("kill: "+killEnergy);
        Debug.Log("track: "+trackEnergy);
        Debug.Log("wreck: "+wreckEnergy);
        switch (nextState)
        {
            case "killState":
                if (rd <= killEnergy && killEnergy<=15f) return true;
                break;
            case "trackState":
                if (rd > killEnergy && rd <= trackEnergy+killEnergy && trackEnergy >= 3f) return true;
                break;
            case "wreckState":
                if (rd > trackEnergy+killEnergy && rd <= energySum && wreckEnergy >= 20f) return true;
                break;
        }

        return false;
    }

}
