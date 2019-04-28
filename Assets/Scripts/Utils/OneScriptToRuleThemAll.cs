using System;
using Board;
using UnityEngine;

namespace Utils {
    public class OneScriptToRuleThemAll : MonoBehaviour {

        public BoardPosition playerPrefab;
        public NpcObjects npcs;
        public IconObjects icons;
        
        [HideInInspector]
        public Loader loader;
        [HideInInspector]
        public BoardManager board;
        [HideInInspector]
        public TaskManager tasker;
        [HideInInspector]
        public IconManager iconer;

        private void Start() {
            if (playerPrefab == null) throw new Exception("player prefab must not be null, drag the player prefab onto this script");
            if (npcs == null) throw new Exception("Npcs must not be null, drag the AllNpcObjects scriptable object onto this script");
            if (icons == null) throw new Exception("Icons must not be null, drag the AllIconsObjects scriptable object onto this script");
            
            loader = FindObjectOfType<Loader>();
            if (loader == null) loader = gameObject.AddComponent<Loader>();
            board = FindObjectOfType<BoardManager>();
            if (board == null) board = gameObject.AddComponent<BoardManager>();
            tasker = FindObjectOfType<TaskManager>();
            if (tasker == null) tasker = gameObject.AddComponent<TaskManager>();
            iconer = FindObjectOfType<IconManager>();
            if (iconer == null) iconer = gameObject.AddComponent<IconManager>();
            
            Setup();
        }

        void Setup() {
            loader.playerPrefab = playerPrefab;
            tasker.Npcs = npcs;
            iconer.Icons = icons;
        }
    }
}