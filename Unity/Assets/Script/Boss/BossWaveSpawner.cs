using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossWaveSpawner : MonoBehaviour
{
    private bool effectPlayed = false;
    public GameObject waveStartEffectPrefab;

    public Animator bossAnimator;

    [System.Serializable]
    public class WarningConfig
    {
        public Vector3 position;
        public Vector3 scale = new Vector3(3, 1, 3);
        public Vector3 rotationEuler;
        public float warningDuration = 2f;
    }

    [System.Serializable]
    public class WarningWave
    {
        public WarningConfig[] warnings;
        public float waitAfterWave = 1f;

        public Vector3 effectSpawnPosition = Vector3.zero; 
    }

    public GameObject warningAreaPrefab;
    public WarningWave[] waveSequence;

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

                    controller.onFillComplete.AddListener(() =>
                    {
                        if (!effectPlayed && waveStartEffectPrefab != null)
                        {
                            Vector3 pos = wave.effectSpawnPosition + Vector3.up * 0.5f;

                            GameObject fx = Instantiate(waveStartEffectPrefab, pos, Quaternion.identity);

                            Destroy(fx, 1f); // 여기! 1초 뒤 자동 삭제
                            effectPlayed = true;
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
}