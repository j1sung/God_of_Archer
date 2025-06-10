using Fusion;
using UnityEngine;

namespace GodOfArcher
{
    public enum BeaconState { Active, Inactive, Ignited, Extinguished, Igniting, Extinguishing }
    /// <summary>
    /// Periodically checks for an object with Health component within radius and refills health.
    /// </summary>
    public class Signal_Fire : NetworkBehaviour
	{
		public float      Radius = 4f;
		public float      Cooldown = 30f;
		public LayerMask  LayerMask;
		public GameObject FireObject;
		public Gameplay gameplay;

        [Networked]
        public BeaconState State { get; private set; } = BeaconState.Active;

        public bool CanIgnite => State == BeaconState.Igniting;
        public bool CanExtinguish => State == BeaconState.Extinguishing;

        public string beaconName;

        [Networked]
        public bool IsActive { get; set; } = false;

        public Player IgnitePlayer { get; set; }
        public Player ExtinguishPlayer { get; set; }

        [Networked]
        private float timer { get; set; } = 0.0f;

        private static Collider[] _colliders = new Collider[8];

        public override void FixedUpdateNetwork()
        {
            if (IsActive == false)
                return;

            int collisions = Runner.GetPhysicsScene().OverlapSphere(gameObject.transform.position, Radius, _colliders, LayerMask, QueryTriggerInteraction.Ignore);
            if (collisions != 0) Debug.Log(collisions);
            for (int i = 0; i < collisions; i++)
            {
                var player = _colliders[i].GetComponentInParent<Player>();
                if (player != null && player.actState == playerActState.Interact)
                {
                    if ((player.team == Team.Chung) && ((State == BeaconState.Active) || (State == BeaconState.Igniting)))
                    {
                        if (IgnitePlayer == null)
                        {
                            IgnitePlayer = player;
                            State = BeaconState.Igniting;
                            timer = 0;
                        }
                    }

                    if ((player.team == Team.Chung) && (State == BeaconState.Igniting))
                    {
                        if (IgnitePlayer == player)
                        {
                            Debug.Log("On Firing" + timer);
                            timer += Runner.DeltaTime;
                            if (timer > 3.0f)
                            {
                                Ignite();
                            }
                        }
                    }

                    if ((player.team == Team.Josen) && (State == BeaconState.Ignited))
                    {
                        if (ExtinguishPlayer == null)
                        {
                            ExtinguishPlayer = player;
                            State = BeaconState.Extinguishing;
                            timer = 0;
                        }
                    }

                    if ((player.team == Team.Josen) && (State == BeaconState.Extinguishing))
                    {
                        if (ExtinguishPlayer == player)
                        {
                            timer += Runner.DeltaTime;
                            if (timer > 7.0f)
                            {
                                Extinguish();
                            }
                        }
                    }
                }
                else if(player != null && player.actState != playerActState.Interact)
                {
                    if ((player.team == Team.Chung) && (State == BeaconState.Igniting))
                    {
                        if (IgnitePlayer == player)
                        {
                            IgnitePlayer = null;
                            State = BeaconState.Active;
                            timer = 0;
                            break;
                        }
                    }

                    if ((player.team == Team.Josen) && (State == BeaconState.Extinguishing))
                    {
                        if (ExtinguishPlayer == player)
                        {
                            ExtinguishPlayer = null;
                            State = BeaconState.Ignited;
                            timer = 0;
                            break;
                        }
                    }
                }
            }
        }

        // ¡°»≠
        public void Ignite()
        {
            if (!CanIgnite) return;
            State = BeaconState.Ignited;
			IsActive = true;
			gameplay.SetRemainingTime(40.0f, beaconName);
            Debug.Log($"{beaconName} ∫¿»≠ ¡°»≠µ (Ignited)!");
        }

        // º“»≠
        public void Extinguish()
        {
            if (!CanExtinguish) return;
            State = BeaconState.Extinguished;
            IsActive = false;
			gameplay.Fire_Extinguished();
            Debug.Log($"{beaconName} ∫¿»≠ º“»≠µ (Extinguished)!");
        }

        // ¥Ÿ∏• ∫¿»≠∞° ¡°»≠µ∆¿ª ∂ß ∫Ò»∞º∫»≠
        public void SetInactive()
        {
            State = BeaconState.Inactive;
			gameObject.SetActive( false );
            Debug.Log($"{beaconName} ∫¿»≠ ∫Ò»∞º∫»≠(Inactive)");
        }

		public void SetActive()
		{
			State = BeaconState.Active;
            IsActive = false;
            gameObject.SetActive(true);
		}

        public override void Render()
		{
			FireObject.SetActive(IsActive);
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawWireSphere(transform.position, Radius);
		}
	}
}
