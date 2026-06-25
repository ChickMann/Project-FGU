using System;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public abstract class BaseFeedbackEntity<T> where T : Enum
{
    public T type;
    public MMF_Player player;
}

public abstract class BaseFeedbackManager<T, TEntity> : MonoBehaviour 
    where T : Enum 
    where TEntity : BaseFeedbackEntity<T>
{
    [SerializeField] private List<TEntity> feedbackList = new List<TEntity>();
    private Dictionary<T, MMF_Player> feedbackDict = new Dictionary<T, MMF_Player>();

    protected virtual void Awake()
    {
        foreach (var entity in feedbackList)
        {
            if (entity.player != null)
            {
                feedbackDict.TryAdd(entity.type, entity.player);
            }
        }
    }

    public MMF_Player GetFeedback(T searchType)
    {
        if (feedbackDict.TryGetValue(searchType, out MMF_Player resultPlayer))
        {
            return resultPlayer;
        }
        
        Debug.LogWarning($"[FeedbackManager] not found {searchType} on {gameObject.name}");
        return null;
    }

    public void PlayFeedback(T type)
    {
        MMF_Player player = GetFeedback(type);
        if (player != null)
        {
            player.PlayFeedbacks();
        }
    }

    public void StopFeedback(T type)
    {
        MMF_Player player = GetFeedback(type);
        if (player != null)
        {
            player.StopFeedbacks();
        }
    }
}