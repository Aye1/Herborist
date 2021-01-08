using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DynamicUIManager : MonoBehaviour
{
    public static DynamicUIManager Instance { get; private set; }

    private PlayerMovement _player;
    [SerializeField, Required, AssetsOnly] private CollectiblePicked _collectiblePickedUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        _player = PlayerMovement.Instance;
    }

    public void SpawnCollectiblePicked(CollectiblePackage package)
    {
        CollectiblePicked newPickedUI = Instantiate(_collectiblePickedUI, transform);
        newPickedUI.transform.position = Camera.main.WorldToScreenPoint(_player.transform.position);
        _collectiblePickedUI.Package = package;
        UIFollowPlayer following = newPickedUI.GetComponent<UIFollowPlayer>();
        if (following != null)
        {
            following.player = _player;
            following.useOffset = true;
        }
    }

    public void SpawnCollectiblePicked(List<CollectiblePackage> packages)
    {
        packages.ForEach(p => SpawnCollectiblePicked(p));   
    }
}
