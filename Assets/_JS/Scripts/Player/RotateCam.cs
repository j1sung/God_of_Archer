using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCam : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private Transform camPivot;

    [SerializeField]
    private float rotCamXAxisSpeed = 5; //카메라 x축 회전속도
    [SerializeField]
    private float rotCamYAxisSpeed = 5; //카메라 y축 회전속도

    [Header("모델링과 카메라 위치 각회전 제한")]
    [SerializeField] private float limitMinX = -80f;
    [SerializeField] private float limitMaxX = 50f;
    [SerializeField] private float offsetMinX = 0f;
    [SerializeField] private float offsetMaxX = 0f;


    private float eulerAngleX;
    private float cameraAngleX;
    private float eulerAngleY;


    public float EulerAngle => eulerAngleX;

    private void Start()
    {
        cam.localRotation = Quaternion.identity;
        camPivot.localRotation = Quaternion.identity;
    }

    public void UpdateRotate(float mouseX, float mouseY)
    {
        eulerAngleY += mouseX * rotCamYAxisSpeed;    //마우스 좌/우 이동으로 카메라 y축 회전
        eulerAngleY = Mathf.Repeat(eulerAngleY, 360f);
        transform.localRotation = Quaternion.Euler(0f, eulerAngleY, 0f);

        eulerAngleX -= mouseY * rotCamXAxisSpeed;    //마우스 위/아래 이동으로 카메라 x축 회전

        // 카메라 x축 회전의 경우 회전 범위를 설정
        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);
        cameraAngleX = ClampAngle(eulerAngleX, limitMinX + offsetMinX, limitMaxX + offsetMaxX);

        cam.localRotation = Quaternion.Euler(cameraAngleX, 0f, 0f);
        camPivot.localRotation = Quaternion.Euler(eulerAngleX, 0f, 0f);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

}