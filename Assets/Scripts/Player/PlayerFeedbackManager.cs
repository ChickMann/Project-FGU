using UnityEngine;

public enum PlayerFeedbackType
{
    parry,
    attack,
    Hurt,
    Death,
    Dodge,
    Jump,
    Landing,
    DrawSword,
    Footstep,
    SheathSword,
    Wall_slide,
    Grab,
    Run_Stop,
    Health
}

[System.Serializable]
public class PlayerFeedbackEntity : BaseFeedbackEntity<PlayerFeedbackType> { }

public class PlayerFeedbackManager : BaseFeedbackManager<PlayerFeedbackType, PlayerFeedbackEntity> { }