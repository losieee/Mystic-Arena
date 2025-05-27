using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class BossWaveSpawner : MonoBehaviour
{
    private bool effectPlayed = false;
    public GameObject attackBehaviorPrefab;         //�����Ҷ� ���� �ൿ
    public GameObject warningAreaPrefab;            //���豸�� ǥ��
    public WarningWave[] waveSequence;              //Wave �д�?

    [System.Serializable]
    public class WarningConfig
    {
        public Vector3 position;        //��ġ
        public Vector3 scale = new Vector3(3, 1, 3);    //ũ��
        public Vector3 rotationEuler;       //����
        public float warningDuration = 2f;  //������ ��ٸ��� �ð�
        public FillPatternType fillPattern = FillPatternType.FromEdge;  //���� ��������(�Ʒ����� �� or �߾ӿ��� ��ü)
        public GameObject spawnAfterEffect;     //��� �� ������ ������
        public GameObject attackEffectPrefab;           //���� ����Ʈ

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
                        // ���� ����Ʈ�� �� �� ���� ����
                        if (!effectPlayed && attackBehaviorPrefab != null)
                        {
                            Vector3 pos = wave.effectSpawnPosition + Vector3.up * 0.5f;
                            GameObject fx = Instantiate(attackBehaviorPrefab, pos, Quaternion.identity);
                            Destroy(fx, 1f);
                            effectPlayed = true;
                        }

                        // ���� ���� ����Ʈ ����
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

                        // ���� ���� ���� ����
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

            // ���� �� ��� ���� ������ ���
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

        // �� Renderer���� ���� ��Ƽ���� ����
        List<Material> materials = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            materials.Add(renderer.material); // material ��� �� �ν��Ͻ�ȭ��
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