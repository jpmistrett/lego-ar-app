using System;
using System.Collections.Generic;
using System.Linq;
using PixoEvent;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(PixoTrackableManager))]
public class ModelPositionController : MonoBehaviour
{
    private Camera _mainCamera;

    private PixoTrackableManager _trackableManager;
    private List<string> _trackables;

    public GameObject lego;
    public Vector3 PositionAOffsets = new Vector3(0, 0, 2);
    public float PositionARotation = 0;

    public Vector3 PositionBOffsets = new Vector3(0, 0, 2);
    public float PositionBRotation = 0;

    public Text trackedObjectLabel;


    void Awake()
    {
        _trackableManager = GetComponent<PixoTrackableManager>();
        _trackables = new List<string>();
    }

    void OnEnable()
    {
        _trackableManager.trackablesChanged += OnTrackablesChanged;
    }

    void OnDisable()
    {
        _trackableManager.trackablesChanged -= OnTrackablesChanged;
    }

    private void Update()
    {
        var tracked = _trackables.Count == 0 ? "None" : String.Join(", ", _trackables);
        if (null != trackedObjectLabel)
            trackedObjectLabel.text = $"Tracked object(s): {tracked}";
    }

    void OnTrackablesChanged(PixoTrackablesChangedEventArgs eventArgs)
    {
        foreach (var trackable in eventArgs.added)
        {
            _trackables.Add(trackable.name);

            MovePlacedObject(trackable);
        }

        foreach (var trackable in eventArgs.updated)
        {
            MovePlacedObject(trackable);
        }

        foreach (var trackable in eventArgs.removed)
        {
            // TODO: Are we getting here?
            _trackables.Remove(trackable.name);
        }
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void MovePlacedObject(PixoTrackable trackable)
    {
        if (trackable.name == "qr-matt")
        {
            // RepositionLego(trackable, PositionAOffsets, GetDeviceOffset("A"), PositionARotation,
            // GetDeviceRotationOffset("A"));

            EventController.Publish(new EventPayload
            {
                eventName = "Placement Object Moved",
                trackable = trackable,
            });
        }
        else if (new string[] {"1", "2", "3", "4"}.Contains(trackable.name))
        {
            EventController.Publish(new EventPayload
            {
                eventName = "StepFound",
                payload = trackable.name,
            });
            EventController.Publish(new EventPayload
            {
                eventName = "Placement Object Moved",
                trackable = trackable,
            });
        }
    }

    private Vector3 GetDeviceOffset(string AorB)
    {
        return new Vector3(PlayerPrefs.GetFloat("Offset-" + AorB + "-X", 0),
            PlayerPrefs.GetFloat("Offset-" + AorB + "-Y", 0), PlayerPrefs.GetFloat("Offset-" + AorB + "-Z", 0));
    }

    private Vector3 GetDeviceRotationOffset(string AorB)
    {
        return new Vector3(0, PlayerPrefs.GetFloat("Offset-" + AorB + "-Rotate", 0), 0);
    }

    private void RepositionLego(PixoTrackable trackable, Vector3 offset, Vector3 deviceOffset, float rotation,
        Vector3 deviceRotationOffset)
    {
        var distanceFromCamera = trackable.transform.position - _mainCamera.transform.position;

        // TODO: Do we want to do something will pulling away from the target after initial placement?

        var imageTransform = trackable.transform.position;
        lego.transform.position = imageTransform;
        lego.transform.rotation = trackable.transform.rotation;
    }
}