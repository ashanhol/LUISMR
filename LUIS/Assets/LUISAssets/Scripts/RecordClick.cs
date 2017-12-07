using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.InputModule.Tests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordClick : MonoBehaviour, IInputClickHandler{

    public DictationToLUIS dictation;

    [SerializeField]
    private GameObject recordLight;

    private Renderer buttonRenderer;

    void Awake()
    {
        buttonRenderer = GetComponent<Renderer>();

    }

    public void OnInputClicked(InputClickedEventData eventData)
    {

        if (dictation.IsRecording)
        {
            dictation.StopRecordingDictation();
            recordLight.SetActive(false);
            buttonRenderer.enabled = true;

        }
        else
        {
            dictation.StartRecordingDictation();
            recordLight.SetActive(true);
            buttonRenderer.enabled = false;
        }
    }

}
