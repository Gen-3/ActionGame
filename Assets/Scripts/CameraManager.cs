using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject playerObj;
    Vector3 previousPlayerPos;
    public GameObject[] targetObjs = new GameObject[0];
    Vector3 previousTargetPos;
    public float camSpeed;
    public Vector3 initialCameraPos;
    public bool rockOn;

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
            targetObjs = GameObject.FindGameObjectsWithTag("Enemy");
            Debug.Log($"敵の数は{targetObjs.Length}");
        }
        if (!rockOn)
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
        else
        {
            previousTargetPos = targetObjs[0].transform.position;
            transform.position = playerObj.transform.position + (playerObj.transform.position - targetObjs[0].transform.position).normalized * initialCameraPos.magnitude + new Vector3(0,initialCameraPos.y,0);
            transform.LookAt(targetObjs[0].transform.position);
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
