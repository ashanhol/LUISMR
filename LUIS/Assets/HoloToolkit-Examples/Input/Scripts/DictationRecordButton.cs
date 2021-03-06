﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class DictationRecordButton : MonoBehaviour
    {
        [SerializeField]
        [Range(0.1f, 5f)]
        [Tooltip("The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.")]
        private float initialSilenceTimeout = 5f;

        [SerializeField]
        [Range(5f, 60f)]
        [Tooltip("The time length in seconds before dictation recognizer session ends due to lack of audio input.")]
        private float autoSilenceTimeout = 20f;

        [SerializeField]
        [Range(1, 60)]
        [Tooltip("Length in seconds for the manager to listen.")]
        private int recordingTime = 10;

        [SerializeField]
        private TextMesh speechToTextOutput;

        [SerializeField]
        private LuisRequest LUIS; 


        public bool isRecording { get; set; }

        /// <summary>
        /// Caches the text currently being displayed in dictation display text.
        /// </summary>
        private StringBuilder textSoFar;

        /// <summary>
        /// <remarks>Using an empty string specifies the default microphone.</remarks>
        /// </summary>
        private readonly string DeviceName = string.Empty;

        /// <summary>
        /// The device audio sampling rate.
        /// <remarks>Set by UnityEngine.Microphone.<see cref="Microphone.GetDeviceCaps"/></remarks>
        /// </summary>
        private int samplingRate;

        public bool IsListening { get; private set; }
        private bool isTransitioning;
        private bool hasFailed;

        /// <summary>
        /// String result of the current dictation.
        /// </summary>
        private string dictationResult;

        private DictationRecognizer dictationRecognizer;

        private void Awake()
        {
            isRecording = false;
            // Query the maximum frequency of the default microphone.
            int minSamplingRate; // Not used.
            Microphone.GetDeviceCaps(DeviceName, out minSamplingRate, out samplingRate);

            dictationRecognizer = new DictationRecognizer();
            dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
            dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
            dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
            dictationRecognizer.DictationError += DictationRecognizer_DictationError;

        }

        private void LateUpdate()
        {
            if (IsListening && !Microphone.IsRecording(DeviceName) && dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                // If the microphone stops as a result of timing out, make sure to manually stop the dictation recognizer.
                StartCoroutine(StopRecording());
                isRecording = false;
            }

        }

        private void OnDestroy()
        {
            dictationRecognizer.Dispose();

        }


        /// <summary>
        /// Turns on the dictation recognizer and begins recording audio from the default microphone.
        /// </summary>
        /// <param name="initialSilenceTimeout">The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.</param>
        /// <param name="autoSilenceTimeout">The time length in seconds before dictation recognizer session ends due to lack of audio input.</param>
        /// <param name="recordingTime">Length in seconds for the manager to listen.</param>
        /// <returns></returns>
        public IEnumerator StartRecording(float initialSilenceTimeout = 5f, float autoSilenceTimeout = 20f, int recordingTime = 10)
        {
#if UNITY_WSA || UNITY_STANDALONE_WIN
            if (IsListening || isTransitioning)
            {
                Debug.LogWarning("Unable to start recording");
                yield break;
            }

            IsListening = true;
            isTransitioning = true;

            if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
            {
                PhraseRecognitionSystem.Shutdown();
            }

            while (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
            {
                yield return null;
            }

            dictationRecognizer.InitialSilenceTimeoutSeconds = initialSilenceTimeout;
            dictationRecognizer.AutoSilenceTimeoutSeconds = autoSilenceTimeout;
            dictationRecognizer.Start();

            while (dictationRecognizer.Status == SpeechSystemStatus.Stopped)
            {
                yield return null;
            }

            // Start recording from the microphone.
            Microphone.Start(DeviceName, false, recordingTime, samplingRate);
            textSoFar = new StringBuilder();
            isTransitioning = false;
#else
            return null;
#endif
        }

        /// <summary>
        /// Ends the recording session.
        /// </summary>
        public IEnumerator StopRecording()
        {
#if UNITY_WSA || UNITY_STANDALONE_WIN
            if (!IsListening || isTransitioning)
            {
                Debug.LogWarning("Unable to stop recording");
                yield break;
            }

            IsListening = false;
            isTransitioning = true;

            Microphone.End(DeviceName);

            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
            }

            while (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                yield return null;
            }

            PhraseRecognitionSystem.Restart();
            isTransitioning = false;
#else
            return null;
#endif
        }

        
#if UNITY_WSA || UNITY_STANDALONE_WIN

        /// <summary>
        /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
        /// </summary>
        /// <param name="text">The currently hypothesized recognition.</param>
        private void DictationRecognizer_DictationHypothesis(string text)
        {
            // We don't want to append to textSoFar yet, because the hypothesis may have changed on the next event.
            dictationResult = textSoFar.ToString() + " " + text + "...";
            speechToTextOutput.text = text;
        }

        /// <summary>
        /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
        /// </summary>
        /// <param name="text">The text that was heard by the recognizer.</param>
        /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
        private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
        {
            textSoFar.Append(text + ". ");

            dictationResult = textSoFar.ToString();

            speechToTextOutput.text = text;
            LUIS.GetLuisPrediction(text);

        }

        /// <summary>
        /// This event is fired when the recognizer stops, whether from StartRecording() being called, a timeout occurring, or some other error.
        /// Typically, this will simply return "Complete". In this case, we check to see if the recognizer timed out.
        /// </summary>
        /// <param name="cause">An enumerated reason for the session completing.</param>
        private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
        {
            // If Timeout occurs, the user has been silent for too long.
            if (cause == DictationCompletionCause.TimeoutExceeded)
            {
                Microphone.End(DeviceName);

                dictationResult = "Dictation has timed out. Please try again.";
            }


            textSoFar = null;
            dictationResult = string.Empty;
        }

        /// <summary>
        /// This event is fired when an error occurs.
        /// </summary>
        /// <param name="error">The string representation of the error reason.</param>
        /// <param name="hresult">The int representation of the hresult.</param>
        private void DictationRecognizer_DictationError(string error, int hresult)
        {
            dictationResult = error + "\nHRESULT: " + hresult.ToString();

            textSoFar = null;
            dictationResult = string.Empty;

            speechToTextOutput.color = Color.red;
   //         buttonRenderer.enabled = true;
           // recordLight.SetActive(false);
            speechToTextOutput.text = error;
            Debug.LogError(error);
            StartCoroutine(StopRecording());

        }
#endif
        public void StartRecordingDictation()
        {
            if (isTransitioning)
                return;

            else
            {
                StartCoroutine(StartRecording(initialSilenceTimeout, autoSilenceTimeout, recordingTime));
                isRecording = true;
                speechToTextOutput.color = Color.green;
            }
        }

        public void StopRecordingDictation()
        {
            if (isTransitioning)
                return;
            
            else
            {
                StartCoroutine(StopRecording());
                isRecording = false;
                speechToTextOutput.color = Color.white;
            }
        }
/*
        private void ToggleRecording()
        {
            if (isTransitioning)
                return;

            if (IsListening)
            {
                StartCoroutine(StopRecording());
                speechToTextOutput.color = Color.white;
                buttonRenderer.enabled = true;
                recordLight.SetActive(false);
            }
            else
            {
                StartCoroutine(StartRecording(initialSilenceTimeout, autoSilenceTimeout, recordingTime));
                speechToTextOutput.color = Color.green;
                recordLight.SetActive(true);
                buttonRenderer.enabled = false;
            }
        }

    */
    }
}
