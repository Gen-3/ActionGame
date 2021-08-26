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

    void Start()
    {
        playerObj = GameObject.Find("Player");
        previousPlayerPos = playerObj.transform.position;
        transform.position = playerObj.transform.position + initialCameraPos;
        rockOn = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            rockOn = !rockOn;
            targetObjList.Clear();
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                targetObjList.Add(enemy);
            }

            Debug.Log($"敵の数は{targetObjList.Count}体。");
            nearOne = targetObjList[0];
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (Vector3.Distance(playerObj.transform.position, nearOne.transform.position) > Vector3.Distance(playerObj.transform.position, enemy.transform.position))
                {
                    nearOne = enemy;
                }
            }
        }

        if (!rockOn)//非ロックオン時
        {
            MoveCamera();

            RotateCameraByKeyboard();

            if (Input.GetMouseButton(1))// マウスの右クリックを押している間
            {
                RotateCameraByMouse();
            }

            if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftShift))//デバッグ用コマンド　カメラの初期位置調整用
            {
                Start();
            }
        }
        else//ロックオン時
        {
            previousTargetPos = nearOne.transform.position;
            transform.position = playerObj.transform.position + (playerObj.transform.position - nearOne.transform.position).normalized * initialCameraPos.magnitude + new Vector3(0, initialCameraPos.y, 0);
            transform.LookAt(nearOne.transform.position);
            previousPlayerPos = playerObj.transform.position;
        }
    }

    void MoveCamera()// targetの移動量分、自分（カメラ）も移動する
    {
        transform.position += playerObj.transform.position - previousPlayerPos;
        previousPlayerPos = playerObj.transform.position;
    }

    void RotateCameraByMouse()
    {
        // マウスの移動量
        float mouseInputX = Input.GetAxis("Mouse X");
        //float mouseInputY = Input.GetAxis("Mouse Y");
        // targetの位置のY軸を中心に、回転（公転）する
        transform.RotateAround(previousPlayerPos, Vector3.up, mouseInputX * Time.deltaTime * camSpeed);
        // カメラの垂直移動（※角度制限なし、必要が無ければコメントアウト）
        //transform.RotateAround(targetPos, transform.right, mouseInputY * Time.deltaTime * 200f);


    }

    void RotateCameraByKeyboard()
    {

        float keyInputX = 0;

        if (Input.GetKey(KeyCode.K))
        {
            keyInputX += 1f;
        }
        if (Input.GetKey(KeyCode.H))
        {
            keyInputX -= 1f;
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
}