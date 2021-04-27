using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PixoEvent;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(PixoTrackableManager))]
public class PlaceOnPlane : MonoBehaviour, IEventHandler
{
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    public LegoManager legoManager;
    public TextMeshProUGUI output;

    ARRaycastManager m_RaycastManager;

    private ARPlaneManager _planeManager;
    private PixoTrackableManager trackableManager;
    private Vector3 placementPosition = Vector3.zero;
    private Quaternion placementRotation = Quaternion.identity;

    private GameObject previousPrefab;
    private bool previewing = true;
    private bool previewPlaced = false;

    private List<ARPlane> trackedPlanes;
    private Vector2 screenCenter = new Vector2(Screen.width / 2,Screen.height / 2);
    
    public GameObject previewPrefab;
    public GameObject singlePiecePrefab;
    public GameObject singlePiecePrefab2;
    private bool individualsSpawned = false;

    public GameObject placedPrefab
    {
        get { return legoManager.GetCurrentModel(); }
    }
    
    public GameObject spawnedObject { get; private set; }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        _planeManager = GetComponent<ARPlaneManager>();
        trackableManager = GetComponent<PixoTrackableManager>();
        trackedPlanes = new List<ARPlane>();

        trackableManager.trackablesChanged += trackableManagerOntrackablesChanged;
        legoManager.CurrentModelChanged += (s, a) => { PlaceModel(); };
        EventController.Subscribe(this);
    }

    private void trackableManagerOntrackablesChanged(PixoTrackablesChangedEventArgs obj)
    {
        foreach (var pixoTrackable in obj.added)
        {
            var plane = pixoTrackable.payload as ARPlane;
            
            if (plane != null && !trackedPlanes.Contains(plane))
                trackedPlanes.Add(plane);
        }

        foreach (var pixoTrackable in obj.removed)
        {
            var plane = pixoTrackable.payload as ARPlane;

            if (plane != null && trackedPlanes.Contains(plane))
                trackedPlanes.Remove(plane);
        }

        if (output != null)
            output.text = $"Tracked planes: {String.Join(", ", trackedPlanes)}";
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        touchPosition = default;
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;
            touchPosition = new Vector2(mousePosition.x, mousePosition.y);
        }
#else
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
        }
#endif

        if (Input.touchCount > 0 && !IsPointOverUIObject(touchPosition))
        {
            return true;
        }
        
        return false;
    }
    
    bool IsPointOverUIObject(Vector2 pos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return false;

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(pos.x, pos.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;

    }
    
    void Update()
    {
        if (previewing && previewPlaced)
        {
            m_RaycastManager.Raycast(screenCenter, s_Hits, TrackableType.PlaneWithinPolygon);
            
            var hitPose = s_Hits[0].pose;

            placementPosition = hitPose.position;
            placementRotation = hitPose.rotation;

            PlacePreviewModel();
        }
        
    }

    private void PlaceModel()
    {
        legoManager.buildPosition = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z);
        
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
        }

        StopPlaneVisualization();
        
        spawnedObject = Instantiate(placedPrefab, placementPosition, placementRotation);

        Transform existingModel = GameObject.FindWithTag("ARObject").transform;
        if (existingModel != null)
        {
            spawnedObject.transform.localScale = existingModel.localScale;
            spawnedObject.transform.rotation = existingModel.rotation;
        }

        if (!individualsSpawned)
        {
            SpawnIndividualLegos();
            EventController.Publish(new EventPayload
            {
                eventName = ScreenChangedHandler.ScreenChangedEventName,
                payload = "Active-LegoSteps"
            });
        }
        
    }

    private void SpawnIndividualLegos()
    {
        GameObject obj = Instantiate(singlePiecePrefab, new Vector3(placementPosition.x + 0.1f, placementPosition.y, placementPosition.z + 0.1f), placementRotation);
        legoManager.individualLegos.Insert(0, obj);
        
        GameObject obj2 = Instantiate(singlePiecePrefab2, new Vector3(placementPosition.x - 0.1f, placementPosition.y, placementPosition.z + 0.1f), placementRotation);
        legoManager.individualLegos.Insert(1, obj2);

        GameObject obj3 = Instantiate(singlePiecePrefab, new Vector3(placementPosition.x + 0.1f, placementPosition.y, placementPosition.z - 0.1f), placementRotation);
        legoManager.individualLegos.Insert(2, obj3);

        GameObject obj4 = Instantiate(singlePiecePrefab, new Vector3(placementPosition.x - 0.1f, placementPosition.y, placementPosition.z - 0.1f), placementRotation);
        legoManager.individualLegos.Insert(3, obj4);
        
        GameObject obj5 = Instantiate(singlePiecePrefab, new Vector3(placementPosition.x, placementPosition.y, placementPosition.z - 0.1f), placementRotation);
        legoManager.individualLegos.Insert(4, obj5);

        legoManager.individualLegos[0].GetComponent<IndividualPiece>().ToggleIsNext();
        legoManager.individualLegos[legoManager.individualLegos.Count - 1].GetComponent<IndividualPiece>().ToggleIsLast();

        individualsSpawned = true;
    }

    private void StopPlaneVisualization()
    {
        if (_planeManager != null)
        {
            _planeManager.planePrefab = null;

            foreach (var plane in _planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }

    private void PlacePreviewModel()
    {
        previewPlaced = true;
        
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
        }

        spawnedObject = Instantiate(previewPrefab, placementPosition, placementRotation);
        
        EventController.Publish(new EventPayload
        {
            eventName = ScreenChangedHandler.ScreenChangedEventName,
            payload = "Active-TapToPlace"
        });
    }

    public void TogglePreviewing()
    {
        previewing = !previewing;
        PlaceModel();
    }

    public void OnEvent(EventPayload p)
    {
        if (p.eventName == EventController.ButtonClickedEventName && p.payload == "PlacePreviewPiece")
        {
            var hitPlane = m_RaycastManager.Raycast(screenCenter, s_Hits, TrackableType.PlaneWithinPolygon);

            if (hitPlane)
            {
                placementPosition = s_Hits[0].pose.position;
                placementRotation = s_Hits[0].pose.rotation;
                
                if (!previewPlaced)
                    PlacePreviewModel();
            }
        }
    }
}