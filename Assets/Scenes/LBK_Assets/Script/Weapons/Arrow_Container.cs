using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.Serialization;
using System.IO.Pipes;
using Unity.VisualScripting;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System;

namespace GodOfArcher
{
    public class Arrow_Containr : NetworkBehaviour
    {
        [SerializeField]
        private HitArrowProjectile _HitArrowProjectilePrefab;

        [Networked, Capacity(64)]
        private NetworkArray<ProjectileData> _projectileData { get; }

        private HitArrowProjectile[] _projectiles = new HitArrowProjectile[64];

        [Networked]
        private int _hitArrowCount { get; set; } = 0;

        private SceneObjects _sceneObjects;

        public void Make_Arrow(Vector3 hitPosition, Vector3 fireDirection)
        {
            Debug.Log("hit_Arrow");
            _projectileData.Set(_hitArrowCount % _projectileData.Length, new ProjectileData()
            {
                IsActive = false,
                HitPosition = hitPosition,
                ArrowVelocity = fireDirection,
                ArrowRotation = Quaternion.LookRotation(fireDirection),
            });
            _hitArrowCount++;
            return;
        }

        public void Reset_Arrow_Container()
        {
            _hitArrowCount = 0;
            for (int i = 0; i < _projectiles.Length; i++)
            {
                var data = _projectiles[i];
                if (data != null)
                {
                    Destroy(data.gameObject);
                    _projectiles[i] = null;
                }
            }
            _projectileData.Clear();
        }

        public override void Spawned()
        {
            _hitArrowCount = 0;
            _sceneObjects = Runner.GetSingleton<SceneObjects>();

            for (int i = 0; i < _projectiles.Length; i++)
            {
                var data = _projectiles[i];
                if (data != null)
                {
                    Destroy(data.gameObject);
                    _projectiles[i] = null;
                }
            }
            _projectileData.Clear();

        }

        public override void Render()
        {
            // Instantiate missing projectile objects
            for (int i = 0; i < _hitArrowCount % _projectileData.Length; i++)
            {
                int index = i % _projectileData.Length;
                var data = _projectileData[index];

                var previousProjectile = _projectiles[index];

                if (previousProjectile != null)
                {
                    Destroy(previousProjectile.gameObject);
                }

                var projectile = Instantiate(_HitArrowProjectilePrefab, data.HitPosition, Quaternion.LookRotation(data.ArrowVelocity));
                projectile.gameObject.transform.parent = gameObject.transform;

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

                if (projectile.IsActive == true)
                {
                    if (projectileObject != null)
                    {
                        Destroy(projectileObject.gameObject);
                    }

                    continue;
                }

            }
        }

        public override void FixedUpdateNetwork()
        {
            // Process projectile update
            for (int i = 0; i < _projectileData.Length; i++)
            {
                var data = _projectileData[i];

                UpdateProjectile(ref data);

                _projectileData.Set(i, data);
            }
        }
        public float Radius = 2f;
        public LayerMask LayerMask;

        private static Collider[] _colliders = new Collider[8];

        private void UpdateProjectile(ref ProjectileData projectileData)
        {

            int collisions = Runner.GetPhysicsScene().OverlapSphere(projectileData.HitPosition + Vector3.back, Radius, _colliders, LayerMask, QueryTriggerInteraction.Ignore);
            if(collisions != 0) Debug.Log(collisions);
            for (int i = 0; i < collisions; i++)
            {
                var player = _colliders[i].GetComponentInParent<Player>();
                if (player != null && player.actState == playerActState.Interact)
                {
                    Debug.Log("Add Arrow");
                    if(player.Weapons.Add_Arrow()) projectileData.IsActive = true;
                    break;
                }
            }
        }


        // DATA STRUCTURES

        private struct ProjectileData : INetworkStruct
        {
            public bool IsActive;

            public Vector3 HitPosition { get; set; }
            public Vector3 ArrowVelocity;
            public Quaternion ArrowRotation;
        }
    }
}
