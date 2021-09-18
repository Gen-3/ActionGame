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
    public bool isRolling;
    int rollingCount;

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

    [SerializeField] CameraManager cameraManager=default;

    public Vector3 rollingForward;
    Vector3 moveForward;

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
        if (isRolling)//ローリング継続時
        {
            rollingCount -= 1;
            Debug.Log(rb.velocity.magnitude);
            if (rollingCount < 0)
            {
                isRolling = false;
            }
            return;
        }

        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.I))//弱攻撃ボタン押下時
        {
            if (canAttack)
            {
                animator.SetTrigger("attack1");
            }
        }

        if (Input.GetKey(KeyCode.O))//強攻撃ボタン押下時
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

        //A~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        //A~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (x != 0 || z != 0)//入力があるとき、入力方向にローリング
            {
                isRolling = true;
                animator.SetTrigger("isRolling");
                rollingForward = transform.forward;
                rollingCount = 8;
            }
            else//入力がないとき、パリイ？
            {
                Debug.Log("移動入力ニュートラル時にローリングボタン押下でパリイを実装予定");
            }

        }
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
        if (knockOut) { return; }//ノックアウト時は入力を無視
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

    void InputDirectionConvertor()//
    {
        //A~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        if (!isRolling)
        {
            // 方向キーの入力値とカメラの向きから、移動方向を決定
            moveForward = cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal;
        }

        // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。

        if (!isRolling)
        {
            rb.velocity = moveForward * moveSpeed + new Vector3(0, rb.velocity.y, 0);
        }
        else//ローリング中ならスピードアップ
        {
            rb.velocity = moveForward * moveSpeed * 0.6f + new Vector3(0, rb.velocity.y, 0);
        }


        //ロックオン解除時
        if (!cameraManager.rockOn)
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
            else
            {
            }
        //ロックオン時、常に敵の方を向く
        else
        {
            if (moveForward != Vector3.zero)
            {
                if (!isKnockBuck)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                                                          Quaternion.LookRotation(cameraManager.nearOne.transform.position-transform.position),
                                                          applySpeed);
                }
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
//            damageSource.userEnemy.soundManager.PlaySoundEffect(damageSource.userEnemy.attackID);
//            Debug.Log($"PlayerManagerのOnTriggerEnterでPlaySoundEffectが呼ばれた");
            animator.SetTrigger("hitDamage");

            damageAmount = damageSource.damageAmount;//データ上のHPを減らす
            HP -= damageAmount;
            float sliderValue = HP / MaxHP;
            Debug.Log($"プレイヤーのHPは{HP}");
            uiManager.UpdateHP(sliderValue);

            if (HP <= 0)//ノックアウト処理
            {
                Debug.Log("プレイヤーのHPが０以下になりました/ノックアウト処理");
//                soundManager.PlaySoundEffect(8);//アニメーターから音を出す？
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