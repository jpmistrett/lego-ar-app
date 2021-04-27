using System;
using System.Collections;
using System.Collections.Generic;
using PixoEvent;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LegoManager : MonoBehaviour
{
    private int currentModelIndex = 0;
    private int currentIndividualLegoIndex = 0;
    public GameObject buildCompleteEffect;
    public GameObject[] legoModels;
    [HideInInspector]
    public List<GameObject> individualLegos;
    public GameObject legoPieceBuildPrompt;
    public Vector3 buildPosition;

    //remove these
    public GameObject finalStepObject, finalStepObjectToHide;

    public event EventHandler CurrentModelChanged;

    public GameObject GetCurrentModel()
    {
        return legoModels[currentModelIndex];
    }

    public int CurrentStep()
    {
        return currentModelIndex;
    }

    public void StepModelForward()
    {
        legoPieceBuildPrompt.SetActive(false);

        individualLegos[currentIndividualLegoIndex].GetComponent<IndividualPiece>().Remove();
        currentModelIndex++;
        currentIndividualLegoIndex++;
        
        individualLegos[currentIndividualLegoIndex].GetComponent<IndividualPiece>().ToggleIsNext();

        Handheld.Vibrate();
        CurrentModelChanged(null, null);
    }

    public void EndBuild()
    {
        var fx = Instantiate(buildCompleteEffect, buildPosition, Quaternion.Euler(-90f, 0f, 0f));
        fx.transform.localScale = new Vector3(0.05f,0.05f,0.05f);

        individualLegos[currentIndividualLegoIndex].GetComponent<IndividualPiece>().Remove();
        
        Handheld.Vibrate();
        
        EventController.Publish(new EventPayload
        {
            eventName = ScreenChangedHandler.ScreenChangedEventName,
            payload = "Congrats"
        });
        
        CurrentModelChanged(null, null);
    }
}