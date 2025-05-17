using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossWaveSpawner : MonoBehaviour
{
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
            List<float> durations = new List<float>();

            foreach (WarningConfig config in wave.warnings)
            {
                GameObject instance = Instantiate(
                    warningAreaPrefab,
                    config.position,
                    Quaternion.Euler(config.rotationEuler)
                );

                instance.transform.localScale = config.scale;

                FillController controller = instance.GetComponent<FillController>();
                if (controller != null)
                {
                    controller.fillDuration = config.warningDuration;
                    controller.enabled = true;
                }

                durations.Add(config.warningDuration);
            }

            // 가장 오래 걸리는 경고 이펙트 + 여유 시간만큼 대기
            float maxDuration = Mathf.Max(durations.ToArray());
            yield return new WaitForSeconds(maxDuration + wave.waitAfterWave);
        }
    }
}