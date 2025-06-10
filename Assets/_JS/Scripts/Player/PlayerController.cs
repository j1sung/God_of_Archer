using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerController : MonoBehaviour
{
    [Header("Stamina")]
    [Tooltip("�÷��̾ �޸� �� �ִ� �ִ� �ð� (�� ����)")]
    [SerializeField] 
    private float runDuration = 7f;
    [Tooltip("Ȱ�� ��� �� �ִ� �ִ� �ð� (�� ����)")]
    [SerializeField] 
    private float drawDuration = 5f;

    private PlayerStatus status;
    private float runCost => status.MaxStamina / runDuration; // �޸��� �Ҹ� �ڽ�Ʈ
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
    
    bool running = false; // �޸��°�?
    bool attacking = false; // �������ΰ�?
    bool isFirstDeath = true; // ù �����ΰ�?


    private void Awake()
    {
        //���콺 Ŀ���� ������ �ʰ� �����ϰ� ���� ��ġ�� ����
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _rotateCam = GetComponent<RotateCam>();
        _movementCharacterController = GetComponent<MovementCharacterController>();

        status = GetComponent<PlayerStatus>();
       //status.OnDeath += Die; // ��� �̺�Ʈ ����

        animator = GetComponent<PlayerAnimatorController>();    
        audioSource = GetComponent<AudioSource>();
    } 

    void Update()
    {
        running = UpdateMove(); 
        attacking = UpdateAttack();
        UpdateRotate();
        UpdateJump();

        if (!running && !attacking) // �޸��ų� �����߿� ���׹̳� ȸ�� �ȵ�!
        {
            status.RecoverStamina();
        }
        if(status.CurrentHp == 0 && isFirstDeath) // HP�� 0�̸� ����
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

        // �޸��� ����: ������ �̵� ���̰� Run Ű ����
        bool isTryingToRun = z > 0 && Input.GetButton("Run");

        // ���� ���¹̳��� �޸� �� ���� ��ŭ ���� �ִ°�?
        bool hasStamina = status.CurrentStamina > 0f;

        // ���������� �޸� �� �ִ°�?
        bool isRun = isTryingToRun && hasStamina;

        //�̵� �� �� �� (�ȱ� or �ٱ�)
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
            status.UseStamina(runCost); // ���׹̳� �Ҹ�(�ٱ�)
            return true; // ��� ȸ�� �Ұ�(�ٱ�)
        }
        
        return false; // ��� ȸ�� ����(�ȱ�, ����)
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
            return true; // ��� ȸ�� �Ұ�
        }
        return false; // ��� ȸ�� ����(animator.BowState <= 0.5f)
    }

    private void Die()
    {
        Debug.Log("�÷��̾� ���");
        animator.TriggerDie();
    }

    /*
    private void OnDestroy()
    {
        if (status != null)
            status.OnDeath -= Die; // ���� ���� (�޸� ���� ����)
    }
    */
}
