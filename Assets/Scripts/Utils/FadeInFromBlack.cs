using UnityEngine;

namespace Utils {
    public class FadeInFromBlack : MonoBehaviour {
        public float TimeToFade = 2f;
        public float InitialDelay = 0f;
        
        private float time;
        private bool started;

        private SpriteRenderer _sprite;

        private SpriteRenderer sprite {
            get {
                if (_sprite == null) _sprite = GetComponent<SpriteRenderer>();
                return _sprite;
            }
        }

        void Start() {
            SetAlpha(1);
            var cam = FindObjectOfType<Camera>();
            var camPos = cam.transform.position;
            camPos.z = 0;
            _sprite.sortingOrder = 1000;
            transform.position = camPos;
            transform.localScale = new Vector3(1, 1, 0) * 10000f + new Vector3(0, 0, 1);
            started = true;
        }
        
        void Update() {
            if (started) {
                if (InitialDelay > 0)
                {
                    InitialDelay -= Time.deltaTime;
                }
                else if (time < TimeToFade) {
                    time += Time.deltaTime;
                    SetAlpha(Mathf.Clamp(1 - time / TimeToFade, 0f, 1f));
                } else {
                    started = false;
                    Destroy(gameObject);
                }
            }
        }

        private void SetAlpha(float a) {
            var c = sprite.color;
            c.a = a;
            sprite.color = c;
        }
    }
}