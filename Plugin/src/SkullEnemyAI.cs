using UnityEngine;

namespace SkullEnemy
{
    public class SkullEnemyAI : EnemyAI
    {
        public float laughTimer;

        private float _movementSpeed;
        private float _rotationSpeed;
        private bool _canOnlyCollideWithTargetPlayer;

        public override void Start()
        {
            base.Start();

            _movementSpeed = SkullEnemyPlugin.Instance.ConfigMovementSpeed.Value;
            _rotationSpeed = SkullEnemyPlugin.Instance.ConfigRotationSpeed.Value;
            _canOnlyCollideWithTargetPlayer = SkullEnemyPlugin.Instance.ConfigCanOnlyCollideWithTargetPlayer.Value;
        }

        public override void OnCollideWithPlayer(Collider other)
        {
            base.OnCollideWithPlayer(other);

            if (isEnemyDead)
                return;

            var playerController = MeetsStandardPlayerCollisionConditions(other);
            if (playerController == null)
                return;

            if (_canOnlyCollideWithTargetPlayer && playerController != targetPlayer)
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
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
            transform.position += direction * (Time.deltaTime * _movementSpeed);
        }

        private bool HasValidTarget()
        {
            return targetPlayer != null && targetPlayer.isInsideFactory && !targetPlayer.isPlayerDead;
        }
    }
}