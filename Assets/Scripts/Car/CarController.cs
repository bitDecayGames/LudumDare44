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

    void Update()
    { 
        // TODO TEST Debug Code, safe to remove
        if (Input.GetKeyUp("1"))
        {
            CreateCar();
        }

        if (Input.GetKeyUp("2"))
        {
            RemoveCar();
        }
    }

    public bool HasCar()
    {
        return currentCar != null;
    }

    public void CreateCar()
    {
        if (HasCar()) {
            throw new Exception("You fool! Remove the current car first");
        }

        boardManager = FindObjectOfType<Board.BoardManager>();
        List<Board.Board.Occupier> carBois = boardManager.board.stepLocations[TAG];

        if (carBois.Count != 1) {
            throw new Exception("Need exactly one (1) car boi");
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
            throw new Exception("Can't kill a car if it's not there brah");
        }

        currentCar.AddComponent<KillAfterTime>().timeToKill = 3f;
        currentCar.AddComponent<SpriteRendererFadeOutOverTime>().timeToFadeOut = 2f;
        currentCar = null;
    }
}
