using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class WreckState : BadGuyState
{
    List<Vector3> taskPositions;
    private int targetTaskIndex=-1;
    private int prevTaskIndex=-1;
    private ProgressStatusHandler handler;
    private float efficiency = -10f;
    private float energyCost;
    private float minCost = 1f;
    private float maxCost = 10f;
    private Vector3 visionDir;
    private Vector3 curTaskPos;
    public WreckState(BadGuy badguy, BadGuyStateMachine badguyStateMachine) : base(badguy, badguyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        badguy.statusBar.Select("wreck");
        badguy.agent.angularSpeed = 300f;
        badguy.agent.speed = 10f;
        energyCost = Random.Range(minCost, maxCost);
        
        taskPositions = badguy.taskPositions;
        while (targetTaskIndex == prevTaskIndex)
        {
            targetTaskIndex = Random.Range(0, taskPositions.Count);
            Debug.Log(targetTaskIndex);
        }
        prevTaskIndex = targetTaskIndex;
        
        NavMeshHit navHit;
        curTaskPos = taskPositions[targetTaskIndex];
        if (NavMesh.SamplePosition(curTaskPos, out navHit, 30.0f, NavMesh.AllAreas))
        {
            curTaskPos = navHit.position;
            badguy.agent.SetDestination(navHit.position);
        }
        visionDir = badguy.taskPoints[targetTaskIndex].transform.Find("Facing").position;
        if (NavMesh.SamplePosition(visionDir, out navHit, 30.0f, NavMesh.AllAreas))
        {
            visionDir = navHit.position;
        }
        
        handler = badguy.taskPoints[targetTaskIndex].GetComponentInChildren<ProgressStatusHandler>();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (Vector3.Distance(badguy.transform.position, curTaskPos)<0.2f)
        {
            Wrecking();
        }

        if (energyCost <= 0f)
        {
            Debug.Log("roam");
            badguy.StateMachine.ChangeState(badguy.roamState);
        }
    }

    void Wrecking()
    {
        // badguy.agent.ResetPath();
        Debug.Log("wre");
        badguy.transform.LookAt(visionDir);
        if (handler.DecreaseWorkProgress(efficiency / 10.0f)) {
            badguy.StateMachine.ChangeState(badguy.idleState);
        }
        else 
            handler.Occupying();

        energyCost -= Time.deltaTime;
    }
}
