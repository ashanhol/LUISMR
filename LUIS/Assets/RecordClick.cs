using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.InputModule.Tests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordClick : MonoBehaviour, IInputClickHandler{

    public DictationRecordButton recordbutton;

    [SerializeField]
    private GameObject recordLight;

    private Renderer buttonRenderer;

    void Awake()
    {
        buttonRenderer = GetComponent<Renderer>();

    }

    public void OnInputClicked(InputClickedEventData eventData)
    {

        if (recordbutton.isRecording)
        {
            recordbutton.StopRecordingDictation();
            recordLight.SetActive(false);
            buttonRenderer.enabled = true;

        }
        else
        {
            recordbutton.StartRecordingDictation();
            recordLight.SetActive(true);
            buttonRenderer.enabled = false;


        }
    }

}
