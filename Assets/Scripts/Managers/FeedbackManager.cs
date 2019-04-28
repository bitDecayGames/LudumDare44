using TMPro;
using UnityEngine;

namespace Managers {
    public class FeedbackManager : MonoBehaviour {
        public TextMeshPro PositiveFeedbackTextPrefab;
        public TextMeshPro NegativeFeedbackTextPrefab;

        public void Positive(string text) {
            var t = Instantiate(PositiveFeedbackTextPrefab);
            t.text = text;
        }

        public void Negative(string text) {
            var t = Instantiate(NegativeFeedbackTextPrefab);
            t.text = text;
        }
    }
}