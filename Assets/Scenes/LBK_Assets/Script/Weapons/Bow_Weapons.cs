using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.Serialization;
using System.IO.Pipes;

namespace GodOfArcher
{
    public class Bow_Weapons : WeaponBase
    {
        // PRIVATE MEMBERS

        [SerializeField]
        private float _speed = 50f;
        [SerializeField]
        private LayerMask _hitMask;
        [SerializeField]
        private float _hitImpulse = 50f;
        [SerializeField]
        private float _lifeTime = 4f;
        [SerializeField]
        private float _lifeTimeAfterHit = 2f;
        [SerializeField]
        private DummyProjectile _dummyProjectilePrefab;
        [SerializeField] 
        private int pullSpeed = 100;

        public SkinnedMeshRenderer bowSkin;
        public SkinnedMeshRenderer arrowSkin;

        [FormerlySerializedAs("WeaponVisual")]
        public GameObject FirstPersonVisual;
        public GameObject ThirdPersonVisual;

        [Header("Sounds")]
        public AudioSource DrawSound;
        public AudioSource ShootSound;

        [Header("Ammo")]
        public GameObject Arrow;
        public int MaxClipAmmo = 10;
        public int StartAmmo = 10;

        [Header("Fire Setup")]
        public float Damage = 50f;
        public float gravity = 0.05f;

        [Networked]
        public int ClipAmmo { get; set; }

        [Networked]
        public float drawDistance { get; set; } = 0;

        [Networked]
        private int _fireCount { get; set; }
        [Networked, Capacity(64)]
        private NetworkArray<ProjectileData> _projectileData { get; }

        private DummyProjectile[] _projectiles = new DummyProjectile[64];



        private int _visibleFireCount;
        private SceneObjects _sceneObjects;

        public bool Drawing = false;
        // WeaponBase INTERFACE

        public void Draw(Vector3 firePosition, Vector3 fireDirection, bool justPressed)
        {
            if (justPressed == true && Drawing == false)
            {
                if(ClipAmmo > 0) { Arrow.SetActive(true); }
                Debug.Log("Pressed");
                Drawing = true;
                ShootSound.Stop();
                //DrawSound.Stop();
                DrawSound.Play();
            }
        }

        private Vector3 _FireRayPosition = Vector3.zero;
        private Vector3 _FireRayEndPosition = Vector3.zero;
        public void Update() 
        {
            Debug.DrawRay(_FireRayPosition, _FireRayPosition + (_FireRayEndPosition * _speed), Color.red);
        }
        public void Shoot(Vector3 firePosition, Vector3 fireDirection, bool justReleased)
        {

            if (justReleased == true && Drawing == true)
            {
                Debug.Log("Released");
                Drawing = false;
                DrawSound.Stop();
                ShootSound.Play();
                if (ClipAmmo > 0 && drawDistance >= 10)
                {
                    _FireRayPosition = firePosition;
                    _FireRayEndPosition = fireDirection;
                    _projectileData.Set(_fireCount % _projectileData.Length, new ProjectileData()
                    {
                        IsActive = true,
                        FireTick = Runner.Tick,
                        FirePosition = firePosition,
                        FireVelocity = fireDirection * drawDistance,
                        FireRotation = Quaternion.LookRotation(fireDirection),
                        UpVelocity = (new Vector3(0, fireDirection.y, 0)) * drawDistance,
                        FinishTick = Runner.Tick + Mathf.RoundToInt(_lifeTime / Runner.DeltaTime),
                    }); ;

                    drawDistance = 0;
                    _fireCount++;
                    ClipAmmo--;
                    Arrow.SetActive(false);
                }
                return;
            }
        }

        public bool Add_Arrow_one()
        {
            if(ClipAmmo >= 10) return false;
            ClipAmmo++;
            if (ClipAmmo > 10) ClipAmmo = 10;
            return true;
        }
        public void WeaponeStop()
        {
            Drawing = false;
            drawDistance = 0;
            ShootSound.Stop();
            DrawSound.Stop();
        }

        public override void Spawned()
        {
            _visibleFireCount = _fireCount;
            _sceneObjects = Runner.GetSingleton<SceneObjects>();
            Arrow.SetActive(false);
            ClipAmmo = StartAmmo;

            for (int i = 0; i < _projectiles.Length; i++)
            {
                var data = _projectiles[i];
                if(data != null) Destroy(data.gameObject);
            }
            _projectileData.Clear();

        }

        public override void FixedUpdateNetwork()
        {
            int tick = Runner.Tick;

            if(Drawing) {
                drawDistance += Runner.DeltaTime * pullSpeed;
                if (drawDistance > 100) drawDistance = 100;
                Debug.Log($"{drawDistance}");
            }

            // Process projectile update
            for (int i = 0; i < _projectileData.Length; i++)
            {
                var data = _projectileData[i];

                if (data.IsActive == false)
                    continue;
                if (data.FinishTick <= tick)
                    continue;

                // For simplicity projectile update is processed directly in the Weapon.
                // It might be more suitable to move this logic to projectile object itself in order
                // to have different projectile behaviours without the need to alter the weapon.
                // See Projectiles Advanced where such approach is used.
                data.FireVelocity = new Vector3(data.FireVelocity.x, (data.FireVelocity.y - (data.UpVelocity.magnitude * gravity) * Runner.DeltaTime), data.FireVelocity.z);
                UpdateProjectile(ref data, tick);

                _projectileData.Set(i, data);
            }
        }

