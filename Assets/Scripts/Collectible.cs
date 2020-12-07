using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MoreMountains.Feedbacks;

public struct CollectiblePackage
{
    public Type type;
    public int count;
}

public class Collectible : MonoBehaviour, IInteractable
{
    public int collectibleCount = 25;
    public MMFeedbacks feedback;

    public void Interact(GameObject aPLayer)
    {
        feedback.PlayFeedbacks();
        aPLayer.GetComponent<Inventory>().Add(GetCollectibles());
    }

    public bool CanInteract()
    {
        return collectibleCount > 0;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public List<CollectiblePackage> GetCollectibles()
    {
        CollectiblePackage pck;
        pck.type = typeof(Collectible);
        pck.count = Mathf.Min(4, collectibleCount);
        collectibleCount -= pck.count;

        List<CollectiblePackage> res = new List<CollectiblePackage>() {
                pck
            };
        return res;
    }
}
