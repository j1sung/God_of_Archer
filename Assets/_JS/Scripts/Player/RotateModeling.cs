using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class RotateModeling : MonoBehaviour
{
    [Header("References")]
    [Tooltip("�� ������Ʈ(Animator�� �پ� �ִ� GameObject)")]
    [SerializeField] private PlayerAnimatorController animator;
    [Tooltip("ī�޶� �ٿ� ���� �� GameObject (Main Camera�� �θ�)")]
    [SerializeField] private RotateCam cam;

    [SerializeField] private Animator anim;

    [Header("Head Correction Offsets")]
    [Tooltip("Idle ������ �� Head�� �߰� ȸ�� ���� (Euler.x, y, z ��)")]
    [SerializeField] private Vector3 headOffsetIdle = Vector3.zero;
    [Tooltip("Walk ������ �� Head�� �߰� ȸ�� ����")]
    [SerializeField] private Vector3 headOffsetWalk = Vector3.zero;
    [Tooltip("Run ������ �� Head�� �߰� ȸ�� ����")]
    [SerializeField] private Vector3 headOffsetRun = Vector3.zero;
    [Tooltip("Jump ������ �� Head�� �߰� ȸ�� ����")]
    [SerializeField] private Vector3 headOffsetJump = Vector3.zero;

    private Transform spineBone;
    void Start()
    {
        spineBone = animator.GetSpineTransform;
        spineBone.localRotation = Quaternion.identity;
    }

    void OnAnimatorIK(int layerIndex)
    {
        float moveSpeed = animator.MoveSpeed;
        AnimatorStateInfo baseLayer = anim.GetCurrentAnimatorStateInfo(0);

        Quaternion baseRot; // ��ü(spine) ȸ�� ���
        Vector3 headOffsetEuler; // ���º� Head �߰� ���� ������ ����

        
        if (moveSpeed == 0f)
        {
            baseRot = Quaternion.Euler(0f, 0f, cam.EulerAngle); // Idle �� Z�� ȸ��
            headOffsetEuler = headOffsetIdle;
        }
        else if (moveSpeed <= 0.5f)
        {
            baseRot = Quaternion.Euler(0f, 0f, cam.EulerAngle); // Walk �� Z�� ȸ��
            headOffsetEuler = headOffsetWalk;
        }
        else
        {
            baseRot = Quaternion.Euler(cam.EulerAngle, 0f, 0f); // Run (moveSpeed > 0.5f) �� X�� ȸ��
            headOffsetEuler = headOffsetRun;
        }
        if (baseLayer.IsTag("Jump"))
        {
            baseRot = Quaternion.Euler(0f, 0f, cam.EulerAngle);
            headOffsetEuler = headOffsetJump;
        }
        Quaternion headOffsetRot = Quaternion.Euler(headOffsetEuler);

        // Spine ���� �����
        animator.SetSpineRotation(baseRot);
        animator.SetHeadRotation(headOffsetRot);

        
    }
}