        public override void Render()
        {
            if (_visibleFireCount < _fireCount)
            {
                PlayFireEffect();
            }

            bowSkin.SetBlendShapeWeight(0, drawDistance);
            arrowSkin.SetBlendShapeWeight(0, drawDistance);

            // Instantiate missing projectile objects
            for (int i = _visibleFireCount; i < _fireCount; i++)
            {
                int index = i % _projectileData.Length;
                var data = _projectileData[index];

                var previousProjectile = _projectiles[index];
                if (previousProjectile != null)
                {
                    Destroy(previousProjectile.gameObject);
                }

                var projectile = Instantiate(_dummyProjectilePrefab, data.FirePosition, Quaternion.LookRotation(data.FireVelocity));

                // When using multipeer, move to correct scene and disable renderers for other clients. Can be omitted otherwise.
                if (Runner.Config.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
                {
                    Runner.MoveToRunnerScene(projectile);
                    Runner.AddVisibilityNodes(projectile.gameObject);
                }

                _projectiles[index] = projectile;
            }

            // For proxies we move projectiles in remote time frame, for input/state authority we use local time frame
            float renderTime = Object.IsProxy == true ? Runner.RemoteRenderTime : Runner.LocalRenderTime;
            float floatTick = renderTime / Runner.DeltaTime;

            // Update projectile visuals
            for (int i = 0; i < _projectiles.Length; i++)
            {
                var projectile = _projectileData[i];
                var projectileObject = _projectiles[i];

                if (projectile.IsActive == false)
                {
                    if (projectileObject != null)
                    {
                        Destroy(projectileObject.gameObject);
                    }

                    continue;
                }

                if (projectile.HitPosition != Vector3.zero)
                {
                    projectileObject.transform.position = projectile.HitPosition;
                    projectileObject.ShowHitEffect();
                }
                else
                {
                    projectileObject.transform.position = GetMovePosition(ref projectile, floatTick);
                    projectileObject.transform.rotation = Quaternion.LookRotation(GetMovePosition(ref projectile, floatTick-1f) - GetMovePosition(ref projectile, floatTick));
                }
            }

            _visibleFireCount = _fireCount;
        }

        // PRIVATE METHODS

        private void UpdateProjectile(ref ProjectileData projectileData, int tick)
        {
            if (projectileData.HitPosition != Vector3.zero)
                return;

            var previousPosition = GetMovePosition(ref projectileData, tick - 1f);
            var nextPosition = GetMovePosition(ref projectileData, tick);

            var direction = nextPosition - previousPosition;

            float distance = direction.magnitude;
            direction /= distance; // Normalize

            var hitOptions = HitOptions.IncludePhysX | HitOptions.IgnoreInputAuthority;

            if (Runner.LagCompensation.Raycast(previousPosition, direction, distance,
                    Object.InputAuthority, out var hit, _hitMask, hitOptions) == true)
            {
                projectileData.HitPosition = hit.Point;
                projectileData.FinishTick = tick + Mathf.RoundToInt(_lifeTimeAfterHit / Runner.DeltaTime);

                if (hit.Collider != null)
                {
                    projectileData.IsActive = false;
                    Debug.Log("HitGround!");
                    _sceneObjects.Gameplay._ArrowContainer.Make_Arrow(projectileData.HitPosition, projectileData.FireVelocity);
                }

                if (hit.Hitbox != null)
                {
                    projectileData.IsActive = false;
                    Debug.Log("Hit!");
                    ApplyDamage(hit.Hitbox, hit.Point, direction);
                }
            }
        }

        private Vector3 GetMovePosition(ref ProjectileData data, float currentTick)
        {
            float time = (currentTick - data.FireTick) * Runner.DeltaTime;

            if (time <= 0f)
                return data.FirePosition;
            
            return data.FirePosition + data.FireVelocity * time;
        }

        private void ApplyDamage(Hitbox enemyHitbox, Vector3 position, Vector3 direction)
        {
            var enemyHealth = enemyHitbox.Root.GetComponent<Health>();
            if (enemyHealth == null || enemyHealth.IsAlive == false)
                return;

            float damageMultiplier = enemyHitbox is BodyHitbox bodyHitbox ? bodyHitbox.DamageMultiplier : 1f;
            bool isCriticalHit = damageMultiplier > 1f;

            float damage = Damage * damageMultiplier;
            if (_sceneObjects.Gameplay.DoubleDamageActive)
            {
                damage *= 2f;
            }

            if (enemyHealth.ApplyDamage(Object.InputAuthority, damage, position, direction, isCriticalHit) == false)
                return;

            if (HasInputAuthority && Runner.IsForward)
            {
                // For local player show UI hit effect.
                //_sceneObjects.GameUI.PlayerView.Crosshair.ShowHit(enemyHealth.IsAlive == false, isCriticalHit);
            }
        }

        public void ToggleVisibility(bool isVisible)
        {
            FirstPersonVisual.SetActive(isVisible);
            ThirdPersonVisual.SetActive(isVisible);

            /*if (_muzzleEffectInstance != null)
            {
                _muzzleEffectInstance.SetActive(false);
            }*/
        }

        // DATA STRUCTURES
        
        private struct ProjectileData : INetworkStruct
        {
            public bool IsActive;

            public int FireTick;
            public int FinishTick;

            public Vector3 FirePosition;
            public Vector3 FireVelocity;
            public Vector3 UpVelocity;
            public Quaternion FireRotation;

            public Vector3 HitPosition { get; set; }
        }
    }
}
