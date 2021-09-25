using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject playerObj;
    Vector3 previousPlayerPos;
    public List<GameObject> targetObjList = new List<GameObject>();
    Vector3 previousTargetPos;
    public float camSpeed;
    public Vector3 initialCameraPos;
    public bool rockOn;
    public GameObject nearOne;
    Quaternion initQuaternion;
    GameObject rockOnObject;
    [SerializeField] UIManager uIManager=default;

    void Start()
    {
        playerObj = GameObject.Find("Player");
        previousPlayerPos = playerObj.transform.position;
        transform.position = playerObj.transform.position + initialCameraPos;
        rockOn = false;
        initQuaternion = transform.rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Semicolon))//ロックオンボタン押下時、ロックオン・オフを切り替える処理
        {
            if (rockOn)
            {
                rockOn = false;
                return;
            }
            targetObjList.Clear();
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))//シーン中のEnemyタグのついたオブジェクト達を取得
            {
                int i = 1;
                //画面中央、キャラより奥にいて一定の距離内にいる敵にロックオン候補を絞る
                if (Vector3.Angle(playerObj.transform.position - Camera.main.transform.position, enemy.transform.position - playerObj.transform.position) < 60 && (playerObj.transform.position - enemy.transform.position).magnitude < 20)
                {
                    i++;
                    targetObjList.Add(enemy);
                }
            }

            //ロックオン可能な敵がいた場合
            if (targetObjList.Count != 0)
            {
                rockOn = !rockOn;

                //最も近い敵をnearOneとする
                nearOne = targetObjList[0];
                foreach (GameObject enemy in targetObjList)
                {
                    if (Vector3.Distance(playerObj.transform.position, nearOne.transform.position) > Vector3.Distance(playerObj.transform.position, enemy.transform.position))
                    {
                        nearOne = enemy;
                    }
                }
            }

        }


        if (!rockOn)//非ロックオン時
        {
            if (rockOnObject != null)//ロックオンをしていて敵を倒した際の処理
            {
                //ロックオンを解除し、マーカーを非表示に
                uIManager.rockOnMarker.enabled = false;
            }

            MoveCamera();

            RotateCameraByKeyboard();

            ResetCameraAngle();

            //if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftShift))//デバッグ用コマンド　カメラの初期位置調整用
            //{
            //    Start();
            //}
        }
        else//ロックオン時
        {
            if (nearOne.tag != "Enemy")//敵撃破時、TagをEnemy以外に変更することで、ロックオンを自動的に外す
            {
                rockOn = false;
                rockOnObject = null;
                uIManager.rockOnMarker.enabled = false;
            }
            else//ロックオン時の処理
            {
                //ロックオンマーカーを表示
                rockOnObject = nearOne;

                if ((playerObj.transform.position - rockOnObject.transform.position).magnitude > 20)
                {
                    rockOn = false;
                    return;
                }

                uIManager.rockOnMarker.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, rockOnObject.transform.position);
                uIManager.rockOnMarker.enabled = true;

                previousTargetPos = nearOne.transform.position;//カメラを敵とプレイヤーの延長線上に動かす
                transform.position = Vector3.Slerp(transform.position, playerObj.transform.position + (new Vector3((playerObj.transform.position - nearOne.transform.position).normalized.x, 0, (playerObj.transform.position - nearOne.transform.position).normalized.z) * 10) + new Vector3(0, initialCameraPos.y, 0), 0.2f);//カメラ位置の移動
                transform.LookAt(nearOne.transform.position);//カメラをターゲットに向ける
                previousPlayerPos = playerObj.transform.position;


            }
        }
    }

    void MoveCamera()// playerの移動量分、自分（カメラ）も移動する
    {
        transform.position += playerObj.transform.position - previousPlayerPos;//ここ、ロックオン時にも解除時にもなめらかに移行するようにできないか
        previousPlayerPos = playerObj.transform.position;
    }

    void RotateCameraByKeyboard()//カメラの左右移動
    {

        float keyInputX = 0;

        if (Input.GetKey(KeyCode.J))
        {
            keyInputX -= 2.5f;
        }
        if (Input.GetKey(KeyCode.L))
        {
            keyInputX += 2.5f;
        }

        transform.RotateAround(previousPlayerPos, Vector3.up, keyInputX * Time.deltaTime * camSpeed);
    }

    void ResetCameraAngle()//カメラの上下アングルを最初の角度に戻す。今や意味不明のオーパーツ。
    {
        Quaternion previousRotation = this.transform.localRotation;
        Vector3 rotationAngles = previousRotation.eulerAngles;
        rotationAngles.x = 10f;
        Quaternion rotation = Quaternion.Euler(rotationAngles);
        this.transform.localRotation = Quaternion.Slerp(previousRotation, rotation, 0.2f);
    }

}