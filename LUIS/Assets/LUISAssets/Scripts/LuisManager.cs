using HoloToolkit.Unity;
using Microsoft.Cognitive.LUIS;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuisManager : Singleton<LuisManager> {

    
    private List<GameObject> luisObjects;

    #region MonoBehaviour Implementation

    protected override void Awake()
    {
        base.Awake();
        
    }


    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    #endregion
    
    public void ProcessResult(LuisResult res)
    {
        Debug.Log("process LUIS result for intent: " + res.TopScoringIntent.Name + " targets:" + luisObjects.Count) ;

        if (luisObjects == null && luisObjects.Count == 0)
        {
            return;
        }

        foreach (GameObject obj in luisObjects) {

            LuisBehavior script = (LuisBehavior)obj.GetComponent<LuisBehavior>();
            IList<Entity> entities = new List<Entity>();

            string id = "";
            string entitySelector = script.EntityType;

            if (res.Entities.TryGetValue(entitySelector, out entities))
            {
                id = entities[0].Value;
            }
            else
            {
                Debug.LogWarning("Entity identity is missing");
                return;
            }

            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("Entity identity cannot be empty");
                return;
            }

            if (!id.Equals(script.EntityId))
            {
                continue;
            }

            //List<string> entitiesNames = new List<string>();
            //foreach (Entity entity in res.GetAllEntities())
            //{
            //    entitiesNames.Add(entity.Name);
            //}

            // loop through intentActions and invoke event with entity value.
            foreach (var intentAction in script.ActionableEntities)
            {
                // check for primary intent match for each action
                if (String.Equals(res.TopScoringIntent.Name, intentAction.PrimaryIntent, StringComparison.OrdinalIgnoreCase) )
                {
                    entities = new List<Entity>();
                    if (res.Entities.TryGetValue(intentAction.EntityType, out entities))
                    {
                        string entityProperty = entities[0].Value;
                        // Invoke Unity event: LuisEvent with entityId, entityProperty, LuisResult, GameObject
                        intentAction.Response.Invoke(id, entityProperty, res, obj);
                    }
                }
            }
            
        }
        
    }

    public void AddLuisObject(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("Failed to add Luis object");
            return;
        }
        if (luisObjects == null)
        {
            Debug.Log("objects init!");
            luisObjects = new List<GameObject>();
        }
        Debug.Log("objects:" + luisObjects.Count);
        luisObjects.Add(obj);
    }

    public void RemoveLuisObject(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("Failed to remove Luis object");
            return;
        }
        luisObjects.Remove(obj);
    }
}
