using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ZombieArmy.Character
{
	/// <summary>
	/// 学生AI
	/// </summary>
	public class StudentAi1 : MonoBehaviour
	{
		private NavMeshAgent nav;//移动目标
        public float DetectionRange = 10;//侦测范围
        public float AttackRange = 3;//攻击范围
        //攻击的目标
        [SerializeField] private Transform attackTarget;
        //寻路的目标
        [SerializeField] private Transform   FindTarget;
        //敌人layer
        [SerializeField] public  LayerMask enemyLayer;
		//角色状态信息
		private CharacterStatusInfo characterStatusInfo;
		//开始攻击时间
		private float startAttackTime;
		//攻击范围内的所有敌人目标 最多10个敌人
		public Collider[] withinAttackRangeEnemies = new Collider[10];
        //侦测范围内的所有敌人 最多十个敌人
        public Collider[] withinDetectionRangeEnemies = new Collider[10];
        //攻击次数
        private int AttackNumber=1;
        private Vector3 Tran;
        void Start()
		{
			characterStatusInfo = GetComponent<StudentStatus>().characterStatusInfo;
            nav = GetComponent<NavMeshAgent>();
            Tran = gameObject.transform.position ;
            
        }

        void Update()
        {
            //检测侦测范围内的敌人
            int inDetectionDistanceEnemyCount = Physics.OverlapSphereNonAlloc(transform.position, DetectionRange, withinDetectionRangeEnemies, enemyLayer);
            //检测攻击范围内的敌人
            int overlapEnemyCount = Physics.OverlapSphereNonAlloc(transform.position,AttackRange, withinAttackRangeEnemies, enemyLayer);
            //如果房间触发器被触发则学生才会开始索敌
            //先检测侦察范围内是否有敌人，如果有则拉近敌人距离
            //如果攻击范围内有敌人 则攻击敌人
              if (inDetectionDistanceEnemyCount == 0)
             {
                nav.destination  = Tran;
                return;
            }

            FindTargetEnemy(withinDetectionRangeEnemies, inDetectionDistanceEnemyCount);

            //根据攻击时间间隔计算攻击
            if (startAttackTime < Time.time)
            {
                    AttackTargetEnemy(withinAttackRangeEnemies, overlapEnemyCount);
                    startAttackTime = Time.time + characterStatusInfo.AttackInterval;            
            }
		}
        private void FindTargetEnemy(Collider[] withinDetectionRangeEnemies, int enemyCount = 0)
        {
            FindTarget = SelectDetectionTargetEnemy(withinDetectionRangeEnemies, enemyCount);
            nav.destination = FindTarget .position  ;
        }
        private void AttackTargetEnemy(Collider[] withinAttackRangeEnemies, int enemyCount = 0)
        {
            //选择目标敌人
            attackTarget = SelectDetectionTargetEnemy(withinAttackRangeEnemies, enemyCount);
            if ((attackTarget.position.x - gameObject.transform.position.x) * (attackTarget.position.x - gameObject.transform.position.x) + (attackTarget.position.y - gameObject.transform.position.y) * (attackTarget.position.y - gameObject.transform.position.y) >= AttackRange * AttackRange)
            {
                attackTarget = null;
            }
            else
            {
                nav.destination = attackTarget.position;
                //面向目标敌人
                //motor.GraduallyRotateTowardTarget(attackTarget.transform.position);
                //目标敌人扣血
                if (AttackNumber % 3 != 0)
                {
                    attackTarget.GetComponent<CharacterStatus>().TakeDamage(characterStatusInfo.Atk);
                    AttackNumber++;
                }
                else if (AttackNumber % 3 == 0)
                {
                    attackTarget.GetComponent<CharacterStatus>().TakeDamage(characterStatusInfo.Atk * 1.5f);
                    // Debug.Log(123456);
                    AttackNumber++;
                }
            }
        }

        /*<summary>
        /// 根据仇恨值选择一个目标敌人
        /// </summary>
        /// <param name="withinAttackRangeEnemies">在攻击范围内的所有敌人数组</param>
        /* <returns></returns>
        /*
        private Transform SelectTargetEnemy(Collider[] withinAttackRangeEnemies, int enemyCount = 0)
        {
            //随机一个仇恨值
            float randomHatredValue = Random.Range(0, 1f);
            //计算总仇恨值
            int hatredCount = 0;
            for (int i = 0; i < enemyCount; i++)
            {
                hatredCount += withinAttackRangeEnemies[i].GetComponent<CharacterStatus>().characterStatusInfo.Hatred;
            }

            //计算每一个单位的仇恨值比例 判断随机仇恨值是否在比例区间内
            float hatredRatio = 0;
            for (int i = 0; i < enemyCount; i++)
            {
                //左区间 = 当前仇恨比例， 仇恨比例右区间 = 当前仇恨比例 + 当前敌人仇恨值占比
                float hatredRatioRightRange = hatredRatio + (float)withinAttackRangeEnemies[i].GetComponent<CharacterStatus>().characterStatusInfo.Hatred / hatredCount;

                if (hatredRatio <= randomHatredValue && randomHatredValue < hatredRatioRightRange)
                {
                    return withinAttackRangeEnemies[i].transform;
                }

                //更新仇恨比例左区间为右区间
                hatredRatio = hatredRatioRightRange;
            }
            return withinAttackRangeEnemies[enemyCount - 1].transform;
        }
        */
        private Transform SelectDetectionTargetEnemy(Collider[] withinDetectionRangeEnemies, int enemyCount = 0)
        {
            float MinDistance = 1000000000;
            int j=0;
            for (int i = 0;i < enemyCount;i++ )
            {
                float Distance;
                Distance = (withinDetectionRangeEnemies[i].GetComponent<Transform>().position.x) * (withinDetectionRangeEnemies[i].GetComponent<Transform>().position.x) + (withinDetectionRangeEnemies[i].GetComponent<Transform>().position.z) * (withinDetectionRangeEnemies[i].GetComponent<Transform>().position.z);
                if (MinDistance >Distance )
                {
                    MinDistance = Distance;
                    j = i;
                    return withinDetectionRangeEnemies[j].transform;
                }
               
            }
           return withinDetectionRangeEnemies[j].transform;
        }

    }


}
