using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using ZombieArmy.UI;

namespace ZombieArmy.Character
{
    /// <summary>
    /// 角色状态类：血量、攻击力、战斗数值计算
    /// </summary>
    public class StudentStatus : CharacterStatus
    {
        protected override void OnDeath()
        {
            base.OnDeath();
            Unit.UnitSpawner.Instance.AddUnitToUnitGroup(transform);
        }

        //显示侦测范围
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, GetComponent<StudentAI>().findRange);
        }
    }
}
        