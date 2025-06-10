using UnityEngine;

namespace Codersan {
    public class AnimationPlayer : MonoBehaviour {
        [SerializeField] private bool fixedY;
        [SerializeField] private string stateName;
    
        private Animator _animator;
        private float _originalY;

        private void Start() {
            _animator = GetComponent<Animator>();
            if (fixedY) {
                _originalY = transform.position.y;
            }
            _animator.Play(stateName);
        
        }

        private void Update() {
            if (fixedY) {
                Vector3 position = transform.position;
                position.y = _originalY;
                transform.position = position;
            }
        }
    }
}