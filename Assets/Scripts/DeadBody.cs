using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBody : MonoBehaviour
{

    private float bodyDestroyTime=60f;
    private float destroyCounter=0f;

    private void Update(){
        destroyCounter+=Time.deltaTime;
        if(destroyCounter>=bodyDestroyTime){
            Destroy(this.gameObject);
        }
    }

}
