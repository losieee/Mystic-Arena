using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class BossWaveSpawner : MonoBehaviour
{
    private bool effectPlayed = false;
    public GameObject attackBehaviorPrefab;         //공격할때 보스 행동
    public GameObject warningAreaPrefab;            //위험구역 표시
    public WarningWave[] waveSequence;              //Wave 분담?

    [System.Serializable]
    public class WarningConfig
    {
        public Vector3 position;        //위치
        public Vector3 scale = new Vector3(3, 1, 3);    //크기
        public Vector3 rotationEuler;       //기울기
        public float warningDuration = 2f;  //끝나고 기다리는 시간
        public FillPatternType fillPattern = FillPatternType.FromEdge;  //무슨 패턴인지(아래에서 위 or 중앙에서 전체)
        public GameObject spawnAfterEffect;     //경고 후 실행할 프리팹
        public GameObject attackEffectPrefab;           //공격 이펙트

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

            foreach (WarningConfig config in wave.warnings)
            {
                GameObject instance = Instantiate(
                    warningAreaPrefab,
                    config.position,
                    Quaternion.Euler(config.rotationEuler)
                );

                instance.transform.localScale = config.scale;

                FillController controller = instance.GetComponentInChildren<FillController>();
                if (controller != null)
                {
                    controller.fillDuration = config.warningDuration;
                    controller.fillPattern = config.fillPattern;

                    controller.onFillComplete.AddListener(() =>
                    {
                        // 공통 이펙트는 단 한 번만 실행
                        if (!effectPlayed && attackBehaviorPrefab != null)
                        {
                            Vector3 pos = wave.effectSpawnPosition + Vector3.up * 0.5f;
                            GameObject fx = Instantiate(attackBehaviorPrefab, pos, Quaternion.identity);
                            Destroy(fx, 1f);
                            effectPlayed = true;
                        }

                        // 개별 공격 이펙트 생성
                        if (config.attackEffectPrefab != null)
                        {
                            Vector3 spawnPos = config.position + Vector3.down * 0.5f;

                            GameObject attackFx = Instantiate(
                                config.attackEffectPrefab,
                                spawnPos,
                                Quaternion.Euler(config.rotationEuler)
                            );

                            Vector3 originalScale = attackFx.transform.localScale;
                            attackFx.transform.localScale = new Vector3(config.scale.x/1.5f, originalScale.y, config.scale.z/1.5f);

                            Destroy(attackFx, 1f);
                        }

                        // 개별 공격 범위 생성
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
                    });
                    controller.enabled = true;
                }
            }

            // 가장 긴 경고가 끝날 때까지 대기
            float maxDuration = 0f;
            foreach (var config in wave.warnings)
                maxDuration = Mathf.Max(maxDuration, config.warningDuration);

            yield return new WaitForSeconds(maxDuration + wave.waitAfterWave);
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

        // 각 Renderer마다 개별 머티리얼 복사
        List<Material> materials = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            materials.Add(renderer.material); // material 사용 시 인스턴스화됨
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