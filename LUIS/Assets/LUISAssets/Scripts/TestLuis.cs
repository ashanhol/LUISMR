using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Cognitive.LUIS;

public static class TestLuis {

    public static LuisResult GetLuisResult(string primaryIntent = "Change Color", string entityIdType = "Shapes", string entityId = "box", string entityPropertyType = "Colors", string entityProperty = "red")
    {
        LuisResult test = new LuisResult();

        Intent intent = new Intent
        {
            Name = primaryIntent,
            Score = 0.96
        };

        test.TopScoringIntent = intent;

        Dictionary<string, object> resolutionObject = new Dictionary<string, object>();
        resolutionObject.Add("values", entityId);

        Dictionary<string, object> resolutionProperty = new Dictionary<string, object>();
        resolutionProperty.Add("values", entityProperty);

        Entity addEntityId = new Entity
        {
            Name = entityIdType,
            Resolution = resolutionObject,
            StartIndex = 7,
            EndIndex = 9,
            Value = entityId,
            Score = -1
        };

        Entity addEntityProperty = new Entity
        {
            Name = entityPropertyType,
            Resolution = resolutionProperty,
            StartIndex = 20,
            EndIndex = 22,
            Value = entityProperty,
            Score = -1
        };

        test.Entities = new Dictionary<string, IList<Entity>>();
        test.Entities.Add(entityIdType, new List<Entity> { addEntityId });
        test.Entities.Add(entityPropertyType, new List<Entity> { addEntityProperty });

        List<Intent> intents = new List<Intent>();

        test.Intents = intents.ToArray();

        return test;
    }

}
