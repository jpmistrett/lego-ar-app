using System;
using System.Collections;
using System.Collections.Generic;
using PixoEvent;
using UnityEngine;

public class ModelLoader : MonoBehaviour, IEventHandler
{
    public LegoManager legoManager;
    public GameObject spawnedObject;

    public GameObject placedPrefab
    {
        get { return legoManager.GetCurrentModel(); }
    }

    public void OnEvent(EventPayload p)
    {
        Debug.Log("OMG I got a message : " + p.eventName);
        if (p.eventName == "StepFound")
        {
            // TODO: don't just assume it's an integer
            //legoManager.StepTo(Int32.Parse(p.payload));
            PlaceModel();
        }

        if (p.eventName == "Placement Object Moved")
        {
            var transform = p.trackable.transform;
            PlaceModel(transform.position, transform.rotation);
        }
    }

    void Start()
    {
        EventController.Subscribe(this);
    }

    private void PlaceModel(Vector3 position, Quaternion rotation)
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
        }

        spawnedObject = Instantiate(placedPrefab, position, rotation);
    }

    private void PlaceModel()
    {
        var position = new Vector3(0, 0, 0);
        var rotation = new Quaternion(0, 0, 0, 0);

        if (spawnedObject != null)
        {
            position = spawnedObject.transform.position;
            rotation = spawnedObject.transform.rotation;
        }

        PlaceModel(position, rotation);
    }
}