using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAppearance : MonoBehaviour
{
    public List<Transform> myAvailablePosition;
    public GameObject myCat;

    void Awake()
    {
        if (myAvailablePosition.Count > 0)
        {
            myCat.SetActive(true);
            int randomPosition = Random.Range(0, myAvailablePosition.Count - 1);
            Debug.Log(randomPosition);
            myCat.transform.position = myAvailablePosition[randomPosition].transform.position;
        }
        else
        {
            myCat.SetActive(false);
        }
    }

}
