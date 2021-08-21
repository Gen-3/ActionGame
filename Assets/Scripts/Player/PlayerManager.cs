using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    float x;
    float z;
    public Collider weaponCollider;

    Rigidbody rb;
    Animator animator;

    public bool isSlow;
    public bool canAttack;
    public bool isKnockBuck;

    //A~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    float inputHorizontal;
    float inputVertical;

    float moveSpeed = 3f;
    const float defaultMoveSpeed = 3.0f;
    //A~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public float applySpeed;       // 回転の適用速度


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        DisableWeaponCollider();
        isSlow = false;
        canAttack = true;
        moveSpeed = defaultMoveSpeed;
    }

    void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (canAttack)
            {
                animator.SetTrigger("attack");
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            Damage();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            KnockOut();
        }


        //A~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        //A~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    }

    public void Damage()
    {
        animator.SetTrigger("hitDamage");
    }

    public void KnockOut()
    {
        animator.SetTrigger("knockOut");
    }

    private void FixedUpdate()
    {
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                Vector3 direction = transform.position + new Vector3(x, 0, z) * speedCoefficient;
                transform.LookAt(direction);

                rb.velocity = new Vector3(x, 0, z) * speedCoefficient;
                animator.SetFloat("MoveSpeed", rb.velocity.magnitude);
        ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        */

        if (!isSlow)
        {
            if (x != 0 || z != 0)//入力があるときはisMovingをtrueにして動かす
            {
                moveSpeed = defaultMoveSpeed;
                animator.SetBool("isMoving", true);

                InputDirectionConvertor();
            }
            else//入力がないときはisMovingをfalseにする
            {
                animator.SetBool("isMoving", false);
            }
        }
        else//isSlow時
        {
            animator.SetBool("isMoving", false);
            moveSpeed = defaultMoveSpeed / 6;

            InputDirectionConvertor();
        }
    }

    void InputDirectionConvertor()
    {
        //A~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        // 方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 moveForward = cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal;

        // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
        rb.velocity = moveForward * moveSpeed + new Vector3(0, rb.velocity.y, 0);

        // キャラクターの向きを進行方向に
        if (moveForward != Vector3.zero)
        {
            if (!isKnockBuck)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(moveForward),
                                                      applySpeed); ;
            }
        }
        //A~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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


    void OnTriggerEnter(Collider other)//何かTriggerにぶつかった(enter)ときに自動的に呼ばれる
    {
        DamageSource damageSource = other.GetComponent<DamageSource>();
        if (damageSource != null)//もしぶつかった相手がDamageSourceを持っていたら
        {
            animator.SetTrigger("hitDamage");
        }
    }
}