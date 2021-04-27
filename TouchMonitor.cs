using System;
using UnityEngine;
using UnityEngine.UI;

public class TouchMonitor : MonoBehaviour
{
    public float rotatespeed = 150f;
    public float zoomSpeed = 0.75f;
    public float minZoom = 1, maxZoom = 10;
    private float startingPosition;
    private Transform model;

    private bool canRaycast = true;
    private float timeRemaining;
    
    // Update is called once per frame
    void Update()
    {
        CheckForTouch();
        
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            canRaycast = true;
        }
    }

    private void CheckForTouch()
    {
        if (Input.touchCount == 1)
        {
            if (canRaycast)
            {
                canRaycast = false;
                timeRemaining = 0.5f;

                RaycastHit hit;
                
                Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                
                if (Physics.Raycast(raycast, out hit))
                {
                    hit.collider.gameObject.GetComponent<IndividualPiece>().Interact();
                }
            }

            model = GameObject.FindWithTag("ARObject").transform;
            
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startingPosition = touch.position.x;
                    break;
                case TouchPhase.Moved:
                    if (startingPosition > touch.position.x)
                    {
                        model.transform.Rotate(Vector3.up, -rotatespeed * Time.deltaTime);
                    }
                    else if (startingPosition < touch.position.x)
                    {
                        model.transform.Rotate(Vector3.up, rotatespeed * Time.deltaTime);
                    }
                    break;
                case TouchPhase.Ended:
                    break;
            }
        }

        if (Input.touchCount == 2)
        {
            model = GameObject.FindWithTag("ARObject").transform;

            Touch touchOne = Input.GetTouch(0);
            Touch touchTwo = Input.GetTouch(1);
            
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            Vector2 touchTwoPrevPos = touchTwo.position - touchTwo.deltaPosition;
            
            float prevTouchDeltaMag = (touchOnePrevPos - touchTwoPrevPos).magnitude;
            float touchDeltaMag = (touchOne.position - touchTwo.position).magnitude;
            
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            float pinchAmount = deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;
            
            Vector3 newScale = new Vector3();
            newScale.x = Mathf.Clamp(model.localScale.x + pinchAmount, minZoom, maxZoom);
            newScale.y = Mathf.Clamp(model.localScale.y + pinchAmount, minZoom, maxZoom);
            newScale.z = Mathf.Clamp(model.localScale.z + pinchAmount, minZoom, maxZoom);
            model.transform.localScale = newScale;
        }
    }
}
