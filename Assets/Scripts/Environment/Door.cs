using System.Collections.Generic;
using UnityEngine;

namespace Environment {
    public class Door : MonoBehaviour {
        private List<Animator> animators = new List<Animator>();
        private bool isOpen = false;
        private int inMe = 0;

        private void Start() {
            animators.AddRange(GetComponentsInChildren<Animator>());
            Close();
        }

        private void Update() {
            if (inMe > 0) Open();
            else Close();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            inMe++;
        }

        private void OnTriggerEnter(Collider other) {
            inMe++;
        }

        private void OnTriggerExit2D(Collider2D other) {
            inMe--;
        }

        private void OnTriggerExit(Collider other) {
            inMe--;
        }

        public void Open() {
            if (animators.Count > 0 && !isOpen) {
                FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.SlidingDoorOpen);
                animators.ForEach(a => a.Play("Open"));
                isOpen = true;
            }
        }

        public void Close() {
            if (animators.Count > 0 && isOpen) {
                FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.SlidingDoorClose);
                animators.ForEach(a => a.Play("Close"));
                isOpen = false;
            }
        }
    }
}