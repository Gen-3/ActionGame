using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject targetObj;
    Vector3 targetPos;
    public float camSpeed;
    public Vector3 cameraPos;

    void Start()
    {
        //targetObj = GameObject.Find("Player");
        targetPos = targetObj.transform.position;
        transform.position = targetPos + cameraPos;
    }

    void Update()
    {
        MoveCamera();

        RotateCameraByKeyboard();

        if (Input.GetMouseButton(1))// マウスの右クリックを押している間
        {
            RotateCameraByMouse();
        }

        if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftShift))
        {
            Start();
        }
    }

    void MoveCamera()// targetの移動量分、自分（カメラ）も移動する
    {
        transform.position += targetObj.transform.position - targetPos;
        targetPos = targetObj.transform.position;
    }

    void RotateCameraByMouse()
    {
        // マウスの移動量
        float mouseInputX = Input.GetAxis("Mouse X");
        //float mouseInputY = Input.GetAxis("Mouse Y");
        // targetの位置のY軸を中心に、回転（公転）する
        transform.RotateAround(targetPos, Vector3.up, mouseInputX * Time.deltaTime * camSpeed);
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

        transform.RotateAround(targetPos, Vector3.up, keyInputX * Time.deltaTime * camSpeed);

    }
}
