using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { START, PLAYER1TURN, PLAYER2TURN, END }
public class GameController : MonoBehaviour
{
    [SerializeField] private GameState _gameState = GameState.START;
    //private GameState previousState = GameState.START;
    [SerializeField] private bool foul;
    [SerializeField] private BallType player1BType, player2BType;
    [SerializeField] private PocketType player1ChosenPocket, player2ChosenPocket;
    [SerializeField] private LayerMask tableLayer;
    [SerializeField] private Vector3 tableCenter, waitingPoint;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private List<int> P1PocketedBalls = new List<int>(), P2PocketedBalls = new List<int>();
    [SerializeField] private List<BallController> Balls = new List<BallController>();
    private bool didPocketOwnBall = false;
    public static GameController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        foul = false;
        _gameState = GameState.PLAYER1TURN;

    }
    void Start()
    {
        _uiManager.DisableMenus();
        PhysicsController.instance.SetDefaultPhysics();

        player1BType = BallType.FULL;
        player2BType = BallType.HALF;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_uiManager != null)
            {
                _uiManager.ShowGameMenu();
            }
        }
    }

    public void CheckPocketedBall(BallController ballController, PocketType pocketType)
    {
        BallType ballType = ballController.getBallType();
        if (_gameState == GameState.PLAYER1TURN && P1PocketedBalls.Count == 7)
        {
            _uiManager.OnGameEnd("Player 2 wins");
            RemoveFromBalls(ballController);
            _gameState = GameState.END;
        }
        if (_gameState == GameState.PLAYER2TURN && P2PocketedBalls.Count == 7)
        {
            _uiManager.OnGameEnd("Player 1 wins");
            RemoveFromBalls(ballController);
            _gameState = GameState.END;
        }
        switch (ballType)
        {
            case BallType.HALF:
                if (player1BType == BallType.HALF)
                {
                    P1PocketedBalls.Add(ballController.getBallNumber());
                    /*if (!didPocketOwnBall)
                        didPocketOwnBall = _gameState == GameState.PLAYER1TURN ? true : false;*/
                    _gameState = GameState.PLAYER2TURN;
                    didPocketOwnBall = true;
                }
                else
                {
                    P2PocketedBalls.Add(ballController.getBallNumber());
                    /*if (!didPocketOwnBall)
                        didPocketOwnBall = _gameState == GameState.PLAYER2TURN ? true : false;*/
                    _gameState = GameState.PLAYER2TURN;
                    didPocketOwnBall = false;
                }
                RemoveFromBalls(ballController);
                _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
                break;
            case BallType.FULL:
                if (player1BType == BallType.FULL)
                {
                    P1PocketedBalls.Add(ballController.getBallNumber());
                    /*if (!didPocketOwnBall)
                        didPocketOwnBall = _gameState == GameState.PLAYER1TURN ? true : false;*/
                    _gameState = GameState.PLAYER1TURN;
                    didPocketOwnBall = false;
                }
                else
                {
                    P2PocketedBalls.Add(ballController.getBallNumber());
                    /*if (!didPocketOwnBall)
                        didPocketOwnBall = _gameState == GameState.PLAYER2TURN ? true : false;*/
                    _gameState = GameState.PLAYER1TURN;
                    didPocketOwnBall = true;
                }
                RemoveFromBalls(ballController);
                _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
                break;

        }
    }
    public void Foul()
    {
        Debug.Log("Foul");
        foul = true;
    }
    public Vector3 GetWaitingPoint()
    {
        return waitingPoint;
    }
    //TODO Finish CheckChangeTurn
    public void CheckChangeTurn()
    {

        if (!didPocketOwnBall)
        {
            _gameState = _gameState == GameState.PLAYER1TURN ? GameState.PLAYER2TURN : GameState.PLAYER1TURN;
            _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
            return;
        }
        didPocketOwnBall = false;
    }
    public LayerMask WhatIsTable()
    {
        return tableLayer;
    }
    public void EndFoul()
    {
        foul = false;
    }
    public bool IsFoul()
    {
        return foul;
    }
    public bool AreBallsMoving()
    {
        bool temp = false;
        foreach (BallController ballController in Balls)
        {
            if (ballController.isMoving())
            {
                temp = true;
                break;
            }
        }
        //Debug.Log(temp);
        return temp;
    }
    public void RemoveFromBalls(BallController ballController)
    {

        Balls.Remove(ballController);
        Debug.Log("Removed ball nr " + ballController.getBallNumber());

    }
    public GameState GetGameState()
    {
        return _gameState;
    }
}