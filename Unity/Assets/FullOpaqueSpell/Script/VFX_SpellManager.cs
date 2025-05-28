using UnityEngine;
using System.Collections;

namespace FullOpaqueVFX
{
    public class VFX_SpellManager : MonoBehaviour
    {
        public SpellData currentSpell;
        public Transform target;
        private bool isOnCooldown = false;
        private CameraShake cameraShake;

        void Start()
        {
            if (!Application.isPlaying) return;
            cameraShake = FindObjectOfType<CameraShake>();
        }

        void Update()
        {
            if (!Application.isPlaying) return;

            if (currentSpell != null && Input.GetKeyDown(currentSpell.activationKey) && !isOnCooldown)
            {
                StartCoroutine(CastSpell());
            }
        }

        private IEnumerator CastSpell()
        {
            if (!Application.isPlaying || target == null) yield break;

            isOnCooldown = true;

            // 1️⃣ Incantation
            GameObject incantation = currentSpell.SpawnEffect(currentSpell.incantationPrefab, transform.position, Quaternion.identity);
            if (incantation != null)
            {
                AdjustParticleLifetime(incantation, currentSpell.castTime);
                incantation.SetActive(true);
                PlayParticleSystem(incantation);
                if (currentSpell.shakeEnabled && cameraShake != null)
                {
                    cameraShake.Shake(currentSpell.shakeStrengthIncantation, currentSpell.shakeDurationIncantation);
                }
            }

            yield return new WaitForSeconds(currentSpell.castTime);

            if (incantation != null)
                Destroy(incantation);

            // 2️⃣ Détermination de la position et de la rotation du Main Spell
            Vector3 spawnPosition = transform.position;
            Quaternion spawnRotation = Quaternion.identity;

            if (currentSpell.spellTargetBehavior == SpellData.SpellTargetBehavior.SpawnOnTarget && target != null)
            {
                spawnPosition = target.position;
            }
            else if (currentSpell.spellTargetBehavior == SpellData.SpellTargetBehavior.FromCasterLookAtTarget)
            {
                // On reste sur la position du caster et on oriente le sort vers la target (si assignée)
                spawnPosition = transform.position;
                if (target != null)
                {
                    spawnRotation = Quaternion.LookRotation(target.position - transform.position);
                }
            }
            // Pour SpawnOnCaster, spawnPosition et spawnRotation restent ceux par défaut (position du caster, rotation identité)

            // Si "SpawnOnGround" est activé, on effectue un raycast depuis la position de départ définie selon le comportement
            if (currentSpell.spawnOnGround)
            {
                // Pour SpawnOnCaster, le raycast part de transform.position,
                // sinon s'il y a une target, on part de target.position.
                Vector3 origin = (currentSpell.spellTargetBehavior == SpellData.SpellTargetBehavior.SpawnOnCaster) ? transform.position : target.position;
                spawnPosition = GetGroundPosition(origin);
            }

            // 2️⃣.1 Lancer le Main Spell
            GameObject mainSpell = currentSpell.SpawnEffect(currentSpell.mainSpellPrefab, spawnPosition, spawnRotation);
            if (mainSpell != null)
            {
                mainSpell.SetActive(true);
                MainSpellManager mainSpellManager = mainSpell.GetComponent<MainSpellManager>();
                if (mainSpellManager != null)
                {
                    mainSpellManager.SetImpactShakeStrength(currentSpell.shakeStrengthImpact);
                    mainSpellManager.SetImpactShakeDuration(currentSpell.shakeDurationImpact);
                    mainSpellManager.SetTarget(target);

                    if (currentSpell.spellTracking)
                        mainSpellManager.EnableTracking();
                }
                PlayParticleSystem(mainSpell);
            }

            // 3️⃣ Gestion du Spell Burst
            if (currentSpell.spellBurstPrefab != null)
            {
                Vector3 burstPosition = transform.position;
                GameObject spellBurst = currentSpell.SpawnEffect(currentSpell.spellBurstPrefab, burstPosition, Quaternion.identity);
                if (spellBurst != null)
                {
                    spellBurst.SetActive(true);
                    PlayParticleSystem(spellBurst);
                    if (currentSpell.shakeEnabled && cameraShake != null)
                    {
                        cameraShake.Shake(currentSpell.shakeStrengthBurst, currentSpell.shakeDurationBurst);
                    }
                    StartCoroutine(DestroyAfterParticles(spellBurst));
                }
            }

            yield return new WaitForSeconds(currentSpell.cooldown);
            isOnCooldown = false;
        }

        // Effectue un raycast depuis une position donnée vers le bas en ne détectant que le layer "Terrain"
        private Vector3 GetGroundPosition(Vector3 originPos)
        {
            RaycastHit hit;
            Vector3 rayStart = originPos + Vector3.up * 10f;
            int terrainLayer = LayerMask.NameToLayer("Terrain");
            int terrainLayerMask = 1 << terrainLayer;
            if (Physics.Raycast(rayStart, Vector3.down, out hit, Mathf.Infinity, terrainLayerMask))
            {
                return hit.point;
            }
            return originPos;
        }

        private void AdjustParticleLifetime(GameObject spellObject, float lifetime)
        {
            if (spellObject == null) return;
            ParticleSystem[] particleSystems = spellObject.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in particleSystems)
            {
                var mainModule = ps.main;
                if (!mainModule.loop)
                {
                    mainModule.startLifetime = lifetime;
                }
            }
        }

        private void PlayParticleSystem(GameObject obj)
        {
            if (obj == null) return;
            ParticleSystem[] psArray = obj.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in psArray)
            {
                ps.Play();
            }
        }

        private IEnumerator DestroyAfterParticles(GameObject obj)
        {
            if (obj == null) yield break;
            ParticleSystem[] psArray = obj.GetComponentsInChildren<ParticleSystem>();
            yield return new WaitUntil(() => System.Array.TrueForAll(psArray, ps => ps == null || !ps.IsAlive(true)));
            Destroy(obj);
        }
    }
}
