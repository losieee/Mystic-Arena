using UnityEngine;

namespace FullOpaqueVFX
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Spell Data")]
    public class SpellData : ScriptableObject
    {
        public enum SpellTargetBehavior
        {
            SpawnOnCaster,
            SpawnOnTarget,
            FromCasterLookAtTarget
        }

        public string spellName;
        public GameObject incantationPrefab;
        public GameObject spellBurstPrefab;
        public GameObject mainSpellPrefab;

        public float castTime = 2f;
        public float cooldown = 3f;

        public KeyCode activationKey = KeyCode.E;

        public bool shakeEnabled = true;
        public float shakeStrengthIncantation = 0.05f;
        public float shakeStrengthBurst = 0.05f;
        public float shakeStrengthImpact = 0.05f;
        public float shakeDurationIncantation = 0.2f;
        public float shakeDurationBurst = 0.2f;
        public float shakeDurationImpact = 0.2f;

        public Color spellColor = Color.white;
        public SpellTargetBehavior spellTargetBehavior = SpellTargetBehavior.SpawnOnCaster;
        public bool spellTracking = false;
        public bool spawnOnGround = false;

        // Instancie le prefab et applique la couleur définie
        public GameObject SpawnEffect(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab != null)
            {
                GameObject obj = Instantiate(prefab, position, rotation);
                ApplyColorToParticles(obj);
                return obj;
            }
            return null;
        }

        // Applique la couleur du sort aux systèmes de particules de l'objet instancié
        private void ApplyColorToParticles(GameObject spellObject)
        {
            if (spellObject == null || spellColor == Color.white) return;
            ParticleSystem[] particleSystems = spellObject.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in particleSystems)
            {
                var mainModule = ps.main;
                mainModule.startColor = spellColor;
            }
        }
    }
}
