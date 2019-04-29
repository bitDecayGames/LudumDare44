using System;
using System.Collections.Generic;
using Board;
using Managers;
using TMPro;
using UnityEngine;

namespace Utils {
    public class OneScriptToRuleThemAll : MonoBehaviour {
        public BoardPosition playerPrefab;
        public NpcObjects npcs;
        public IconObjects icons;
        public FadeInFromBlack fadeInPrefab;
        public TextMeshPro positiveFeedbackTextPrefab;
        public TextMeshPro negativeFeedbackTextPrefab;
        public List<Sprite> cars;

        [HideInInspector] public Loader loader;
        [HideInInspector] public BoardManager board;
        [HideInInspector] public FeedbackManager feedback;
        [HideInInspector] public TaskManager tasker;
        [HideInInspector] public IconManager iconer;
        [HideInInspector] public EasyNavigator navigator;
        [HideInInspector] public CarController car;

        private FadeInFromBlack fadeInner;

        private void Start() {
            if (playerPrefab == null) throw new Exception("player prefab must not be null, drag the player prefab onto this script");
            if (npcs == null) throw new Exception("Npcs must not be null, drag the AllNpcObjects scriptable object onto this script");
            if (icons == null) throw new Exception("Icons must not be null, drag the AllIconsObjects scriptable object onto this script");
            if (fadeInPrefab == null) throw new Exception("You must have a FadeInFromBlack attached to this script");
            if (positiveFeedbackTextPrefab == null) throw new Exception("You need to drag the positive feedback text prefab onto this script");
            if (negativeFeedbackTextPrefab == null) throw new Exception("You need to drag the negative feedback text prefab onto this script");

            loader = FindObjectOfType<Loader>();
            if (loader == null) loader = gameObject.AddComponent<Loader>();
            board = FindObjectOfType<BoardManager>();
            if (board == null) board = gameObject.AddComponent<BoardManager>();
            feedback = FindObjectOfType<FeedbackManager>();
            if (feedback == null) feedback = gameObject.AddComponent<FeedbackManager>();
            tasker = FindObjectOfType<TaskManager>();
            if (tasker == null) tasker = gameObject.AddComponent<TaskManager>();
            iconer = FindObjectOfType<IconManager>();
            if (iconer == null) iconer = gameObject.AddComponent<IconManager>();
            navigator = FindObjectOfType<EasyNavigator>();
            if (navigator == null) navigator = gameObject.AddComponent<EasyNavigator>();

            car = FindObjectOfType<CarController>();
            if (car == null) car = gameObject.AddComponent<CarController>();
            car.carSprites = cars;

            fadeInner = Instantiate(fadeInPrefab);

            Setup();
        }

        void Setup() {
            loader.playerPrefab = playerPrefab;
            feedback.PositiveFeedbackTextPrefab = positiveFeedbackTextPrefab;
            feedback.NegativeFeedbackTextPrefab = negativeFeedbackTextPrefab;
            tasker.Npcs = npcs;
            tasker.Feedback = feedback;
            iconer.Icons = icons;
        }
    }
}