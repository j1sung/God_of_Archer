using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateModel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAnimatorController playerAnimator;
    [SerializeField] private Transform spine;
    [SerializeField] private Animator anim;

    [Header("Speeds & Limits")]
    [SerializeField] private float pitchSpeed = 5f; //ī�޶� Pitch ȸ���ӵ�
    [SerializeField] private float rollSpeed = 5f; //ī�޶� Roll ȸ���ӵ�
    [SerializeField] private float minRoll = -80;  //Roll ȸ�� ����(�ּ�)
    [SerializeField] private float maxRoll = 50;   //Roll ȸ�� ����(�ִ�)
    [SerializeField] private float minPitch = -120;  //Pitch ȸ�� ����(�ּ�)
    [SerializeField] private float maxPitch = 50;   //Pitch ȸ�� ����(�ִ�)

    [Header("Head Correction Offsets")]
    [Tooltip("Idle ������ �� Head�� �߰� ȸ�� ���� (Euler.x, y, z ��)")]
    [SerializeField] private Vector3 headOffsetIdle = Vector3.zero;
    [Tooltip("Walk ������ �� Head�� �߰� ȸ�� ����")]
    [SerializeField] private Vector3 headOffsetWalk = Vector3.zero;
    [Tooltip("Run ������ �� Head�� �߰� ȸ�� ����")]
    [SerializeField] private Vector3 headOffsetRun = Vector3.zero;
    [Tooltip("Jump ������ �� Head�� �߰� ȸ�� ����")]
    [SerializeField] private Vector3 headOffsetJump = Vector3.zero;

    private float eulerPitch;
    private float eulerRoll;


    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y");

        UpdatePitch(mouseY);
        UpdateRoll(mouseY);
    }

    void UpdatePitch(float mouseY)
    {
        eulerPitch -= mouseY * pitchSpeed;    //��/�Ʒ� �̵����� ī�޶� x�� ȸ��
        eulerPitch = ClampAngle(eulerPitch, minPitch, maxPitch); // pitch ȸ�� ������ ����
    }
    void UpdateRoll(float mouseY)
    {
        eulerRoll -= mouseY * rollSpeed;
        eulerRoll = ClampAngle(eulerRoll, minRoll, maxRoll);
        
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    // IK �ܰ迡�� ���� ����� �ݴϴ�.
    void OnAnimatorIK(int layerIndex)
    {
        float moveSpeed = playerAnimator.MoveSpeed;
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        Quaternion baseRot; // ��ü(spine) ȸ�� ���
        Vector3 headOffsetEuler; // ���º� Head �߰� ���� ������ ����

        if (info.IsTag("Jump"))
        {
            baseRot = Quaternion.Euler(0f, 0f, eulerRoll);
            headOffsetEuler = headOffsetJump;
        }
        else
        {
            if (moveSpeed == 0f)
            {
                baseRot = Quaternion.Euler(0f, 0f, eulerRoll); // Idle �� Z�� ȸ��
                headOffsetEuler = headOffsetIdle;
            }
            else if (moveSpeed <= 0.5f)
            {
                baseRot = Quaternion.Euler(0f, 0f, eulerRoll); // Walk �� Z�� ȸ��
                headOffsetEuler = headOffsetWalk;
            }
            else
            {
                baseRot = Quaternion.Euler(eulerPitch, 0f, 0f); // Run (moveSpeed > 0.5f) �� X�� ȸ��
                headOffsetEuler = headOffsetRun;
            }
        }
        
        Quaternion headOffsetRot = Quaternion.Euler(headOffsetEuler);

        // Spine ���� �����
        playerAnimator.SetSpineRotation(baseRot);
        playerAnimator.SetHeadRotation(headOffsetRot);
    }
}
