using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualPiece : MonoBehaviour
{
    public bool isNext { get; private set; }
    
    public float degreesPerSecond = 20.0f;
    public float amplitude = 0.025f;
    public float frequency = 1f;
    private bool rotateAndBob, isLast;

    public GameObject testSparkleEffect, testClickEffect;
    private GameObject fx;
    private LegoManager legoManager;
 
    Vector3 posOffset, tempPos;
 
    void Start () {
        legoManager = GameObject.Find("LEGO Controller").GetComponent<LegoManager>();
        posOffset = transform.position;
        isNext = false;
    }
     
    void Update () 
    {
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
            
        if (rotateAndBob)
        {
            tempPos = posOffset;
            tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

            transform.position = tempPos;
        }
    }

    public void ToggleIsNext()
    {
        fx = Instantiate(testSparkleEffect, transform.position, Quaternion.identity);
        fx.transform.localScale = new Vector3(0.05f,0.05f,0.05f);
        rotateAndBob = true;
        isNext = true;

    }
    
    public void ToggleIsLast()
    {
        isLast = true;
    }

    public void Interact()
    {
        if (rotateAndBob)
        {
            if (isLast)
            {
                legoManager.EndBuild();
            }
            else
            {
                legoManager.StepModelForward();
            }
        }
    }

    public void Remove()
    {
        var obj = Instantiate(testClickEffect, transform.position, Quaternion.identity);
        obj.transform.localScale = new Vector3(0.05f,0.05f,0.05f);
        Destroy(fx);
        Destroy(gameObject);
    }
}
