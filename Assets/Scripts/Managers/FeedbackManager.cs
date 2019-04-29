using TMPro;
using UnityEngine;

namespace Managers {
    public class FeedbackManager : MonoBehaviour {
        public TextMeshPro PositiveFeedbackTextPrefab;
        public TextMeshPro NegativeFeedbackTextPrefab;

        public void Positive(string text, Transform transform) {
            if (transform.Find("Sprite") != null)
            {
                transform = transform.Find("Sprite").transform;
            }
            var t = Instantiate(PositiveFeedbackTextPrefab, transform);
            t.text = text;
        }

        public void Negative(string text, Transform transform) {
            if (transform.Find("Sprite") != null)
            {
                transform = transform.Find("Sprite").transform;
            }
            var t = Instantiate(NegativeFeedbackTextPrefab, transform);
            t.text = text;
        }
    }
}