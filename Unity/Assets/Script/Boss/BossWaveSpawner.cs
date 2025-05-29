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

        // 공통
        public Vector3 position;
        public Vector3 scale = new Vector3(3, 1, 3);
        public Vector3 rotationEuler;
        public float warningDuration = 2f;
        public FillPatternType fillPattern = FillPatternType.FromEdge;

        // Bottom 전용
        [Header("Bottom")]
        public GameObject spawnAfterEffect;
        public GameObject attackEffectPrefab;
        public AudioClip bottomSound;

        // Lazer 전용
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
        public bool lazerRotateClockwise = true;    //true - 시계방향, false - 반시계방향
        public float lazerRotateDuration = 1.5f;    //레이저 회전 시간
        public float lazerRotateAngle = 180f;       //회전할 각도
        public int lazerShotCount = 1;              //레이저 수
        public float lazerShotAngleGap = 0f;        //레이저 간 간격

        // Meteor 전용
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
                        float rotateDuration = 1.5f;         // 회전 시간
                        float effectDuration = 2.2f;         // 이펙트 보여지는 시간
                        float totalWarningTime = rotateDuration + effectDuration;

                        Quaternion from = rotatingTarget.transform.rotation;
                        float rotationAmount = config.lazerRotateClockwise ? 90f : -90f;
                        Quaternion to = Quaternion.Euler(from.eulerAngles + new Vector3(0, rotationAmount, 0));
                        StartCoroutine(RotateThenSpawnReadyEffect(rotatingTarget.transform, from, to, rotateDuration, effectDuration, config));
                    }
                }
                if (config.attackType == AttackType.Meteor && !meteorSpawnStarted)
                {
                    meteorSpawnStarted = true;
                    StartCoroutine(SpawnMeteorConfigsSequentially(wave.warnings, 0.5f));
                    meteorSpawnStarted = false;
                }

                FillController controller = instance.GetComponentInChildren<FillController>();
                if (controller != null)
                {
                    controller.fillDuration = config.warningDuration;
                    controller.fillPattern = config.fillPattern;

                    controller.onFillComplete.AddListener(() =>
                    {
                        // 공통 효과 한 번만 실행
                        if (!effectPlayed && wave.attackBehaviorPrefab != null)
                        {
                            Vector3 pos = wave.effectSpawnPosition + Vector3.up * 0.5f;
                            GameObject fx = Instantiate(wave.attackBehaviorPrefab, pos, Quaternion.identity);
                            Destroy(fx, 1f);
                            effectPlayed = true;
                        }

                        // 타입별 이펙트 처리
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
                                        float direction = config.lazerRotateClockwise ? 1f : -1f;
                                        for (int i = 0; i < config.lazerShotCount; i++)
                                        {
                                            float angleOffset = direction * config.lazerShotAngleGap * i;

                                            GameObject beam = Instantiate(config.lazerBeamPrefab);
                                            beam.transform.SetParent(rotationTarget.transform);
                                            beam.transform.localPosition = config.lazerBeamSpawnPosition;

                                            // Y축 기준 회전만 적용
                                            Vector3 rotation = config.lazerBeamRotation + new Vector3(0f, angleOffset, 0f);
                                            beam.transform.localRotation = Quaternion.Euler(rotation);
                                            beam.transform.localScale = config.lazerBeamScale;

                                            Destroy(beam, 4f);
                                        }
                                    }
                                }

                                GameObject rotatingTarget = GameObject.Find("Boss");        // 회전 및 사운드 재생
                                if (rotatingTarget != null)
                                {
                                    float direction = config.lazerRotateClockwise ? 1f : -1f;

                                    StartCoroutine(RotateAfterWarning(
                                        rotatingTarget.transform,
                                        config.lazerRotateDuration,
                                        direction * config.lazerRotateAngle,
                                        1f,
                                        0f,
                                        config.lazerSound,
                                        config.lazerReadyEffectPosition
                                    ));
                                }
                                break;

                            case AttackType.Meteor:
                                // 메테오 관련
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

        // 회전 완료 후 Ready 이펙트 생성
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
    private IEnumerator RotateAfterWarning(Transform target, float firstDuration, float firstYRotation, float secondDuration, float dummyUnused, AudioClip sound, Vector3 soundPosition)
    {
        float startY = target.rotation.eulerAngles.y;
        if (startY > 180f) startY -= 360f; // -180 ~ 180 범위로 정규화

        Quaternion from = Quaternion.Euler(0f, startY, 0f);
        Quaternion to = Quaternion.Euler(0f, startY + firstYRotation, 0f);

        // 사운드 재생
        if (sound != null)
            AudioSource.PlayClipAtPoint(sound, soundPosition);

        float t = 0f;
        while (t < firstDuration)
        {
            target.rotation = Quaternion.Lerp(from, to, t / firstDuration);
            t += Time.deltaTime;
            yield return null;
        }
        target.rotation = to;

        yield return new WaitForSeconds(0.3f);

        // 항상 절대 -180도로 복귀
        Quaternion returnTo = Quaternion.Euler(0f, -180f, 0f);
        Quaternion returnFrom = target.rotation;

        float t2 = 0f;
        while (t2 < secondDuration)
        {
            target.rotation = Quaternion.Lerp(returnFrom, returnTo, t2 / secondDuration);
            t2 += Time.deltaTime;
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
                Destroy(meteor, 0.2f);
                

                yield return new WaitForSeconds(interval);
            }
        }
    }
    private IEnumerator PlayMeteorEffectAfterDelay(WarningConfig config, Vector3 position)
    {
        yield return new WaitForSeconds(0.1f);

        // 사운드
        if (config.meteorSound != null)
        {
            AudioSource.PlayClipAtPoint(config.meteorSound, position);
        }

        // 이펙트
        if (config.meteorEffectPrefab != null)
        {
            GameObject effect = Instantiate(config.meteorEffectPrefab, position, Quaternion.identity);
            effect.transform.localScale = config.meteorEffectScale;
            Destroy(effect, 0.7f); // 이펙트도 일정 시간 뒤 파괴
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