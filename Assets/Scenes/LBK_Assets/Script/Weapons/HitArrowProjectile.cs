using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

namespace GodOfArcher
{
    public class HitArrowProjectile : MonoBehaviour
    {
        // PRIVATE MEMBERS

        [SerializeField]
        private GameObject _hitEffect;
        [SerializeField]
        private GameObject _visualsRoot;

        private bool _hitEffectVisible;
        [Networked]
        public bool used { get; set; } = false;

        // PUBLIC METHODS

        public void ShowHitEffect()
        {
            Debug.Log("Show Hit Effect");
            if (_hitEffectVisible == true)
                return;

            if (_hitEffect != null)
            {
                _hitEffect.SetActive(true);
            }

            if (_visualsRoot != null)
            {
                _visualsRoot.SetActive(false);
            }

            _hitEffectVisible = true;
        }

        // MONOBEHAVIOUR

        protected void Awake()
        {
            if (_hitEffect != null)
            {
                _hitEffect.SetActive(false);
            }
        }
    }
}