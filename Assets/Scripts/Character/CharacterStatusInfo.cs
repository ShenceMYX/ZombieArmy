using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieArmy.Character
{
	/// <summary>
	/// 角色状态属性信息
	/// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/CharacterStatusInfo")]
	public class CharacterStatusInfo : ScriptableObject
	{
        #region Readonly Version
        ////攻击力
        //[SerializeField] private float atk;
        //public float Atk => atk;
        ////生命值
        //[SerializeField] private float health;
        //public float Health => health;
        ////攻击间隔
        //[SerializeField] private float attackInterval;
        //public float AttackInterval => attackInterval;
        ////攻击距离
        //[SerializeField] private float attackRange;
        //public float AttackRange => attackRange;
        ////仇恨值
        //[SerializeField] private int hatred;
        //public int Hatred => hatred;
        #endregion

        //攻击力
        public float Atk;
		//生命值
		[SerializeField] private float maxHealth;
		public float MaxHealth => maxHealth;
		//攻击间隔
		public float AttackInterval;
		//攻击距离
		[SerializeField] private float attackRange;
		public float AttackRange => attackRange;
		//仇恨值
		[SerializeField] private float hatred;
		public float Hatred => hatred;

	}
}