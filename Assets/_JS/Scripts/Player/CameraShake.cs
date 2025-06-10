using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour
{
    [Header("1) 이동 속도 참고용 (PlayerAnimatorController 등)")]
    [Tooltip("MoveSpeed(0~1)를 제공하는 컴포넌트. 0=Idle, (0~0.5]=Walk, (0.5~1]=Run 으로 간주합니다.")]
    [SerializeField] private PlayerAnimatorController playerAnimator = null;

    [Header("2) 흔들림 적용 대상 (일반적으로 Main Camera Transform)")]
    [Tooltip("흔들림을 실제로 적용할 Transform. 비워두면 이 스크립트가 붙은 GameObject(transform)를 사용합니다.")]
    [SerializeField] private Transform shakeTarget = null;

    [Header("3) 소리 없이 ‘위치/회전 흔들림’만 제어하기 위한 공통 설정")]
    [Tooltip("흔들림을 생성할 때 노이즈가 얼마나 빠르게 변할지 (값이 클수록 진동이 빠름)")]
    [SerializeField] private float noiseFrequency = 1.0f;

    [Tooltip("정지(Idle) 시 진동을 빠르게 감쇠시켜 원래 위치/회전으로 복귀시키는 속도")]
    [SerializeField] private float dampingSpeed = 1.0f;

    [Header("4) 걷기(Walk) 상태에서의 위치 흔들림 (Position Amplitude)")]
    [Tooltip("걷기 시 X축(좌우) 흔들림 진폭 (±값, 단위: m)")]
    [SerializeField] private float walkPosAmpX = 0.02f;
    [Tooltip("걷기 시 Y축(상하) 흔들림 진폭 (±값, 단위: m)")]
    [SerializeField] private float walkPosAmpY = 0.015f;

    [Header("5) 걷기(Walk) 상태에서의 회전 흔들림 (Rotation Amplitude)")]
    [Tooltip("걷기 시 Pitch(X축 회전) 흔들림 진폭 (±값, 단위: deg)")]
    [SerializeField] private float walkRotAmpX = 0.3f;
    [Tooltip("걷기 시 Yaw(Y축 회전) 흔들림 진폭 (±값, 단위: deg)")]
    [SerializeField] private float walkRotAmpY = 0.25f;

    [Header("6) 뛰기(Run) 상태에서의 위치 흔들림 (Position Amplitude)")]
    [Tooltip("뛰기 시 X축(좌우) 흔들림 진폭 (±값, 단위: m)")]
    [SerializeField] private float runPosAmpX = 0.05f;
    [Tooltip("뛰기 시 Y축(상하) 흔들림 진폭 (±값, 단위: m)")]
    [SerializeField] private float runPosAmpY = 0.04f;

    [Header("7) 뛰기(Run) 상태에서의 회전 흔들림 (Rotation Amplitude)")]
    [Tooltip("뛰기 시 Pitch(X축 회전) 흔들림 진폭 (±값, 단위: deg)")]
    [SerializeField] private float runRotAmpX = 0.6f;
    [Tooltip("뛰기 시 Yaw(Y축 회전) 흔들림 진폭 (±값, 단위: deg)")]
    [SerializeField] private float runRotAmpY = 0.5f;

    // 내부에서 보관할 초기 위치/회전
    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;

    // Perlin Noise를 위한 시간값
    private float noiseTime = 0f;

    private void Awake()
    {
        // shakeTarget이 비어 있으면 '이 스크립트가 붙은 오브젝트(transform)' 를 기본값으로 사용
        if (shakeTarget == null)
            shakeTarget = this.transform;

        // 시작 시점의 초기 위치/회전을 저장
        initialLocalPos = shakeTarget.localPosition;
        initialLocalRot = shakeTarget.localRotation;
    }

    private void Update()
    {
        if (playerAnimator == null)
            return;

        // 1) 현재 MoveSpeed 값을 가져옴 (0 ~ 1)
        float moveSpeed = playerAnimator.MoveSpeed;

        // 2) 움직이지 않는(Idle) 상태인 경우 → 감쇠(Damping)만 수행
        if (moveSpeed <= 0f)
        {
            // 점진적으로 원래 위치/회전으로 돌아오도록 보간
            shakeTarget.localPosition = Vector3.Lerp(shakeTarget.localPosition, initialLocalPos, Time.deltaTime * dampingSpeed);
            shakeTarget.localRotation = Quaternion.Slerp(shakeTarget.localRotation, initialLocalRot, Time.deltaTime * dampingSpeed);
            return;
        }

        // 3) 걷기 또는 뛰기 상태에 따라 각각 다른 진폭(Amplitude) 값을 선택
        float posAmpX, posAmpY, rotAmpX, rotAmpY;
        if (moveSpeed > 0.5f)
        {
            // Run 상태
            posAmpX = runPosAmpX;
            posAmpY = runPosAmpY;
            rotAmpX = runRotAmpX;
            rotAmpY = runRotAmpY;
        }
        else
        {
            // Walk 상태
            posAmpX = walkPosAmpX;
            posAmpY = walkPosAmpY;
            rotAmpX = walkRotAmpX;
            rotAmpY = walkRotAmpY;
        }

        // 4) noiseTime 을 계속 흘려 보냄 (시간 경과에 따라 Perlin Noise 값이 변화하도록)
        noiseTime += Time.deltaTime * noiseFrequency;

        // 5) Perlin Noise 기반으로 각 축별 노이즈 값을 생성 (0~1) → (-0.5~+0.5) 범위로 변환 → 진폭 곱셈
        float noiseX = (Mathf.PerlinNoise(noiseTime, 0f) - 0.5f) * 2f; // 종횡 방향 흔들 (X축)
        float noiseY = (Mathf.PerlinNoise(0f, noiseTime) - 0.5f) * 2f; // 상하 방향 흔들 (Y축)
        float noiseZ = (Mathf.PerlinNoise(noiseTime * 0.7f, noiseTime * 0.7f) - 0.5f) * 2f; // (Z축 흔들은 필요 없다면 0f로 고정 가능)

        Vector3 posNoise = new Vector3(noiseX * posAmpX, noiseY * posAmpY, noiseZ * 0f);

        // 6) Rotation 진동도 Perlin Noise 사용 → Pitch(X축), Yaw(Y축) 두 축만 사용
        float noiseRotX = (Mathf.PerlinNoise(noiseTime * 1.2f, noiseTime * 0.8f) - 0.5f) * 2f; // Pitch
        float noiseRotY = (Mathf.PerlinNoise(noiseTime * 0.8f, noiseTime * 1.3f) - 0.5f) * 2f; // Yaw
        // Z축(Roll) 흔들림이 필요하다면 아래처럼 추가할 수도 있지만, 1인칭 게임에서 롤 흔들림은 거의 쓰지 않으므로 0으로 두는 게 안정적입니다.
        float noiseRotZ = 0f;
        Vector3 rotNoiseEuler = new Vector3(noiseRotX * rotAmpX, noiseRotY * rotAmpY, noiseRotZ);

        // 7) 최종 위치/회전 적용
        shakeTarget.localPosition = initialLocalPos + posNoise;
        shakeTarget.localRotation = initialLocalRot * Quaternion.Euler(rotNoiseEuler);
    }
}