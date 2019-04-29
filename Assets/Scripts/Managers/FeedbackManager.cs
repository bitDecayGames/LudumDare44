using TMPro;
using UnityEngine;

namespace Managers {
    public class FeedbackManager : MonoBehaviour {
        public TextMeshPro PositiveFeedbackTextPrefab;
        public TextMeshPro NegativeFeedbackTextPrefab;

        public void Positive(string text, Transform transform) {
            var t = Instantiate(PositiveFeedbackTextPrefab, transform.position, Quaternion.identity);
            t.text = text;
        }

        public void Negative(string text, Transform transform) {
            var t = Instantiate(NegativeFeedbackTextPrefab, transform.position, Quaternion.identity);
            t.text = text;
        }
    }
}