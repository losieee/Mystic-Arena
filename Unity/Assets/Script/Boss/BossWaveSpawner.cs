using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BossWaveSpawner : MonoBehaviour
{
    private bool effectPlayed = false;
    private BossController boss;
    public GameObject warningAreaPrefab;
    public Transform cameraShakeHolder;
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
        [Header("CameraShake")]
        public bool isShakeCamera = false;
        [Header("Integer")]
        public GameObject integerPrefab;

        // Bottom 전용
        [Header("Bottom")]
        public GameObject spawnAfterEffect;
        public GameObject attackEffectPrefab;
        public AudioClip bottomSound;
        public float bottomDestroyTime = 0.5f;

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
        public float lazerBeamDuration = 4f;        //레이저 지속시간
        public float lazerSpeed = 1f;               //레이저 회전 속도
        public float lazerSoundDuration = 4f;          //레이저 사운드 재생 길이

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
        boss = GameObject.FindGameObjectWithTag("Boss")?.GetComponent<BossController>();

        StopAllCoroutines();
        StartCoroutine(SpawnWaveSequence());
    }

    private IEnumerator SpawnWaveSequence()
    {
        BossController boss = GameObject.FindGameObjectWithTag("Boss")?.GetComponent<BossController>();

        foreach (WarningWave wave in waveSequence)
        {
            if (boss == null || boss.currentHP <= 0 || Fight_Demo.isDead)
            {
                yield break;
            }

            effectPlayed = false;

            List<WarningConfig> meteorConfigs = new List<WarningConfig>();
            int meteorIndex = 0;
            foreach (WarningConfig config in wave.warnings)
            {
                int myMeteorIndex = meteorIndex;

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
                        float rotateDuration = 1.5f;         // 회전 시간s
                        float effectDuration = 2.2f;         // 이펙트 보여지는 시간
                        float totalWarningTime = rotateDuration + effectDuration;

                        Quaternion from = rotatingTarget.transform.rotation;
                        float rotationAmount = config.lazerRotateClockwise ? 90f : -90f;
                        Quaternion to = Quaternion.Euler(from.eulerAngles + new Vector3(0, rotationAmount, 0));
                        StartCoroutine(RotateThenSpawnReadyEffect(rotatingTarget.transform, from, to, rotateDuration, effectDuration, config));
                    }
                }
                if (config.attackType == AttackType.Meteor)
                {
                    meteorIndex++;
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
                        StartCoroutine(AttackThenSpawnIntegerWhenAttackEnds(config, myMeteorIndex));
                    });

                    controller.enabled = true;
                }
            }

            float maxDuration = 0f;
            foreach (var config in wave.warnings)
                maxDuration = Mathf.Max(maxDuration, config.warningDuration);

            yield return WaitForOrInterrupt(maxDuration);

            if (boss == null || boss.currentHP <= 0 || Fight_Demo.isDead)
            {
                yield break;
            }

            yield return WaitForOrInterrupt(wave.waitAfterWave);
        }
    }
    private IEnumerator AttackThenSpawnIntegerWhenAttackEnds(WarningConfig config, int meteorIndex = 0)
    {
        if (boss == null || boss.currentHP <= 0 || Fight_Demo.isDead)
            yield break;

        Coroutine shakeCoroutine = null;
        if (config.isShakeCamera)
        {
            shakeCoroutine = StartCoroutine(CameraShake(config.warningDuration, 0.3f));  // 진동 크기 조절
        }

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
                    GameObject audioObj = new GameObject("TempAudio_BottomSound");
                    audioObj.transform.position = transform.position;

                    AudioSource audioSource = audioObj.AddComponent<AudioSource>();
                    audioSource.clip = config.bottomSound;
                    audioSource.volume = 0.01f;
                    audioSource.loop = false;

                    audioSource.Play();

                    Destroy(audioSource, 0.5f);
                    Destroy(audioObj, 0.5f);
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
                yield return WaitForOrInterrupt(1f);
                break;

            case AttackType.Laser:
                if (boss == null || boss.currentHP <= 0 || Fight_Demo.isDead)
                    yield break;

                if (config.lazerSound != null)
                {
                    GameObject audioObj = new GameObject("TempAudio_LaserSound");
                    audioObj.transform.position = config.lazerReadyEffectPosition;

                    AudioSource audioSource = audioObj.AddComponent<AudioSource>();
                    audioSource.clip = config.lazerSound;
                    audioSource.volume = 0.01f;
                    audioSource.loop = true;

                    audioSource.Play();

                    Destroy(audioObj, config.lazerSoundDuration);
                }

                // 레이저 빔 생성
                if (config.lazerBeamPrefab != null)
                {
                    GameObject rotationTarget = GameObject.Find("Boss");
                    if (rotationTarget != null)
                    {
                        float direction = config.lazerRotateClockwise ? 1f : -1f;
                        float rotateDuration = config.lazerRotateDuration * config.lazerSpeed;
                        float beamDuration = rotateDuration;

                        for (int i = 0; i < config.lazerShotCount; i++)
                        {
                            float angleOffset = direction * config.lazerShotAngleGap * i;

                            GameObject beam = Instantiate(config.lazerBeamPrefab);
                            beam.transform.SetParent(rotationTarget.transform);
                            beam.transform.localPosition = config.lazerBeamSpawnPosition;

                            Vector3 rotation = config.lazerBeamRotation + new Vector3(0f, angleOffset, 0f);
                            beam.transform.localRotation = Quaternion.Euler(rotation);
                            beam.transform.localScale = config.lazerBeamScale;

                            Destroy(beam, beamDuration);
                        }

                        // 회전 코루틴 실행
                        StartCoroutine(RotateAfterWarning(
                            rotationTarget.transform,
                            rotateDuration,
                            direction * config.lazerRotateAngle,
                            0.6f,
                            0f,
                            config.lazerSound,
                            config.lazerReadyEffectPosition
                        ));
                        yield return WaitForOrInterrupt(beamDuration);
                    }
                }
                break;

            case AttackType.Meteor:
                float fixedMeteorDelay = 0.3f;
                yield return WaitForOrInterrupt(meteorIndex * fixedMeteorDelay);

                if (config.meteorPrefab != null)
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
                    // 메테오 떨어질때마다 지진
                    if (config.isShakeCamera)
                    {
                        StartCoroutine(CameraShake(0.5f, 0.3f));
                    }

                    // 이펙트 실행
                    StartCoroutine(PlayMeteorEffectAfterDelay(config, config.position));

                    Destroy(meteor, 0.2f);

                    yield return WaitForOrInterrupt(0.7f);
                }
                break;
        }

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            cameraShakeHolder.localPosition = Vector3.zero;
        }

        //integerPrefab
        if (config.integerPrefab != null)
        {
            Vector3 myTargetPosition = new Vector3(-0.77f, 7f, -25.29f);
            SpawnInteger(myTargetPosition);
        }
    }

    private IEnumerator WaitForOrInterrupt(float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            if (boss == null || boss.currentHP <= 0 || Fight_Demo.isDead)
                yield break;

            elapsed += Time.deltaTime;
            yield return null;
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
        if (startY > 180f) startY -= 360f; // -180 ~ 180 범위로 정규화

        float targetY = startY + firstYRotation;

        float t = 0f;
        while (t < firstDuration)
        {
            if (boss == null || boss.currentHP <= 0 || Fight_Demo.isDead) yield break;

            float currentY = Mathf.Lerp(startY, targetY, t / firstDuration);
            target.rotation = Quaternion.Euler(0f, currentY, 0f);
            t += Time.deltaTime;
            yield return null;
        }
        target.rotation = Quaternion.Euler(0f, targetY, 0f);

        yield return WaitForOrInterrupt(0.3f);

        // 항상 절대 -180도로 복귀
        float returnStartY = target.rotation.eulerAngles.y;
        if (returnStartY > 180f) returnStartY -= 360f;

        float returnTargetY = -180f;

        float t2 = 0f;
        while (t2 < secondDuration)
        {
            if (boss == null || boss.currentHP <= 0 || Fight_Demo.isDead) yield break;
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

        // 사운드
        if (config.meteorSound != null)
        {
            GameObject audioObj = new GameObject("TempAudio_MeteorSound");
            audioObj.transform.position = position;

            AudioSource audioSource = audioObj.AddComponent<AudioSource>();
            audioSource.clip = config.meteorSound;
            audioSource.volume = 0.01f;
            audioSource.loop = false;

            audioSource.Play();

            Destroy(audioObj, config.meteorSound.length);
        }

        // 이펙트
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
    public void SpawnInteger(Vector3 position)
    {
        foreach (WarningWave wave in waveSequence)
        {
            foreach (WarningConfig config in wave.warnings)
            {
                if (config.integerPrefab != null)
                {
                    GameObject integerObj = Instantiate(config.integerPrefab, position, Quaternion.identity);

                    Rigidbody rb = integerObj.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.useGravity = true;
                        rb.AddForce(Vector3.down * 100f);
                    }
                    return;
                }
            }
        }
    }
    // 카메라 흔들림 효과
    private IEnumerator CameraShake(float duration, float magnitude)
    {
        if (cameraShakeHolder == null)
        {
            yield break;
        }

        Vector3 originalPos = cameraShakeHolder.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cameraShakeHolder.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraShakeHolder.localPosition = originalPos;
    }
}