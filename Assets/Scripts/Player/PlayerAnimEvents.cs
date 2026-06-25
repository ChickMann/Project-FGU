using System;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    PlayerFeedbackManager playerFeedbackManager;
    PlayerController playerController;

    private void Awake()
    {
        playerFeedbackManager = GetComponent<PlayerFeedbackManager>();
        playerController = GetComponent<PlayerController>();
    }

    void AE_footstep()
    {
        playerFeedbackManager.PlayFeedback(PlayerFeedbackType.Footstep);
    }

    void AE_Jump()
    {
        playerFeedbackManager.PlayFeedback(PlayerFeedbackType.Jump);
    }

    void AE_Landing()
    {
        playerFeedbackManager.PlayFeedback(PlayerFeedbackType.Landing);
    }

    void AE_Parry()
    {
        playerFeedbackManager.PlayFeedback(PlayerFeedbackType.parry);
    }
    void AE_ParryStance()
    {
        playerFeedbackManager.PlayFeedback(PlayerFeedbackType.DrawSword);
    }
    
    void AE_Hurt()
    {
        playerFeedbackManager.PlayFeedback(PlayerFeedbackType.Hurt);
    }

    void AE_Death()
    {
        playerFeedbackManager.PlayFeedback(PlayerFeedbackType.Death);
    }

    void AE_SwordAttack()
    {
        playerFeedbackManager.PlayFeedback(PlayerFeedbackType.attack);
    }
    void AE_Dodge()
    {
        playerFeedbackManager.PlayFeedback(PlayerFeedbackType.Dodge);
    }

    void AE_WallSlide()
    {
       playerFeedbackManager.PlayFeedback(PlayerFeedbackType.Wall_slide);
    }

    void AE_LedgeGrab()
    {
       playerFeedbackManager.PlayFeedback(PlayerFeedbackType.Grab);
    }

    void AE_LedgeClimb()
    {
       playerFeedbackManager.PlayFeedback(PlayerFeedbackType.Run_Stop);
    }
    void AE_RunStop()
    {
        playerController.RunStop();
    }

    void AE_SheathSword()
    {
        playerFeedbackManager.PlayFeedback(PlayerFeedbackType.SheathSword);
    }
    public void AE_setPositionToClimbPosition()
    {
        playerController.SetPositionToClimbPosition();
    }
}
