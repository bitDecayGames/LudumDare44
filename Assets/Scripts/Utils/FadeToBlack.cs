using System;
using UnityEditor;
using UnityEngine;

namespace Utils {
    public class FadeToBlack : MonoBehaviour {
        private float timeToFade;
        private float time;
        private bool started;
        private Action onDone;

        private SpriteRenderer spriteRenderer;
        
        private static FadeToBlack instance;
        public static FadeToBlack Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject gameObject = new GameObject();
                    instance = gameObject.AddComponent<FadeToBlack>();
                    instance.spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                    instance.spriteRenderer.sprite = Resources.Load<Sprite>("Images/white_pixel");
                    instance.spriteRenderer.color = new Color(0,0,0,0);
                    gameObject.name = "FadeToBlack";
                }

                return instance;
            }
        }
	
        void Awake ()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        
        void Start() {
            SetAlpha(0);
            transform.position = new Vector3(100000, 100000, 0);
            transform.localScale = new Vector3(1, 1, 1);
            spriteRenderer.sortingOrder = 8000;
            var camPos = Camera.main.transform.position;
            camPos.z = 0;
            transform.position = camPos;
            transform.localScale = new Vector3(1, 1, 0) * 10000000f + new Vector3(0, 0, 1);
        }
        
        void Update() {
            if (started) {
                if (time < timeToFade) {
                    time += Time.deltaTime;
                    SetAlpha(Mathf.Clamp(time / timeToFade, 0f, 1f));
                } else {
                    started = false;
                    if (onDone != null) onDone();
                }
            }
        }

        public bool IsFadingOut()
        {
            return started;
        }

        public void FadeOut(float timeToFade, Action onDone) {
            if (started)
            {
                return;
            }
            
            this.timeToFade = timeToFade;
            time = 0;
            this.onDone = onDone;
            started = true;
        }

        private void SetAlpha(float a) {
            var c = spriteRenderer.color;
            c.a = a;
            spriteRenderer.color = c;
        }
    }
}