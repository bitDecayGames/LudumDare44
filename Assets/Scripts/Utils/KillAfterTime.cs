using UnityEngine;

namespace Utils {
    public class KillAfterTime : MonoBehaviour {
        public float timeToKill;
        private float time;
        private bool isDead = false;

        protected void Update() {
            if (time < timeToKill) {
                time += Time.deltaTime;
            } else if (!isDead) {
                isDead = true;
                Destroy(gameObject);
            }
        }
    }
}