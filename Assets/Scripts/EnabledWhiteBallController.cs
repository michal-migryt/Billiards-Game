using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnabledWhiteBallController: MonoBehaviour
{
    private WhiteBallController whiteBallController;
    private BallController ballController;
    private void OnMouseDown()
    {
        whiteBallController = GetComponent<WhiteBallController>();
        ballController = GetComponent<BallController>();

        if (whiteBallController != null)
        {
            if(WhiteBallController.CurrentActiveBall != null)
            {
                WhiteBallController.CurrentActiveBall.DisableController();
            }
            
            if (GameController.instance.GetGameState() == GameState.PLAYER1TURN && ballController.getBallType() == BallType.FULL)
            {
                whiteBallController.EnabledController();
                WhiteBallController.SetCurrentActiveBall(whiteBallController);
            }
            if (GameController.instance.GetGameState() == GameState.PLAYER2TURN && ballController.getBallType() == BallType.HALF)
            {
                whiteBallController.EnabledController();
                WhiteBallController.SetCurrentActiveBall(whiteBallController);
            }
        }
    }
}
