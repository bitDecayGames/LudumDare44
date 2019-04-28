using TMPro;
using UnityEngine;

namespace Utils {
    public class TextMeshProFadeOutOverTime : AbstractFadeOutOverTime {
        private TextMeshPro txt;

        void Start() {
            txt = GetComponentInChildren<TextMeshPro>();
        }
        
        void Update() {
            _Update();
        }

        public override bool IsReady() {
            return txt != null;
        }

        public override Color GetColor() {
            return txt.color;
        }

        public override void SetColor(Color color) {
            txt.color = color;
        }
    }
}