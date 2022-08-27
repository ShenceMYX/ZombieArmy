using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieArmy.Unit;
using Random = UnityEngine.Random;
using UnityEngine.AI;

namespace ZombieArmy.Character
{

    public class spine : MonoBehaviour { 
    public float MarshDamge;
        private TrapManager TrapManager;
        private int frame=1;
        private int frameNow = 90;
        private void Start()
        {
            TrapManager = FindObjectOfType<TrapManager>();
        }
        private void Update()
        {
            frame++;
        }

        private void OnTriggerStay(Collider other)
        {
         if (frame > frameNow)
            {
                other.GetComponent<CharacterStatus>()?.TakeDamage(MarshDamge);
                frameNow += 90;
            }
               
               
            
        }

    }
}
