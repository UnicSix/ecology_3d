using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

    public class VillagerFOV : MonoBehaviour
    {
        public float radius;
        [Range(0,360)]
        public float angle;
        public GameObject villagerRef;
        public bool isInSight;
        public LayerMask targetMask;
        public LayerMask obstructionMask;

        public void Start()
        {
            villagerRef = GameObject.Find("Villager");
            StartCoroutine(FOVRoutine());
        }

        private IEnumerator FOVRoutine()
        {
            float delay = 0.2f;
            WaitForSeconds wait = new WaitForSeconds(delay);
            while (true)
            {
                yield return wait;
                FieldOfViewCheck();
            }
        }

        private void FieldOfViewCheck()
        {
            Collider[] rangeCheck = Physics.OverlapSphere(transform.position, radius, targetMask);
            if (rangeCheck.Length != 0)
            {
                Transform target = rangeCheck[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.position, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);
                    if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    {
                        isInSight = true;
                    }
                    else
                        isInSight = true;
                }
                else
                    isInSight = false;
            }
            else if (isInSight)
                isInSight = false;
        }
    }