using UnityEngine;

namespace Utils {
    public class MoveUp : MonoBehaviour {
        public float Speed;
        
        private void Update() {
            var pos = transform.position;
            pos.y += Speed;
            transform.position = pos;
        }
    }
}