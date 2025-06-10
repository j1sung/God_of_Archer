using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerController : MonoBehaviour
{
    [Header("Stamina")]
    [Tooltip("플레이어가 달릴 수 있는 최대 시간 (초 단위)")]
    [SerializeField] 
    private float runDuration = 7f;
    [Tooltip("활을 당길 수 있는 최대 시간 (초 단위)")]
    [SerializeField] 
    private float drawDuration = 5f;

    private PlayerStatus status;
    private float runCost => status.MaxStamina / runDuration; // 달리기 소모 코스트
    private float drawCost => status.MaxStamina / drawDuration;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipWalk;
    [SerializeField] 
    private AudioClip audioClipRun;

    [Header("Audio Mixer Groups")]
    [SerializeField] private AudioMixerGroup walkMixerGroup;
    [SerializeField] private AudioMixerGroup runMixerGroup;

    //private RotateYaw _rotateYaw;
    private RotateCam _rotateCam;
    private MovementCharacterController _movementCharacterController;
    private PlayerAnimatorController animator;
    private AudioSource audioSource;
    
    bool running = false; // 달리는가?
    bool attacking = false; // 공격중인가?
    bool isFirstDeath = true; // 첫 죽음인가?


    private void Awake()
    {
        //마우스 커서를 보이지 않게 설정하고 현재 위치에 고정
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _rotateCam = GetComponent<RotateCam>();
        _movementCharacterController = GetComponent<MovementCharacterController>();

        status = GetComponent<PlayerStatus>();
       //status.OnDeath += Die; // 사망 이벤트 구독

        animator = GetComponent<PlayerAnimatorController>();    
        audioSource = GetComponent<AudioSource>();
    } 

    void Update()
    {
        running = UpdateMove(); 
        attacking = UpdateAttack();
        UpdateRotate();
        UpdateJump();

        if (!running && !attacking) // 달리거나 공격중엔 스테미나 회복 안됨!
        {
            status.RecoverStamina();
        }
        if(status.CurrentHp == 0 && isFirstDeath) // HP가 0이면 죽음
        {
            isFirstDeath = false;
            Die();
        }
    }

    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        //float joystickX = Input.GetAxis("JoyX");
        //float joystickY = Input.GetAxis("JoyY");

        _rotateCam.UpdateRotate(mouseX, mouseY);
    }

    private bool UpdateMove()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 달리기 조건: 앞으로 이동 중이고 Run 키 누름
        bool isTryingToRun = z > 0 && Input.GetButton("Run");

        // 현재 스태미나가 달릴 수 있을 만큼 남아 있는가?
        bool hasStamina = status.CurrentStamina > 0f;

        // 최종적으로 달릴 수 있는가?
        bool isRun = isTryingToRun && hasStamina;

        //이동 중 일 떼 (걷기 or 뛰기)
        if (x != 0 || z != 0)
        {
            _movementCharacterController.MoveSpeed = (isRun == true) ? status.RunSpeed : status.WalkSpeed;
            animator.MoveSpeed = (isRun == true) ? 1 : 0.5f;
            audioSource.clip = (isRun == true) ? audioClipRun : audioClipWalk;
            audioSource.outputAudioMixerGroup = (isRun == true) ? runMixerGroup : walkMixerGroup;

            if (audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            _movementCharacterController.MoveSpeed = 0;
            animator.MoveSpeed = 0;

            if(audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }

        _movementCharacterController.MoveTo(new Vector3(x,0,z));

        if (isRun == true)
        {
            status.UseStamina(runCost); // 스테미나 소모(뛰기)
            return true; // 기력 회복 불가(뛰기)
        }
        
        return false; // 기력 회복 가능(걷기, 정지)
    }

    private void UpdateJump() 
    {
        if(Input.GetButtonDown("Jump") && _movementCharacterController.IsGrounded)
        {
            _movementCharacterController.Jump();
            animator.TriggerJump();
        }
    }

    private bool UpdateAttack()
    {

        if (animator.BowState > 0.5f)
        {
            status.UseStamina(drawCost);
            return true; // 기력 회복 불가
        }
        return false; // 기력 회복 가능(animator.BowState <= 0.5f)
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");
        animator.TriggerDie();
    }

    /*
    private void OnDestroy()
    {
        if (status != null)
            status.OnDeath -= Die; // 구독 해제 (메모리 누수 방지)
    }
    */
}
