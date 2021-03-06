using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform target;
    Animator animator;
    public Collider weaponCollider;
    public float rotateSpeed;       // 回転の適用速度
    public float defaultMoveSpeed;

    public float HP;
    public float MaxHP;
    public float atk;
    public float damageAmount;

    public EnemyUIManager enemyUIManager;
    public GameObject enemyUIManagerGO;

    private float damageShowTime;
    //private float fixedsliderShowTime;

    [SerializeField] RagdollManager ragdollManager;

    public SoundManager soundManager;
    public int attackID;//攻撃音を鳴らすために必要

    void Start()
    {
        target = GameObject.FindGameObjectWithTag ("Player").transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
//        weaponCollider = GetComponentInChildren<CapsuleCollider>();
        DisableWeaponCollider();
        rotateSpeed = 0.1f;

        enemyUIManagerGO.SetActive(true);
        HP = MaxHP;
        float sliderValue = HP / MaxHP;
        enemyUIManager.UpdateHP(sliderValue);
        enemyUIManager.damageText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = target.position;
        if (!float.IsInfinity(agent.remainingDistance))
        {
            animator.SetFloat("Distance", agent.remainingDistance);//ここを(!float.IsInfiniry(agent.remainingDistance))で処理する
        }
        else
        {
            animator.SetFloat("Distance", Vector3.Distance(target.position, transform.position));
        }

        if (damageShowTime > 0)
        {
            damageShowTime -= 1;
        }
        if (damageShowTime == 0)
        {
            enemyUIManager.damageText.text = "";
            damageShowTime -= 1;
        }
    }

    //private void FixedUpdate()
    //{
    //    if (fixedsliderShowTime > 0)
    //    {
    //        fixedsliderShowTime -= 1;
    //        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"+fixedsliderShowTime);
    //    }
    //    if (fixedsliderShowTime == 0)
    //    {
    //        Debug.Log($"{fixedsliderShowTime}でfixedの方は終わり");
    //        fixedsliderShowTime -= 1;
    //    }
    //}


    public void DisableWeaponCollider()//アニメーションのイベントで制御
    {
        weaponCollider.enabled = false;
    }
    public void EnableWeaponCollider()
    {
        weaponCollider.enabled = true;
    }

    public void SetOnSuperArmor()
    {
        animator.SetBool("superArmor", true); 
    }
    public void SetOffSuperArmor()
    {
        animator.SetBool("superArmor", false);
    }

    void OnTriggerEnter(Collider other)
    {
        DamageSource damageSource = other.GetComponent<DamageSource>();
        if (damageSource != null)//もしぶつかった相手がDamageSourceを持っていたら
        {
            damageSource.userplayer.soundManager.PlaySoundEffect(damageSource.userplayer.attackID);
            Debug.Log(damageSource.userplayer.attackID);

            animator.SetTrigger("hitDamage");//

            damageShowTime = 50f;

            damageAmount = damageSource.damageAmount;//データ上のHPを減らす
            HP -= damageAmount;
            float sliderValue = HP / MaxHP;
            enemyUIManager.UpdateHP(sliderValue);
            enemyUIManager.UpdateDamageText(damageSource.damageAmount);

            int damageAmountInt = (int)damageAmount;
            enemyUIManager.damageText.text = damageAmountInt.ToString();

            if (HP <= 0)//ノックアウト処理
            {
//                soundManager.PlaySoundEffect(6);くらいSEはアニメーターからに統一
                animator.GetComponent<NavMeshAgent>().speed = 0;
                animator.SetTrigger("knockOut");
                animator.tag = "Untagged";
                enemyUIManager.rockOnMarker.enabled = false;
                DisableWeaponCollider();
            }
        }
    }

    public void KnockOut()
    {
        animator.enabled=false;
        enemyUIManagerGO.SetActive(false);
//        ragdollManager.SetRagdoll(true);
    }
}