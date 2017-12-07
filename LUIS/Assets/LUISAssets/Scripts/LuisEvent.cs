using Microsoft.Cognitive.LUIS;
using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LuisEvent : UnityEvent<string, string, LuisResult, GameObject>
{
    // entity identity name
    // entity property value
    // full luis result
    // game object
}