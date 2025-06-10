using UnityEngine;

namespace GodOfArcher
{
    public class DummyProjectile : MonoBehaviour
    {
        // PRIVATE MEMBERS

        [SerializeField]
        private GameObject _hitEffect;
        [SerializeField]
        private GameObject _visualsRoot;

        private bool _hitEffectVisible;

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
