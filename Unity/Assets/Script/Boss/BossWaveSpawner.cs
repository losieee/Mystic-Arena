using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossWaveSpawner : MonoBehaviour
{
    private bool effectPlayed = false;
    bool meteorSpawnStarted = false;
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
        [Header("Integer")]
        public bool spawnInteger = false;
        public GameObject integerPrefab;

        // Bottom ����
        [Header("Bottom")]
        public GameObject spawnAfterEffect;
        public GameObject attackEffectPrefab;
        public AudioClip bottomSound;
        public float bottomDestroyTime = 0.5f;

        // Lazer ����
        [Header("Lazer")]
        public Vector3 lazerReadyEffectPosition;
        public Vector3 lazerReadyEffectRotation;
        public GameObject lazerReadyEffectPrefab;
        public GameObject lazerBeamPrefab;
        public AudioClip lazerReadySound;
        public AudioClip lazerSound;
        public Vector3 lazerBeamSpawnPosition;
        public Vector3 lazerBeamScale;
        public Vector3 lazerBeamRotation;
        public bool lazerRotateClockwise = true;    //true - �ð����, false - �ݽð����
        public float lazerRotateDuration = 1.5f;    //������ ȸ�� �ð�
        public float lazerRotateAngle = 180f;       //ȸ���� ����
        public int lazerShotCount = 1;              //������ ��
        public float lazerShotAngleGap = 0f;        //������ �� ����
        public float lazerBeamDuration = 4f;        //������ ���ӽð�
        public float lazerSpeed = 1f;               //������ ȸ�� �ӵ�
        public float lazerSoundDuration = 4f;          //������ ���� ��� ����

        // Meteor ����
        [Header("Meteor")]
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
        public GameObject attackBehaviorPrefab;
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
                        float rotationAmount = config.lazerRotateClockwise ? 90f : -90f;
                        Quaternion to = Quaternion.Euler(from.eulerAngles + new Vector3(0, rotationAmount, 0));
                        StartCoroutine(RotateThenSpawnReadyEffect(rotatingTarget.transform, from, to, rotateDuration, effectDuration, config));
                    }
                }
                if (config.attackType == AttackType.Meteor)
                {
                    meteorConfigs.Add(config);
                }

                FillController controller = instance.GetComponentInChildren<FillController>();
                if (controller != null)
                {
                    controller.fillDuration = config.warningDuration;
                    controller.fillPattern = config.fillPattern;

                    controller.onFillComplete.AddListener(() =>
                    {
                        // ���� ȿ�� �� ���� ����
                        if (!effectPlayed && wave.attackBehaviorPrefab != null)
                        {
                            Vector3 pos = wave.effectSpawnPosition + Vector3.up * 0.5f;
                            GameObject fx = Instantiate(wave.attackBehaviorPrefab, pos, Quaternion.identity);
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
                                    Transform hitBox = spawned.transform.Find("Hitbox");
                                    Transform meshBackgound = spawned.transform.Find("Background");
                                    Vector3 adjustedScale = new Vector3(config.scale.x, config.scale.z, 1);
                                    if (meshChild != null && meshBackgound != null)
                                    {
                                        meshChild.localScale = adjustedScale;
                                        hitBox.localScale = adjustedScale;
                                        meshBackgound.localScale = adjustedScale;
                                    }
                                    else
                                    {
                                        spawned.transform.localScale = adjustedScale;
                                    }
                                    StartCoroutine(FadeOutAndDestroy(spawned, config.bottomDestroyTime));
                                }
                                break;

                            case AttackType.Laser:
                                GameObject audioObj = new GameObject("TempAudio_LaserSound");
                                audioObj.transform.position = config.lazerReadyEffectPosition;

                                AudioSource audioSource = audioObj.AddComponent<AudioSource>();
                                audioSource.clip = config.lazerSound;
                                audioSource.volume = 0.03f;
                                audioSource.loop = true;

                                audioSource.Play();

                                Destroy(audioObj, config.lazerSoundDuration);

                                if (config.lazerBeamPrefab != null)
                                {
                                    GameObject rotationTarget = GameObject.Find("Boss");
                                    if (rotationTarget != null)
                                    {
                                        float direction = config.lazerRotateClockwise ? 1f : -1f;   //��� �������� ȸ���Ұ��� (lazerRotateClockwise ���� ����)
                                        float rotateDuration = config.lazerRotateDuration * config.lazerSpeed;  // ȸ�� �ӵ� ������ ȸ�� �ð� ���
                                        float beamDuration = rotateDuration;            // ������ ���� �ð�, ȸ�� �ð��� ����ȭ

                                        // ������ ����
                                        if (config.lazerBeamPrefab != null)
                                        {
                                            for (int i = 0; i < config.lazerShotCount; i++)
                                            {
                                                float angleOffset = direction * config.lazerShotAngleGap * i;

                                                GameObject beam = Instantiate(config.lazerBeamPrefab);
                                                beam.transform.SetParent(rotationTarget.transform);
                                                beam.transform.localPosition = config.lazerBeamSpawnPosition;

                                                // Y�� ���� ȸ���� ����
                                                Vector3 rotation = config.lazerBeamRotation + new Vector3(0f, angleOffset, 0f);
                                                beam.transform.localRotation = Quaternion.Euler(rotation);
                                                beam.transform.localScale = config.lazerBeamScale;

                                                Destroy(beam, beamDuration);
                                            }
                                        }

                                        // ȸ��
                                        StartCoroutine(RotateAfterWarning(
                                            rotationTarget.transform,
                                            rotateDuration,
                                            direction * config.lazerRotateAngle,
                                            1f,
                                            0f,
                                            config.lazerSound,
                                            config.lazerReadyEffectPosition
                                        ));
                                    }
                                }
                                break;

                            case AttackType.Meteor:
                                //���׿� ����
                                break;
                        }
                        // Integer Drop ó��
                        if (config.spawnInteger && config.integerPrefab != null)
                        {
                            Vector3 dropPosition = config.position + new Vector3(0, 6f, 0);
                            GameObject integerObj = Instantiate(config.integerPrefab, dropPosition, Quaternion.identity);

                            // �������� ȿ��
                            Rigidbody rb = integerObj.GetComponent<Rigidbody>();
                            if (rb != null)
                            {
                                rb.useGravity = true;
                                rb.AddForce(Vector3.down * 100f);  // ���ϴ� �� ����
                            }
                        }
                    });

                    controller.enabled = true;
                }
            }

            float maxDuration = 0f;
            foreach (var config in wave.warnings)
                maxDuration = Mathf.Max(maxDuration, config.warningDuration);

            yield return new WaitForSeconds(maxDuration);
            if (meteorConfigs.Count > 0)
            {
                yield return StartCoroutine(SpawnMeteorConfigsSequentially(meteorConfigs.ToArray(), 0.4f));
            }
            yield return new WaitForSeconds(wave.waitAfterWave);
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
                Destroy(meteor, 0.2f);

                yield return new WaitForSeconds(interval);
            }
        }
    }
    private IEnumerator RotateAfterWarning(Transform target, float firstDuration, float firstYRotation, float secondDuration, float dummyUnused, AudioClip sound, Vector3 soundPosition)
    {
        float startY = target.rotation.eulerAngles.y;
        if (startY > 180f) startY -= 360f; // -180 ~ 180 ������ ����ȭ

        float targetY = startY + firstYRotation;

        float t = 0f;
        while (t < firstDuration)
        {
            float currentY = Mathf.Lerp(startY, targetY, t / firstDuration);
            target.rotation = Quaternion.Euler(0f, currentY, 0f);
            t += Time.deltaTime;
            yield return null;
        }
        target.rotation = Quaternion.Euler(0f, targetY, 0f);

        yield return new WaitForSeconds(0.3f);

        // �׻� ���� -180���� ����
        float returnStartY = target.rotation.eulerAngles.y;
        if (returnStartY > 180f) returnStartY -= 360f;

        float returnTargetY = -180f;

        float t2 = 0f;
        while (t2 < secondDuration)
        {
            float currentY = Mathf.LerpAngle(returnStartY, returnTargetY, t2 / secondDuration);
            target.rotation = Quaternion.Euler(0f, currentY, 0f);

            t2 += Time.deltaTime;
            yield return null;
        }
        target.rotation = Quaternion.Euler(0f, returnTargetY, 0f);
    }
    private IEnumerator PlayMeteorEffectAfterDelay(WarningConfig config, Vector3 position)
    {
        yield return new WaitForSeconds(0.1f);

        // ����
        if (config.meteorSound != null)
        {
            GameObject audioObj = new GameObject("TempAudio_MeteorSound");
            audioObj.transform.position = position;

            AudioSource audioSource = audioObj.AddComponent<AudioSource>();
            audioSource.clip = config.meteorSound;
            audioSource.volume = 0.005f;
            audioSource.loop = false;

            audioSource.Play();

            Destroy(audioObj, config.meteorSound.length);
        }

        // ����Ʈ
        if (config.meteorEffectPrefab != null)
        {
            GameObject effect = Instantiate(config.meteorEffectPrefab, position, Quaternion.identity);
            effect.transform.localScale = config.meteorEffectScale;
            Destroy(effect, 0.7f);
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