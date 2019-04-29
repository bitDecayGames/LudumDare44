using System.Collections.Generic;
using UnityEngine;

namespace Environment {
    public class Door : MonoBehaviour {
        private Animator animator;
        private bool isOpen = false;
        private int inMe = 0;

        private void Start() {
            animator = GetComponentInChildren<Animator>();
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
            if (animator != null && !isOpen) {
                animator.Play("Open");
                isOpen = true;
            }
        }

        public void Close() {
            if (animator != null && isOpen) {
                animator.Play("Close");
                isOpen = false;
            }
        }
    }
}