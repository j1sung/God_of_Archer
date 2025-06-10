using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateYaw : MonoBehaviour
{
    [SerializeField]
    private float rotCamXAxisSpeed = 5; //카메라 x축 회전속도

    private float eulerAngleY;

    public void UpdateRotateYaw(float X)
    {
        eulerAngleY += X * rotCamXAxisSpeed;    //좌/우 이동으로 카메라 y축 회전

        // 캐릭터 전체 회전(좌우 Y축만 적용)
        transform.rotation = Quaternion.Euler(0, eulerAngleY, 0);
    }
}
