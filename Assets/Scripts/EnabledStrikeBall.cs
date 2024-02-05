using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnabledStrikeBall : MonoBehaviour
{
    private StrikeBall strikeBall;
    private BallController ballController;
    private void OnMouseDown()
    {
        if (GameController.instance.AreBallsMoving() == false)
        {
            StrikeBall.CurrentActiveBall.DisableController();
            strikeBall = GetComponent<StrikeBall>();
            ballController = GetComponent<BallController>();

            if (GameController.instance.GetGameState() == GameState.PLAYER1TURN && ballController.getBallType() == BallType.WHITE)
            {
                strikeBall.EnabledController();
                StrikeBall.SetCurrentActiveBall(strikeBall);
            }
            if (GameController.instance.GetGameState() == GameState.PLAYER2TURN && ballController.getBallType() == BallType.BLACK)
            {
                strikeBall.EnabledController();
                StrikeBall.SetCurrentActiveBall(strikeBall);
            }
        }
    }
}