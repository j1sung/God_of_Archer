using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateYaw : MonoBehaviour
{
    [SerializeField]
    private float rotCamXAxisSpeed = 5; //ī�޶� x�� ȸ���ӵ�

    private float eulerAngleY;

    public void UpdateRotateYaw(float X)
    {
        eulerAngleY += X * rotCamXAxisSpeed;    //��/�� �̵����� ī�޶� y�� ȸ��

        // ĳ���� ��ü ȸ��(�¿� Y�ุ ����)
        transform.rotation = Quaternion.Euler(0, eulerAngleY, 0);
    }
}
