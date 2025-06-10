using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip drawSound;
    [SerializeField]
    private AudioClip shootSound;

    [Header("Bow Setting")]
    [SerializeField]
    private BowSetting bowSetting;

    private float lastAttackTime = 0;

    private AudioSource audioSource;
    private PlayerAnimatorController animator;

    public GameObject arrow;
    public GameObject prefab_arrow;
    public GameObject Arrow_point;
    Camera cam;
    public float arrow_power = 10.0f;

    private bool ReadyFire = false;

    private float currentStamina = -1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<PlayerAnimatorController>();
        cam = Camera.main;
    }

    private void Update()
    {
       // Input.GetAxisRaw("Attack"), status.CurrentStamina);
    }
    
    public void StartBowAction(float bowpower, float stamina)
    {
        currentStamina = stamina;
        OnAttack(bowpower);
    }

    public void StopBowAction()
    {
        //animator.BowState = 0;
        //animator.triggerRelease();
        PlaySound(null);
    }

    public void ShootAction(float bowpower)
    {
        PlaySound(shootSound);
        arrow.SetActive(false);
        ReadyFire = false;
        Vector3 cam_forward = cam.transform.forward;
        GameObject t_arrow = Instantiate(prefab_arrow, Arrow_point.transform.position, Quaternion.Euler(cam.transform.rotation.x, cam.transform.rotation.y + 90, cam.transform.rotation.z));
        t_arrow.transform.right = -cam_forward;
        t_arrow.GetComponent<Rigidbody>().velocity = -t_arrow.transform.right * arrow_power * bowpower;
    }

    public void OnAttack(float bowpower)
    {
        if (animator.MoveSpeed > 0.5f || currentStamina == 0f)
        {  
            arrow.SetActive(false);
            animator.BowState = 0;
            ReadyFire = false;
            audioSource.Stop();

            return;
        }
        else
        {
            if (animator.BowState == 0 && Input.GetAxisRaw("Attack") > 0) { PlaySound(null); ReadyFire = true; }

            if (Input.GetButtonDown("Shoot") && ReadyFire)
            {
                Debug.Log("shoot");
                ShootAction(bowpower);
            }

            if (ReadyFire && bowpower > 0)
            {
                PlaySound(drawSound);
                arrow.SetActive(true);
            }
            else { arrow.SetActive(false); }

            animator.BowState = bowpower;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if(audioSource.clip == clip) { return; }
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
