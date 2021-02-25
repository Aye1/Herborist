using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using Sirenix.OdinInspector;

public class PlayerInteractionManager : MonoBehaviour
{
    private List<IInteractable> myInteractables;
    private GameObject _currentInteractable;

    public GameObject CurrentInteractable
    {
        get { return _currentInteractable; }
        set
        {
            if(value != _currentInteractable)
            {
                if (_currentInteractable != null)
                {
                    SpriteChanger.Instance.Outline(_currentInteractable, false);
                }
                _currentInteractable = value;
                if (_currentInteractable != null)
                {
                    SpriteChanger.Instance.Outline(_currentInteractable, true);
                }
            }
        }
    }

    public bool CanInteract
    {
        get { return CurrentInteractable != null
                && !NavigationManager.Instance.IsPopupOpen
                && !GameManager.Instance.IsInPause; }
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneSwitcher.Instance.OnSceneWillLoad += OnSceneChange;
        myInteractables = new List<IInteractable>();
    }

    private void Update()
    {
        RefreshCurrentInteractable();
    }

    private void OnDestroy()
    {
        SceneSwitcher.Instance.OnSceneWillLoad -= OnSceneChange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.gameObject.GetComponent<IInteractable>();

        if (interactable != null && myInteractables.Contains(interactable) == false)
        {
            myInteractables.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.gameObject.GetComponent<IInteractable>();

        if (interactable != null && myInteractables.Contains(interactable))
        {
            myInteractables.Remove(interactable);
        }
    }

    public void OnInteract()
    {
        if (!GameManager.Instance.IsInPause)
        {
            IInteractable currentInteractable = GetCurrentInteractable();
            if (currentInteractable != null)
            {
                currentInteractable.Interact(this.gameObject);
            }
        }
    }

    private void RefreshCurrentInteractable()
    {
        if(myInteractables == null || myInteractables.Count == 0)
        {
            CurrentInteractable = null;
        }  else
        {
            IEnumerable<IInteractable> possibleInteractables = myInteractables.Where(i => i.CanInteract());
            CurrentInteractable = possibleInteractables.Count() > 0  ? possibleInteractables.OrderBy(x => Vector3.Distance(x.GetGameObject().transform.position, transform.position)).First().GetGameObject() : null;
        }
    }

    public IInteractable GetCurrentInteractable()
    { 
        return CurrentInteractable == null ? null : CurrentInteractable.GetComponent<IInteractable>();
    }

    private void OnSceneChange(SceneType newScene)
    {
        Clean();
    }

    private void Clean()
    {
        CurrentInteractable = null;
        myInteractables.Clear();
    }
}
