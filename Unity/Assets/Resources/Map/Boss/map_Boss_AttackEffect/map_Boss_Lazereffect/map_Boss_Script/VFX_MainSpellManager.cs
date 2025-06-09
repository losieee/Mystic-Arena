using UnityEngine;
using System.Collections;

namespace FullOpaqueVFX
{
    public class MainSpellManager : MonoBehaviour
    {
        private CameraShake cameraShake;
        private float impactShakeStrength = 0f;
        private float impactShakeDuration = 0.2f;
        private Transform target;
        private bool isTracking = false;

        public Transform forceFieldTarget;

        void Start()
        {
            cameraShake = FindObjectOfType<CameraShake>();

            if (forceFieldTarget == null)
            {
                Transform child = transform.Find("ForceFieldTarget");
                if (child != null)
                {
                    forceFieldTarget = child;
                }
            }
        }

        void Update()
        {
            if (isTracking && target != null && forceFieldTarget != null)
            {
                forceFieldTarget.position = target.position;
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void EnableTracking()
        {
            isTracking = true;
        }

        public void OnParticleCollision(GameObject other)
        {
            if (cameraShake != null)
            {
                cameraShake.Shake(impactShakeStrength, impactShakeDuration);
            }
        }

        public void SetImpactShakeStrength(float strength)
        {
            impactShakeStrength = strength;
        }

        public void SetImpactShakeDuration(float duration)
        {
            impactShakeDuration = duration;
        }

        private IEnumerator CheckAndDestroy()
        {
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            yield return new WaitUntil(() => System.Array.TrueForAll(particleSystems, ps => ps == null || !ps.IsAlive(true)));
            Destroy(gameObject);
        }

        void OnEnable()
        {
            StartCoroutine(CheckAndDestroy());
        }
    }
}
