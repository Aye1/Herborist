using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;

public struct CollectiblePackage
{
    public CollectibleScriptableObject type;
    public int count;
}

public class Collectible : MonoBehaviour, IInteractable
{
    [ReadOnly, ShowInInspector]
    private int _collectibleCount;
    [Required]
    public CollectibleScriptableObject collectible;
    [Required]
    public MMFeedbacks feedback;

    private void Awake()
    {
        _collectibleCount = Alea.GetIntInc(collectible.quantity.x, collectible.quantity.y);
    }

    public void Interact(GameObject aPLayer)
    {
        feedback.PlayFeedbacks();
        aPLayer.GetComponent<Inventory>().Add(GetCollectibles());
    }

    public bool CanInteract()
    {
        return _collectibleCount > 0;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public List<CollectiblePackage> GetCollectibles()
    {
        CollectiblePackage pck;
        pck.type = collectible;
        int randomPickQuantity = Alea.GetIntInc(collectible.handGatherQuantity.x, collectible.handGatherQuantity.y);
        pck.count = Mathf.Min(randomPickQuantity, _collectibleCount);
        _collectibleCount -= pck.count;

        List<CollectiblePackage> res = new List<CollectiblePackage>() {
                pck
            };
        return res;
    }
}
