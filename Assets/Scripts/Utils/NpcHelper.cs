using UnityEngine;

namespace Utils {
    public static class NpcHelper {
        
        public static void SmartNamer(GameObject go, TaskStepType taskType) {
            var npcName = go.name.Split('|')[0];
            go.name = npcName + "|" + taskType;
        }
        public static void SmartNamer(GameObject go, TaskStepType taskType, Direction curDirection) {
            var npcName = go.name.Split('|')[0];
            go.name = npcName + "|" + taskType + "|" + curDirection;
        }
    }
}