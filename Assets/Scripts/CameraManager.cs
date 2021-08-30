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
    GameObject nearOne;
    Quaternion initQuaternion;
    GameObject rockOnObject;

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
        if (Input.GetKeyDown(KeyCode.L))//ロックオンボタン押下時
        {
            targetObjList.Clear();
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                int i = 1;
                if (Vector3.Angle(playerObj.transform.position - Camera.main.transform.position, enemy.transform.position - playerObj.transform.position) < 60 && (playerObj.transform.position - enemy.transform.position).magnitude < 20)
                {
                    i++;
                    targetObjList.Add(enemy);
                    Debug.Log($"ロックオン候補{i}は距離{(playerObj.transform.position - enemy.transform.position).magnitude}");
                }
            }
            Debug.Log($"敵の数は{targetObjList.Count}体。");
            if (targetObjList.Count != 0)
            {
                rockOn = !rockOn;

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
            if (rockOnObject != null)
            {
                rockOnObject.GetComponentInChildren<EnemyUIManager>().rockOnMarker.enabled = false;
            }

            MoveCamera();

            RotateCameraByKeyboard();

            ResetCameraAngle();

            if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftShift))//デバッグ用コマンド　カメラの初期位置調整用
            {
                Start();
            }
        }
        else//ロックオン時
        {
            if (nearOne.tag != "Enemy")//敵撃破時、TagをEnemy以外に変更することで、ロックオンを自動的に外す
            {
                rockOn = false;
                rockOnObject = null;
            }
            else
            {
                nearOne.GetComponentInChildren<EnemyUIManager>().rockOnMarker.transform.position=nearOne.transform.position+(Camera.main.transform.position-nearOne.transform.position).normalized+new Vector3(0,1,0);
                nearOne.GetComponentInChildren<EnemyUIManager>().rockOnMarker.enabled = true;

                rockOnObject = nearOne;


                previousTargetPos = nearOne.transform.position;
                Vector3 cameraMoveTo = playerObj.transform.position + (new Vector3((playerObj.transform.position - nearOne.transform.position).normalized.x, 0, (playerObj.transform.position - nearOne.transform.position).normalized.z) * 10) + new Vector3(0, initialCameraPos.y, 0);
                transform.position = Vector3.Slerp(transform.position, cameraMoveTo, 0.2f);
                previousPlayerPos = playerObj.transform.position;
                transform.LookAt(nearOne.transform.position);


            }
        }
    }

    void MoveCamera()// playerの移動量分、自分（カメラ）も移動する
    {
        transform.position += playerObj.transform.position - previousPlayerPos;//ここ、ロックオン時にも解除時にもなめらかに移行するようにできないか
        previousPlayerPos = playerObj.transform.position;
//        transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(10/180f,transform.rotation.y, transform.rotation.z, transform.rotation.w), 0.2f);
//どんどん斜めになっていく。地面に対して一定の角度を目指して徐々に位置を修正したい。
    }

    void RotateCameraByKeyboard()
    {

        float keyInputX = 0;

        if (Input.GetKey(KeyCode.K))
        {
            keyInputX += 2f;
        }
        if (Input.GetKey(KeyCode.H))
        {
            keyInputX -= 2f;
        }

        if (Input.GetKey(KeyCode.I))
        {
            keyInputX += 5f;
        }
        if (Input.GetKey(KeyCode.Y))
        {
            keyInputX -= 5f;
        }
        //if (Input.GetKey(KeyCode.U))
        //{
        //    keyInputX *= 3f;
        //}

        transform.RotateAround(previousPlayerPos, Vector3.up, keyInputX * Time.deltaTime * camSpeed);
    }

    void ResetCameraAngle()
    {
        Quaternion previousRotation = this.transform.localRotation;
        Vector3 rotationAngles = previousRotation.eulerAngles;
        rotationAngles.x = 10f;
        Quaternion rotation = Quaternion.Euler(rotationAngles);
        this.transform.localRotation = Quaternion.Slerp(previousRotation, rotation, 0.2f);
    }

}