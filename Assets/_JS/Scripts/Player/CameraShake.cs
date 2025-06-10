using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour
{
    [Header("1) �̵� �ӵ� ����� (PlayerAnimatorController ��)")]
    [Tooltip("MoveSpeed(0~1)�� �����ϴ� ������Ʈ. 0=Idle, (0~0.5]=Walk, (0.5~1]=Run ���� �����մϴ�.")]
    [SerializeField] private PlayerAnimatorController playerAnimator = null;

    [Header("2) ��鸲 ���� ��� (�Ϲ������� Main Camera Transform)")]
    [Tooltip("��鸲�� ������ ������ Transform. ����θ� �� ��ũ��Ʈ�� ���� GameObject(transform)�� ����մϴ�.")]
    [SerializeField] private Transform shakeTarget = null;

    [Header("3) �Ҹ� ���� ����ġ/ȸ�� ��鸲���� �����ϱ� ���� ���� ����")]
    [Tooltip("��鸲�� ������ �� ����� �󸶳� ������ ������ (���� Ŭ���� ������ ����)")]
    [SerializeField] private float noiseFrequency = 1.0f;

    [Tooltip("����(Idle) �� ������ ������ ������� ���� ��ġ/ȸ������ ���ͽ�Ű�� �ӵ�")]
    [SerializeField] private float dampingSpeed = 1.0f;

    [Header("4) �ȱ�(Walk) ���¿����� ��ġ ��鸲 (Position Amplitude)")]
    [Tooltip("�ȱ� �� X��(�¿�) ��鸲 ���� (����, ����: m)")]
    [SerializeField] private float walkPosAmpX = 0.02f;
    [Tooltip("�ȱ� �� Y��(����) ��鸲 ���� (����, ����: m)")]
    [SerializeField] private float walkPosAmpY = 0.015f;

    [Header("5) �ȱ�(Walk) ���¿����� ȸ�� ��鸲 (Rotation Amplitude)")]
    [Tooltip("�ȱ� �� Pitch(X�� ȸ��) ��鸲 ���� (����, ����: deg)")]
    [SerializeField] private float walkRotAmpX = 0.3f;
    [Tooltip("�ȱ� �� Yaw(Y�� ȸ��) ��鸲 ���� (����, ����: deg)")]
    [SerializeField] private float walkRotAmpY = 0.25f;

    [Header("6) �ٱ�(Run) ���¿����� ��ġ ��鸲 (Position Amplitude)")]
    [Tooltip("�ٱ� �� X��(�¿�) ��鸲 ���� (����, ����: m)")]
    [SerializeField] private float runPosAmpX = 0.05f;
    [Tooltip("�ٱ� �� Y��(����) ��鸲 ���� (����, ����: m)")]
    [SerializeField] private float runPosAmpY = 0.04f;

    [Header("7) �ٱ�(Run) ���¿����� ȸ�� ��鸲 (Rotation Amplitude)")]
    [Tooltip("�ٱ� �� Pitch(X�� ȸ��) ��鸲 ���� (����, ����: deg)")]
    [SerializeField] private float runRotAmpX = 0.6f;
    [Tooltip("�ٱ� �� Yaw(Y�� ȸ��) ��鸲 ���� (����, ����: deg)")]
    [SerializeField] private float runRotAmpY = 0.5f;

    // ���ο��� ������ �ʱ� ��ġ/ȸ��
    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;

    // Perlin Noise�� ���� �ð���
    private float noiseTime = 0f;

    private void Awake()
    {
        // shakeTarget�� ��� ������ '�� ��ũ��Ʈ�� ���� ������Ʈ(transform)' �� �⺻������ ���
        if (shakeTarget == null)
            shakeTarget = this.transform;

        // ���� ������ �ʱ� ��ġ/ȸ���� ����
        initialLocalPos = shakeTarget.localPosition;
        initialLocalRot = shakeTarget.localRotation;
    }

    private void Update()
    {
        if (playerAnimator == null)
            return;

        // 1) ���� MoveSpeed ���� ������ (0 ~ 1)
        float moveSpeed = playerAnimator.MoveSpeed;

        // 2) �������� �ʴ�(Idle) ������ ��� �� ����(Damping)�� ����
        if (moveSpeed <= 0f)
        {
            // ���������� ���� ��ġ/ȸ������ ���ƿ����� ����
            shakeTarget.localPosition = Vector3.Lerp(shakeTarget.localPosition, initialLocalPos, Time.deltaTime * dampingSpeed);
            shakeTarget.localRotation = Quaternion.Slerp(shakeTarget.localRotation, initialLocalRot, Time.deltaTime * dampingSpeed);
            return;
        }

        // 3) �ȱ� �Ǵ� �ٱ� ���¿� ���� ���� �ٸ� ����(Amplitude) ���� ����
        float posAmpX, posAmpY, rotAmpX, rotAmpY;
        if (moveSpeed > 0.5f)
        {
            // Run ����
            posAmpX = runPosAmpX;
            posAmpY = runPosAmpY;
            rotAmpX = runRotAmpX;
            rotAmpY = runRotAmpY;
        }
        else
        {
            // Walk ����
            posAmpX = walkPosAmpX;
            posAmpY = walkPosAmpY;
            rotAmpX = walkRotAmpX;
            rotAmpY = walkRotAmpY;
        }

        // 4) noiseTime �� ��� ��� ���� (�ð� ����� ���� Perlin Noise ���� ��ȭ�ϵ���)
        noiseTime += Time.deltaTime * noiseFrequency;

        // 5) Perlin Noise ������� �� �ະ ������ ���� ���� (0~1) �� (-0.5~+0.5) ������ ��ȯ �� ���� ����
        float noiseX = (Mathf.PerlinNoise(noiseTime, 0f) - 0.5f) * 2f; // ��Ⱦ ���� ��� (X��)
        float noiseY = (Mathf.PerlinNoise(0f, noiseTime) - 0.5f) * 2f; // ���� ���� ��� (Y��)
        float noiseZ = (Mathf.PerlinNoise(noiseTime * 0.7f, noiseTime * 0.7f) - 0.5f) * 2f; // (Z�� ����� �ʿ� ���ٸ� 0f�� ���� ����)

        Vector3 posNoise = new Vector3(noiseX * posAmpX, noiseY * posAmpY, noiseZ * 0f);

        // 6) Rotation ������ Perlin Noise ��� �� Pitch(X��), Yaw(Y��) �� �ุ ���
        float noiseRotX = (Mathf.PerlinNoise(noiseTime * 1.2f, noiseTime * 0.8f) - 0.5f) * 2f; // Pitch
        float noiseRotY = (Mathf.PerlinNoise(noiseTime * 0.8f, noiseTime * 1.3f) - 0.5f) * 2f; // Yaw
        // Z��(Roll) ��鸲�� �ʿ��ϴٸ� �Ʒ�ó�� �߰��� ���� ������, 1��Ī ���ӿ��� �� ��鸲�� ���� ���� �����Ƿ� 0���� �δ� �� �������Դϴ�.
        float noiseRotZ = 0f;
        Vector3 rotNoiseEuler = new Vector3(noiseRotX * rotAmpX, noiseRotY * rotAmpY, noiseRotZ);

        // 7) ���� ��ġ/ȸ�� ����
        shakeTarget.localPosition = initialLocalPos + posNoise;
        shakeTarget.localRotation = initialLocalRot * Quaternion.Euler(rotNoiseEuler);
    }
}