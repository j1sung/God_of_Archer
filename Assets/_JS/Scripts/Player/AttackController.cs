using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AttackController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField]
    private AudioClip drawSound;
    [SerializeField]
    private AudioClip shootSound;
    [SerializeField] private AudioSource audioSource;

    [Header("Audio Mixer Groups")]
    [SerializeField] private AudioMixerGroup drawMixerGroup;
    [SerializeField] private AudioMixerGroup shootMixerGroup;

    [SerializeField] private PlayerAnimatorController animator;

    [SerializeField] private PlayerStatus status;

    // Update is called once per frame
    void Update()
    {
        if (animator.MoveSpeed > 0.5f || status.CurrentStamina == 0f)
        {
            audioSource.Stop();
            return;
        }
        else
        { 
            if (Input.GetMouseButtonUp(0))
            {
                PlaySound(shootSound, shootMixerGroup);
            }

            if (Input.GetMouseButtonDown(0))
            {
                PlaySound(drawSound, drawMixerGroup);
            }
        }
    }

    private void PlaySound(AudioClip clip, AudioMixerGroup group)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.outputAudioMixerGroup = group;
        audioSource.Play();
    }
}
