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
    public bool canCombo;
    public bool knockOut;

    public GameObject WeaponObject;//使ってない
    public SoundManager soundManager;

    //A~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    float inputHorizontal;
    float inputVertical;

    float moveSpeed = 3f;
    [SerializeField] float defaultMoveSpeed = 3.0f;
    //A~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public DamageSource damageSource;
    public float applySpeed;       // 回転の適用速度

    public int attackID;

    public float HP;
    public float MaxHP;
    public float atk;
    public float damageAmount;
    public UIManager uiManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        //        weaponCollider = GetComponentInChildren<CapsuleCollider>();
        DisableWeaponCollider();
        isSlow = false;
        canAttack = true;
        knockOut = false;
        moveSpeed = defaultMoveSpeed;

        HP = MaxHP;
        float sliderValue = HP / MaxHP;
        uiManager.UpdateHP(sliderValue);

    }

    void Update()
    {
        if (knockOut)
        {
            return;
        }
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.J))//弱攻撃ボタン押下時
        {
            if (canAttack)
            {
                animator.SetTrigger("attack1");
            }
        }

        if (Input.GetKey(KeyCode.U))//強攻撃ボタン押下時
        {
            if (canAttack)
            {
                animator.SetTrigger("attack2");
            }
            if (!canAttack && canCombo)
            {
                animator.SetTrigger("attack3");
            }
        }

        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    Damage();
        //}
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    KnockOut();
        //}


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
        if (knockOut) { return; }
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
                                                      applySpeed);
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
            damageSource.userEnemy.soundManager.PlaySoundEffect(damageSource.userEnemy.attackID);

            animator.SetTrigger("hitDamage");

            damageAmount = damageSource.damageAmount;//データ上のHPを減らす
            HP -= damageAmount;
            float sliderValue = HP / MaxHP;
            Debug.Log($"プレイヤーのHPは{HP}");
            uiManager.UpdateHP(sliderValue);

            if (HP <= 0)//ノックアウト処理
            {
                Debug.Log("プレイヤーのHPが０以下になりました/ノックアウト処理");
                soundManager.PlaySoundEffect(8);
                knockOut = true;
                animator.SetTrigger("knockOut");
                GetComponent<CapsuleCollider>().enabled = false;
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach(GameObject enemy in enemies)
                {
                    enemy.GetComponent<Animator>().SetBool("standby", true);
                }
            }
        }
    }


}