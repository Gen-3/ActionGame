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
    public float applySpeed;       // 回転の適用速度


    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        DisableWeaponCollider();
        applySpeed = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = target.position;
        animator.SetFloat("Distance",agent.remainingDistance);

    }


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
            animator.SetTrigger("hitDamage");
        }
    }
}