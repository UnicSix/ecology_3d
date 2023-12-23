using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVisionTrigger
{
    bool seenFootprint{ get; set;}
    Vector3 footprintPos{ get; set; }
    bool seenGuy{ get; set;}
    Vector3 guyPos{ get; set; }

    void SetSeenFootprint(bool seenFootprint);
    void SetSeenGuy(bool seenGuy);
    
    void SetFootprintPos(Vector3 footprintPos);
    void SetGuyPos(Vector3 SetGuyPos);
}