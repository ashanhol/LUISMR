using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class LuisBehavior : MonoBehaviour {

    [Serializable]
    public struct IntentActions
    {
        [Tooltip("The primary intent to match.")]
        public string PrimaryIntent;

        [Tooltip("The entity type to capture.")]
        public string EntityType;

        [Tooltip("The handler to be invoked.")]
        public LuisEvent Response;
    }

    [Tooltip("Identify game object by using a uniquely named entity.")]
    public string EntityType;
    public string EntityId;

    /// <summary>
    /// The entities to be recognized
    /// </summary>
    [Tooltip("The entities to be recognized.")]
    public IntentActions[] ActionableEntities;

    private void Awake() {  
    }

    // Use this for initialization
    void Start () {
        RegisterManager();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDestroy()
    {
        UnregisterManager();
    }

    private void RegisterManager()
    {
        if (LuisManager.Instance == null)
        {
            Debug.LogError("requires luis manager");
            return;
        }
        LuisManager.Instance.AddLuisObject(gameObject);
    }

    private void UnregisterManager()
    {
        if (LuisManager.Instance == null)
        {
            return;
        }
        LuisManager.Instance.RemoveLuisObject(gameObject);
    }
}
