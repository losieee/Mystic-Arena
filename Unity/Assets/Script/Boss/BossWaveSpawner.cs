using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossWaveSpawner : MonoBehaviour
{
    private bool effectPlayed = false;
    bool meteorSpawnStarted = false;
    public GameObject attackBehaviorPrefab;
    public GameObject warningAreaPrefab;
    public WarningWave[] waveSequence;

    public enum AttackType
    {
        Bottom,
        Laser,
        Meteor
    }

    [System.Serializable]
    public class WarningConfig
    {
        public AttackType attackType = AttackType.Bottom;

        // ����
        public Vector3 position;
        public Vector3 scale = new Vector3(3, 1, 3);
        public Vector3 rotationEuler;
        public float warningDuration = 2f;
        public FillPatternType fillPattern = FillPatternType.FromEdge;

        // Bottom ����
        public GameObject spawnAfterEffect;
        public GameObject attackEffectPrefab;
        public AudioClip bottomSound;

        // Lazer ����
        public Vector3 lazerReadyEffectPosition;
        public Vector3 lazerReadyEffectRotation;
        public GameObject lazerReadyEffectPrefab;
        public GameObject lazerBeamPrefab;
        public AudioClip lazerReadySound;
        public AudioClip lazerSound;
        public Vector3 lazerBeamSpawnPosition;
        public Vector3 lazerBeamScale;
        public Vector3 lazerBeamRotation;

        // Meteor ����
        public GameObject meteorPrefab;
        public GameObject meteorEffectPrefab;
        public AudioClip meteorSound;
        public Vector3 meteorSpawnPosition;
        public Vector3 meteorScale;
        public Vector3 meteorEffectScale;
    }

    [System.Serializable]
    public class WarningWave
    {
        public WarningConfig[] warnings;
        public float waitAfterWave = 1f;
        public Vector3 effectSpawnPosition = Vector3.zero;
    }

    private void Start()
    {
        StartCoroutine(SpawnWaveSequence());
    }

    private IEnumerator SpawnWaveSequence()
    {
        foreach (WarningWave wave in waveSequence)
        {
            effectPlayed = false;

            List<WarningConfig> meteorConfigs = new List<WarningConfig>();

            foreach (WarningConfig config in wave.warnings)
            {
                GameObject instance = Instantiate(
                    warningAreaPrefab,
                    config.position,
                    Quaternion.Euler(config.rotationEuler)
                );
                instance.transform.localScale = config.scale;

                if (config.attackType == AttackType.Laser)
                {
                    GameObject rotatingTarget = GameObject.Find("Boss");
                    if (rotatingTarget != null)
                    {
                        float rotateDuration = 1.5f;         // ȸ�� �ð�
                        float effectDuration = 2.2f;         // ����Ʈ �������� �ð�
                        float totalWarningTime = rotateDuration + effectDuration;

                        Quaternion from = rotatingTarget.transform.rotation;
                        Quaternion to = Quaternion.Euler(from.eulerAngles + new Vector3(0, 90f, 0));
                        StartCoroutine(RotateThenSpawnReadyEffect(rotatingTarget.transform, from, to, rotateDuration, effectDuration, config));
                    }
                }
                if (config.attackType == AttackType.Meteor && !meteorSpawnStarted)
                {
                    meteorSpawnStarted = true;
                    StartCoroutine(SpawnMeteorConfigsSequentially(wave.warnings, 0.5f));
                }

                FillController controller = instance.GetComponentInChildren<FillController>();
                if (controller != null)
                {
                    controller.fillDuration = config.warningDuration;
                    controller.fillPattern = config.fillPattern;

                    controller.onFillComplete.AddListener(() =>
                    {
                        // ���� ȿ�� �� ���� ����
                        if (!effectPlayed && attackBehaviorPrefab != null)
                        {
                            Vector3 pos = wave.effectSpawnPosition + Vector3.up * 0.5f;
                            GameObject fx = Instantiate(attackBehaviorPrefab, pos, Quaternion.identity);
                            Destroy(fx, 1f);
                            effectPlayed = true;
                        }

                        // Ÿ�Ժ� ����Ʈ ó��
                        switch (config.attackType)
                        {
                            case AttackType.Bottom:
                                if (config.attackEffectPrefab != null)
                                {
                                    Vector3 spawnPos = config.position + Vector3.down * 0.5f;
                                    GameObject attackFx = Instantiate(config.attackEffectPrefab, spawnPos, Quaternion.Euler(config.rotationEuler));
                                    Vector3 originalScale = attackFx.transform.localScale;
                                    attackFx.transform.localScale = new Vector3(config.scale.x / 1.5f, originalScale.y, config.scale.z / 1.5f);
                                    Destroy(attackFx, 1f);
                                }

                                if (config.bottomSound != null)
                                {
                                    AudioSource.PlayClipAtPoint(config.bottomSound, config.position);
                                }

                                if (config.spawnAfterEffect != null)
                                {
                                    GameObject spawned = Instantiate(config.spawnAfterEffect, config.position, Quaternion.Euler(config.rotationEuler));
                                    Transform meshChild = spawned.transform.Find("Tile");
                                    Transform meshBackgound = spawned.transform.Find("Background");
                                    Vector3 adjustedScale = new Vector3(config.scale.x, config.scale.z, 1);
                                    if (meshChild != null && meshBackgound != null)
                                    {
                                        meshChild.localScale = adjustedScale;
                                        meshBackgound.localScale = adjustedScale;
                                    }
                                    else
                                    {
                                        spawned.transform.localScale = adjustedScale;
                                    }
                                    StartCoroutine(FadeOutAndDestroy(spawned, 1f));
                                }
                                break;

                            case AttackType.Laser:
                                if (config.lazerSound != null)
                                {
                                    AudioSource.PlayClipAtPoint(config.lazerSound, config.lazerReadyEffectPosition);
                                }

                                if (config.lazerBeamPrefab != null)
                                {
                                    GameObject rotationTarget = GameObject.Find("Boss");
                                    if (rotationTarget != null)
                                    {
                                        GameObject beam = Instantiate(config.lazerBeamPrefab);

                                        // Boss�� �ڽ����� ���̱�
                                        beam.transform.SetParent(rotationTarget.transform);

                                        // ���ϴ� ��� ��ġ/ȸ��/������ ���� (local ����)
                                        beam.transform.localPosition = config.lazerBeamSpawnPosition;
                                        beam.transform.localRotation = Quaternion.Euler(config.lazerBeamRotation);
                                        beam.transform.localScale = config.lazerBeamScale;

                                        Destroy(beam, 4f);
                                    }
                                }

                                GameObject rotatingTarget = GameObject.Find("Boss");        // ȸ�� �� ���� ���
                                if (rotatingTarget != null)
                                {
                                    StartCoroutine(RotateAfterWarning(
                                        rotatingTarget.transform,
                                        4f,           // 1�ܰ�: -180�� ȸ�� �ð�
                                        -180f,        // 1�ܰ�: Y�� -180��
                                        1f,           // 2�ܰ�: ���� ȸ�� �ð�
                                        90f,          // 2�ܰ�: �ٽ� +90�� ȸ��
                                        config.lazerSound,
                                        config.lazerReadyEffectPosition
                                    ));
                                }
                                break;

                            case AttackType.Meteor:
                                // ���׿� ����
                                break;
                        }
                    });

                    controller.enabled = true;
                }
            }

            StartCoroutine(SpawnMeteorConfigsSequentially(meteorConfigs.ToArray(), 0.5f));

            float maxDuration = 0f;
            foreach (var config in wave.warnings)
                maxDuration = Mathf.Max(maxDuration, config.warningDuration);

            yield return new WaitForSeconds(maxDuration + wave.waitAfterWave);
        }
    }

    private IEnumerator RotateThenSpawnReadyEffect(Transform target, Quaternion from, Quaternion to, float rotateDuration, float effectDuration, WarningConfig config)
    {
        float time = 0f;
        while (time < rotateDuration)
        {
            target.localRotation = Quaternion.Lerp(from, to, time / rotateDuration);
            time += Time.deltaTime;
            yield return null;
        }
        target.localRotation = to;

        // ȸ�� �Ϸ� �� Ready ����Ʈ ����
        if (config.lazerReadyEffectPrefab != null)
        {
            GameObject readyFx = Instantiate(
                config.lazerReadyEffectPrefab,
                config.lazerReadyEffectPosition,
                Quaternion.Euler(config.lazerReadyEffectRotation)
            );

            if (config.lazerReadySound != null)
            {
                AudioSource.PlayClipAtPoint(config.lazerReadySound, config.lazerReadyEffectPosition);
            }

            readyFx.name = "LazerReadyEffectInstance";

            Destroy(readyFx, effectDuration);
        }
    }
    private IEnumerator RotateAfterWarning(Transform target, float firstDuration, float firstYRotation, float secondDuration, float secondYRotation, AudioClip sound, Vector3 soundPosition)
    {
        Quaternion from = target.rotation;
        Quaternion to = Quaternion.Euler(from.eulerAngles + new Vector3(0, firstYRotation, 0));

        // 1�ܰ� ȸ�� ����
        if (sound != null)
        {
            AudioSource.PlayClipAtPoint(sound, soundPosition);
        }

        float time = 0f;
        while (time < firstDuration)
        {
            target.rotation = Quaternion.Lerp(from, to, time / firstDuration);
            time += Time.deltaTime;
            yield return null;
        }
        target.rotation = to;

        // ��� ���
        if (sound != null)
        {
            yield return new WaitForSeconds(0.3f);
        }

        // 2�ܰ�: �ٽ� +90�� ȸ��
        Quaternion returnFrom = target.rotation;
        Quaternion returnTo = Quaternion.Euler(returnFrom.eulerAngles + new Vector3(0, secondYRotation, 0));

        float time2 = 0f;
        while (time2 < secondDuration)
        {
            target.rotation = Quaternion.Lerp(returnFrom, returnTo, time2 / secondDuration);
            time2 += Time.deltaTime;
            yield return null;
        }
        target.rotation = returnTo;
    }

    private IEnumerator SpawnMeteorConfigsSequentially(WarningConfig[] configs, float interval)
    {
        foreach (var config in configs)
        {
            if (config.attackType == AttackType.Meteor)
            {
                Vector3 spawnPos = config.position + Vector3.up * 15f;

                GameObject meteor = Instantiate(config.meteorPrefab, spawnPos, Quaternion.identity);
                meteor.transform.localScale = config.meteorScale;

                Rigidbody rb = meteor.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = true;
                    rb.AddForce(Vector3.down * 5000f);
                }

                StartCoroutine(PlayMeteorEffectAfterDelay(config, config.position));
                Destroy(meteor, 0.5f);

                yield return new WaitForSeconds(interval);
            }
        }
    }
    private IEnumerator PlayMeteorEffectAfterDelay(WarningConfig config, Vector3 position)
    {
        yield return new WaitForSeconds(0.1f);

        // ����
        if (config.meteorSound != null)
        {
            AudioSource.PlayClipAtPoint(config.meteorSound, position);
        }

        // ����Ʈ
        if (config.meteorEffectPrefab != null)
        {
            GameObject effect = Instantiate(config.meteorEffectPrefab, position, Quaternion.identity);
            effect.transform.localScale = config.meteorEffectScale;
            Destroy(effect, 0.7f); // ����Ʈ�� ���� �ð� �� �ı�
        }
    }

    private IEnumerator FadeOutAndDestroy(GameObject obj, float duration)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Destroy(obj);
            yield break;
        }

        List<Material> materials = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            materials.Add(renderer.material);
        }

        float time = 0f;
        float startAlpha = materials[0].color.a;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            foreach (Material mat in materials)
            {
                Color c = mat.color;
                mat.color = new Color(c.r, c.g, c.b, alpha);
            }

            time += Time.deltaTime;
            yield return null;
        }

        foreach (Material mat in materials)
        {
            Color c = mat.color;
            mat.color = new Color(c.r, c.g, c.b, 0f);
        }

        Destroy(obj);
    }
}