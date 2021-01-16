using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

public class SetEventSelectedObject : MonoBehaviour
{
    [Required]
    public GameObject objectToFocus;

    private void OnBecameVisible()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(objectToFocus);
    }

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(objectToFocus);
    }
}
