using TMPro;
using UnityEngine;
using Utils;

namespace Menus {
    public class Score : MonoBehaviour {
        public static int Money;
        public static int Happiness;
        public static int CompletedTasks;
        public static int FailedTasks;
        public static int TotalTasks;
        public static string LevelName;
        public static string NextLevelName;

        public TextMeshProUGUI levelName;
        public TextMeshProUGUI money;
        public TextMeshProUGUI happiness;
        public TextMeshProUGUI completedTasks;
        public TextMeshProUGUI failedTasks;
        public TextMeshProUGUI totalTasks;

        private EasyNavigator navigator;

        private void Start() {
            navigator = gameObject.AddComponent<EasyNavigator>();

            levelName.text = LevelName;
            money.text = "" + Money;
            happiness.text = "" + Happiness;
            completedTasks.text = "" + CompletedTasks;
            failedTasks.text = "" + FailedTasks;
            totalTasks.text = "" + TotalTasks;
        }

        public void OnClickRestartLevel() {
            navigator.GoToScene(LevelName);
        }

        public void OnClickNextLevel() {
            navigator.GoToScene(NextLevelName);
        }
    }
}