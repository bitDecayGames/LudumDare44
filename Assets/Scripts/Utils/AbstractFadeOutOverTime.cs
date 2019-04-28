using UnityEngine;

namespace Utils {
    public abstract class AbstractFadeOutOverTime : MonoBehaviour {
        public float timeToFadeOut;
        private float time;

        public abstract bool IsReady();
        public abstract Color GetColor();
        public abstract void SetColor(Color color);

        protected void _Update() {
            if (IsReady() && time < timeToFadeOut) {
                time += Time.deltaTime;
                var c = GetColor();
                c.a = Mathf.Clamp(1 - time / timeToFadeOut, 0, 1);
                SetColor(c);
            }
        }
    }
}