using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] float speed;

    private Vector3 movingVelocity = Vector3.zero;


    void Update() {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.y = transform.position.y;
        transform.LookAt(worldPos, transform.up);


        if (Input.GetMouseButtonDown(0)){
            Vector3 movingDirection = (worldPos - transform.position).normalized;
            movingVelocity = movingDirection * speed;
        }
        else {
            movingVelocity = Vector3.zero;
        }
    }

    private void FixedUpdate(){
        if (movingVelocity != Vector3.zero) {
            rigidbody.velocity = movingVelocity;
        }
    }


}
