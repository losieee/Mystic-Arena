using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameView gameView;
    public GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = gameView.gameObject.AddComponent<GameController>();
        gameController.gameView = gameView;
    }
}
