using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Werewolf Data")]
public class WerewolfData : ScriptableObject
{
   
   [Header("Gravity")]
   [HideInInspector] public float gravityStrength; 
   public float gravityScale; 
   [Space(5)]
   public float fallGravityMult; 
   public float maxFallSpeed;
   public float maxFastFallSpeed; 
   [Space(20)]

   
   [Header("Move")]
   public float walkMaxSpeed = 5f; 
   public float walkAcceleration = 0.1f; 
   [HideInInspector] public float walkAccelAmount; 
   [Space(5)]
   public float runMaxSpeed = 10f; 
   public float runAcceleration = 0.2f; 
   [HideInInspector] public float runAccelAmount; 
   public float runDecceleration = 0.2f; 
   [HideInInspector] public float runDeccelAmount; 
   [Space(5)]
   [Range(0.01f, 1)] public float accelInAir = 0.5f; 
   [Range(0.01f, 1)] public float deccelInAir = 0.5f;
   public bool doConserveMomentum = true;
   
   [Header("Attack")]
   public float attackForce;

   [Header("Jump")] 
   public float jumpForce;
   public float jumpTimeToApex; 
   public float jumpHeight;
   public float jumpHangTimeThreshold;
   public float jumpHangAccelerationMult; 
   public float jumpHangMaxSpeedMult;

   [Header("Dogde")]
   public float dogdeDistance;
   public float dogdeTime;
   [HideInInspector] public float dogdeForce;
   
   [Header("Hurt")]
   public float hurtForce;
   
   [Header("Coldown attack")]
   public float attack2Coldown;
   public float attack3Coldown;
   public float attack4Coldown;
   public float attack5Coldown;
  
   private void OnValidate()
   {
      gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
      gravityScale = gravityStrength / Physics2D.gravity.y;
      
      jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
      
      const float FixedUpdatesPerSecond = 50f;
      dogdeForce = dogdeDistance / dogdeTime;
      
      walkAccelAmount = (FixedUpdatesPerSecond * walkAcceleration) / walkMaxSpeed;
      runAccelAmount = (FixedUpdatesPerSecond * runAcceleration) / runMaxSpeed;
      runDeccelAmount = (FixedUpdatesPerSecond * runDecceleration) / runMaxSpeed;
        
      walkAcceleration = Mathf.Clamp(walkAcceleration, 0.01f, walkMaxSpeed);
      runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
      runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
   }
}
