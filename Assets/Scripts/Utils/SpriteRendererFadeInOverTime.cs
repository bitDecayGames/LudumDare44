using UnityEngine;

namespace Utils {
    public class SpriteRendererFadeInOverTime : AbstractFadeInOverTime {
        private SpriteRenderer sprite;

        void Start() {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }
        
        void Update() {
            _Update();
        }

        public override bool IsReady() {
            return sprite != null;
        }

        public override Color GetColor() {
            return sprite.color;
        }

        public override void SetColor(Color color) {
            sprite.color = color;
        }
    }
}