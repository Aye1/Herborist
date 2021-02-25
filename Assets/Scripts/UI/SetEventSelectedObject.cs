using UnityEngine;
using Sirenix.OdinInspector;

public class SetEventSelectedObject : MonoBehaviour
{
    [Required]
    public GameObject objectToFocus;

    private void OnBecameVisible()
    {
        NavigationManager.Instance.SetFocus(objectToFocus);
    }

    void OnEnable()
    {
        NavigationManager.Instance.SetFocus(objectToFocus);
    }
}
