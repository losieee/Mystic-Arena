using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FullOpaqueVFX
{
    public class ShieldCollision : MonoBehaviour
    {
        public GameObject impactVFXPrefab;
        public GameObject explosionVFXPrefab;
        public int maxHits = 5;

        private int currentHits = 0;
        private bool isExploding = false;
        private Collider shieldCollider;
        private ParticleSystem[] allParticles;

        private void Start()
        {
            shieldCollider = GetComponent<Collider>();
            allParticles = GetComponentsInChildren<ParticleSystem>();
        }

        private void OnParticleCollision(GameObject other)
        {
            if (isExploding) return;

            ParticleSystem ps = other.GetComponent<ParticleSystem>();
            if (ps == null) return;

            // Utilisation de List<ParticleCollisionEvent> au lieu de tableau
            List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
            int collisionCount = ps.GetCollisionEvents(gameObject, collisionEvents);

            for (int i = 0; i < collisionCount; i++)
            {
                Vector3 impactPoint = collisionEvents[i].intersection;
                Vector3 normal = (impactPoint - transform.position).normalized;

                RestartShieldParticles();
                SpawnImpactVFX(impactPoint, normal);
            }

            currentHits++;

            if (currentHits >= maxHits)
            {
                ExplodeShield();
            }
        }

        void RestartShieldParticles()
        {
            foreach (ParticleSystem ps in allParticles)
            {
                ps.Stop();
                ps.Play();
            }
        }

        void SpawnImpactVFX(Vector3 position, Vector3 normal)
        {
            if (impactVFXPrefab != null)
            {
                GameObject vfx = Instantiate(impactVFXPrefab, position, Quaternion.LookRotation(normal));
                Destroy(vfx, 2f);
            }
        }

        void ExplodeShield()
        {
            if (isExploding) return;

            isExploding = true;

            if (shieldCollider != null)
            {
                shieldCollider.enabled = false;
            }

            foreach (ParticleSystem ps in allParticles)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            if (explosionVFXPrefab != null)
            {
                GameObject explosionVFX = Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);
                StartCoroutine(DestroyAfterDelay(explosionVFX, 0.5f));
            }

            StartCoroutine(DestroyAfterDelay(gameObject, 0.5f));
        }

        IEnumerator DestroyAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(obj);
        }
    }
}
