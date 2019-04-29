using UnityEngine;

namespace Utils {
    public abstract class AbstractFadeInOverTime : MonoBehaviour {
        public float timeToFadeIn;
        private float time;

        public abstract bool IsReady();
        public abstract Color GetColor();
        public abstract void SetColor(Color color);

        protected void _Update() {
            if (IsReady() && time < timeToFadeIn) {
                time += Time.deltaTime;
                var c = GetColor();
                c.a = Mathf.Clamp(time / timeToFadeIn, 0, 1);
                SetColor(c);
            }
        }
    }
}