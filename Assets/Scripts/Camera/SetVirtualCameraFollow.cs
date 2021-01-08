using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class SetVirtualCameraFollow : MonoBehaviour
{
    private CinemachineVirtualCamera _cam;

    private void Start()
    {
        _cam = GetComponent<CinemachineVirtualCamera>();
        _cam.Follow = PlayerMovement.Instance.transform;
    }
}
