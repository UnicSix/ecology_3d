using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BadGuyStatus : MonoBehaviour
{
    public const int ROAM = 0;
    public const int TRACK = 1;
    public const int KILL = 2;
    public const int WRECK = 3;
    public const int IDLE = 4;
    /*
     * action intents: store intention value of actions
     * 0: roam
     * 1: track
     * 2: kill
     * 3: wreck
     * 4: idle
     * particular intention value / sum of the intention value == possibility to perform certain action
     */
    public Vector3 dest;
    public float[] actionIntent;
    public string[] actions;
    public string curAction;
    void Awake()
    {
        actionIntent = new float[5]{10, 10,10,10,10};
        actions = new string[5]{"roam", "track", "kill", "wreck", "idle"};
        curAction = "idle";
    }

}
