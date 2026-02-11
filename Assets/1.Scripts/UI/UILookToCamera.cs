using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookToCamera : MonoBehaviour
{
    #region Parameters
    private Camera mainCamera;
    bool look = false;
    #endregion
    #region Properties

    #endregion
    #region MonoBehaviour Methods
    private void OnEnable()
    {
        look = true;
    }
    private void OnDisable()
    {
        look = false;
    }
    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null) Debug.Log("TTTTTTTT");
    }
    #endregion
    #region My Methods
    private void Update()
    {
        if (mainCamera == null)
            return;
        if (look)
        {
            //transform.LookAt(mainCamera.transform);
            Vector3 dir = transform.position - mainCamera.transform.position;

            transform.rotation = Quaternion.LookRotation(
                dir,
                mainCamera.transform.up   // 👈 quan trọng
            );
        }
    }
    //void LateUpdate()
    //{
    //    if (mainCamera == null) return;

    //    // UI nhìn theo hướng camera, không bị nghiêng
    //    if (look) transform.forward = mainCamera.transform.forward;
    //}
    #endregion
}
