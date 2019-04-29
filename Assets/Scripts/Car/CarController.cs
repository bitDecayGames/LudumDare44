using System;
using System.Collections.Generic;
using UnityEngine;
using Board;
using Utils;

public class CarController : MonoBehaviour {
    System.Random rnd = new System.Random();

    Board.BoardManager boardManager;
    GameObject currentCar;
    public List<Sprite> carSprites;

    string TAG = "MakeDatCarBoi".ToLower();

    public bool HasCar()
    {
        return currentCar != null;
    }

    public void CreateCar()
    {
        if (HasCar()) {
            // Debug.LogWarning("You fool! Remove the current car first");
            return;
        }

        boardManager = FindObjectOfType<Board.BoardManager>();
        List<Board.Board.Occupier> carBois = boardManager.board.stepLocations[TAG];

        if (carBois.Count != 1) {
            // Debug.LogWarning("Need exactly one (1) car boi");
            return;
        }

        int carIdx = rnd.Next(carSprites.Count);

        currentCar = new GameObject();
        currentCar.name = "Car" + carIdx;

        SpriteRenderer renderer = currentCar.AddComponent<SpriteRenderer>();
        renderer.sprite = carSprites[carIdx];

        // Prevent car for flashing on load
        Color c = renderer.color;
        c.a = 0;
        renderer.color = c;

        renderer.sortingOrder = 1;

        currentCar.AddComponent<SpriteRendererFadeInOverTime>().timeToFadeIn = 2f;

        currentCar.transform.position = carBois[0].gameObject.transform.position;
    }

    public void RemoveCar()
    {
        if (!HasCar())
        {
            // Debug.LogWarning("Can't kill a car if it's not there brah");
            return;
        }

        currentCar.AddComponent<KillAfterTime>().timeToKill = 3f;
        currentCar.AddComponent<SpriteRendererFadeOutOverTime>().timeToFadeOut = 2f;
        currentCar = null;
    }
}
