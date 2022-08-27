using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieArmy.Unit;
using Random = UnityEngine.Random;
using UnityEngine.AI;

namespace ZombieArmy.Character
{
    /// <summary>
	/// 学生AI
	/// </summary>
    public class StudentAI : MonoBehaviour
    {
        //敌人layer
        [SerializeField] private LayerMask enemyLayer;
        //学生种类和信息
        public CharacterStatusInfo characterStatusInfoInstance { get; set; }
        //侦测距离
        public float findRange;
        //攻击的目标
        [SerializeField] private Transform attackTarget;
        //锁定的目标
        [SerializeField] private Transform findTarget;
        //攻击和移动和待机状态判定
        private bool isMove = false;
        //private bool isAttack = false;
        //private bool isWait = true;
        //攻击和侦测范围内的所有敌人目标 最多10个敌人
        public Collider[] withinAttackRangeEnemies = new Collider[10];
        public Collider[] withinFindRangeEnemies = new Collider[10];
        //网格导航
        private NavMeshAgent nav;
        //攻击次数
        private int AttackNumber = 1;
        //动画
        private Animator animator;
        //侦测和攻击范围内敌人的数目
        private int withinAttackRangeEnemiesNumber;
         private int withinFindRangeEnemiesNumber;
    
        //学生的初始位置
        private Vector3 studentOriginPosition;
        // 学生的实时位置
        private Vector3 studentNowPostion;
        //学生的上一秒位置
        private Vector3 studentSecondPsiton;
        //检测时间
        private float checkTime=0;
        //开始攻击时间
        private float startAttackTime;
        private void Start()
        {
            characterStatusInfoInstance = Instantiate(GetComponent<CharacterStatus>().characterStatusInfo);
            nav = GetComponent<NavMeshAgent>();
            studentOriginPosition = transform.position;
            animator = GetComponent<Animator>();
        }
        private void Update()
        {
            FindEnemy();
            CheckMove();
            Timer();
        }
        private void FindEnemy()//检测敌方数目并且确定当前学生的状态
        {
            withinFindRangeEnemiesNumber = Physics.OverlapSphereNonAlloc(transform.position, findRange, withinFindRangeEnemies, enemyLayer);
            withinAttackRangeEnemiesNumber= Physics.OverlapSphereNonAlloc(transform.position, characterStatusInfoInstance.AttackRange, withinAttackRangeEnemies, enemyLayer);
            studentNowPostion = transform.position;
            if (withinFindRangeEnemiesNumber == 0)
            {
                
                 //如果范围内没有敌人，学生为待机状态，同时返回初始位置
                 
                //isWait = true;
                nav.destination = studentOriginPosition;
                findTarget = null;
            }
            else if (withinFindRangeEnemiesNumber > 0 && withinAttackRangeEnemiesNumber == 0)
                
            {
                //如果侦测范围有敌人，攻击范围无敌人，学生追踪侦测范围内的敌人
               // isWait = false;
                attackTarget = null;
                findTarget = SelectTargetEnemy(withinFindRangeEnemies, withinFindRangeEnemiesNumber);
                nav.destination = findTarget.position;
            }
            else if (withinFindRangeEnemiesNumber > 0 && withinFindRangeEnemiesNumber > 0)
            {
                //如果侦测范围和攻击范围都有敌人，则清空侦测范围目标
                findTarget = null;
                nav.destination = transform.position;
                attackTarget = SelectTargetEnemy(withinAttackRangeEnemies, withinAttackRangeEnemiesNumber);
                if(isMove)
                {
                    nav.destination = transform.position;
                }
                if(!isMove&& startAttackTime < Time.time)
                {
                   AttackTargetEnemy(withinAttackRangeEnemies, withinAttackRangeEnemiesNumber);
                    startAttackTime = Time.time + characterStatusInfoInstance.AttackInterval;
                }
            }

        }
        private void CheckMove()//检测学生是否移动
        {
            if (transform.position.x - studentSecondPsiton.x != 0 || transform.position.z - studentSecondPsiton.z != 0)
            {
                isMove = true;
            }
            else
            {
                isMove = false;
            }
        }
        private void Timer()//计时器，并且记录学生上一秒的位置
        {
            if(Time.time-checkTime>=1)
                {
                checkTime=Time.time;
                studentSecondPsiton = transform.position;
            }
        }
        private void AttackTargetEnemy(Collider[] withinAttackRangeEnemies, int enemyCount = 0)
        {
            {
                nav.destination = attackTarget.position;

                //目标敌人扣血
                if (AttackNumber % 3 != 0)//是否攻击三次
                {
                    attackTarget.GetComponent<CharacterStatus>().TakeDamage(characterStatusInfoInstance.Atk);
                    AttackNumber++;
                }
                else if (AttackNumber % 3 == 0)//如果攻击两次了下一次则为强化攻击
                {
                    attackTarget.GetComponent<CharacterStatus>().TakeDamage(characterStatusInfoInstance.Atk* 1.5f);
                    AttackNumber++;
                }
            }
        }
        private Transform SelectTargetEnemy(Collider[] withinAttackRangeEnemies, int enemyCount = 0)
        {
            //随机一个仇恨值
            float randomHatredValue = Random.Range(0, 1f);
            //计算总仇恨值
            float hatredCount = 0;
            for (int i = 0; i < enemyCount; i++)
            {
                hatredCount += withinAttackRangeEnemies[i].GetComponent<BaseStatus>().characterStatusInfo.Hatred;
            }

            //计算每一个单位的仇恨值比例 判断随机仇恨值是否在比例区间内
            float hatredRatio = 0;
            for (int i = 0; i < enemyCount; i++)
            {
                //左区间 = 当前仇恨比例， 仇恨比例右区间 = 当前仇恨比例 + 当前敌人仇恨值占比
                float hatredRatioRightRange = hatredRatio + (float)withinAttackRangeEnemies[i].GetComponent<BaseStatus>().characterStatusInfo.Hatred / hatredCount;

                if (hatredRatio <= randomHatredValue &&
                    randomHatredValue < hatredRatioRightRange)
                {
                    return withinAttackRangeEnemies[i].transform;
                }

                //更新仇恨比例左区间为右区间
                hatredRatio = hatredRatioRightRange;
            }
            return withinAttackRangeEnemies[enemyCount - 1].transform;
        }
    }
}