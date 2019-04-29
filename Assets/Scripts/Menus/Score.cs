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

        public static void ResetScore() {
            Money = 0;
            Happiness = 0;
            CompletedTasks = 0;
            FailedTasks = 0;
            TotalTasks = 0;
            LevelName = "this level name got reset by the ResetScore method";
            NextLevelName = "this level name got reset by the ResetScore method";
        }

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
            money.text = "$" + Money;
            happiness.text = Happiness + " / 10";
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