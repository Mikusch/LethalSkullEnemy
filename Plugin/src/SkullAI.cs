using BepInEx.Configuration;
using UnityEngine;

namespace Skull
{
    public class SkullAI : EnemyAI
    {
        public float laughTimer;
        public float movementSpeed;
        public float rotationSpeed;

        public override void Start()
        {
            base.Start();

            movementSpeed =
                SkullPlugin.ConfigFile.TryGetEntry("Movement", "MovementSpeed",
                    out ConfigEntry<float> movementSpeedEntry)
                    ? movementSpeedEntry.Value
                    : 2f;
            rotationSpeed =
                SkullPlugin.ConfigFile.TryGetEntry("Movement", "RotationSpeed",
                    out ConfigEntry<float> rotationSpeedEntry)
                    ? rotationSpeedEntry.Value
                    : 2f;
        }

        public override void OnCollideWithPlayer(Collider other)
        {
            base.OnCollideWithPlayer(other);

            if (isEnemyDead)
                return;

            var playerController = MeetsStandardPlayerCollisionConditions(other);
            if (playerController == null)
                return;

            playerController.KillPlayer(Vector3.zero, deathAnimation: 1);
        }

        public override void Update()
        {
            base.Update();

            laughTimer -= Time.deltaTime;
            if (laughTimer <= 0.0)
            {
                laughTimer = Random.Range(6f, 7f);

                if (enemyType.audioClips.Length > 0)
                    creatureVoice.PlayOneShot(enemyType.audioClips[Random.Range(0, enemyType.audioClips.Length)]);
            }

            if (!HasValidTarget())
            {
                targetPlayer = GetClosestPlayer();

                if (debugEnemyAI && targetPlayer != null)
                    Debug.Log($"Found new target: {targetPlayer.playerUsername}");
            }

            if (!HasValidTarget())
            {
                if (debugEnemyAI)
                    Debug.Log("Failed to find new target");

                return;
            }

            var direction = (targetPlayer.playerGlobalHead.position - transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(direction);
            transform.rotation =
                Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            transform.position += direction * (Time.deltaTime * movementSpeed);
        }

        private bool HasValidTarget()
        {
            return targetPlayer != null && targetPlayer.isInsideFactory && !targetPlayer.isPlayerDead;
        }
    }
}