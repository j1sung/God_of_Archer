using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCam : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private Transform camPivot;

    [SerializeField]
    private float rotCamXAxisSpeed = 5; //ī�޶� x�� ȸ���ӵ�
    [SerializeField]
    private float rotCamYAxisSpeed = 5; //ī�޶� y�� ȸ���ӵ�

    [Header("�𵨸��� ī�޶� ��ġ ��ȸ�� ����")]
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
        eulerAngleY += mouseX * rotCamYAxisSpeed;    //���콺 ��/�� �̵����� ī�޶� y�� ȸ��
        eulerAngleY = Mathf.Repeat(eulerAngleY, 360f);
        transform.localRotation = Quaternion.Euler(0f, eulerAngleY, 0f);

        eulerAngleX -= mouseY * rotCamXAxisSpeed;    //���콺 ��/�Ʒ� �̵����� ī�޶� x�� ȸ��

        // ī�޶� x�� ȸ���� ��� ȸ�� ������ ����
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