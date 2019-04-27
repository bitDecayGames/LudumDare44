using UnityEngine;

namespace Utils {
    public class FadeInFromBlack : MonoBehaviour {
        private const float timeToFade = 2f;
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
            transform.position = camPos;
            transform.localScale = new Vector3(1, 1, 0) * 10000f + new Vector3(0, 0, 1);
            started = true;
        }
        
        void Update() {
            if (started) {
                if (time < timeToFade) {
                    time += Time.deltaTime;
                    SetAlpha(Mathf.Clamp(1 - time / timeToFade, 0f, 1f));
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