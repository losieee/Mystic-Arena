using UnityEngine;
using System.Collections;

namespace FullOpaqueVFX
{
    public class CameraShake : MonoBehaviour
    {
        private Vector3 originalPosition;
        private float shakeMagnitude;
        private float shakeDuration;

        void Start()
        {
            originalPosition = transform.localPosition;
        }

        public void Shake(float magnitude, float duration)
        {
            if (duration > 0)
            {
                shakeMagnitude = magnitude;
                shakeDuration = duration;
                StartCoroutine(PerformShake());
            }
        }

        private IEnumerator PerformShake()
        {
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                float x = Random.Range(-1f, 1f) * shakeMagnitude;
                float y = Random.Range(-1f, 1f) * shakeMagnitude;
                transform.localPosition = originalPosition + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPosition;
        }
    }
}
