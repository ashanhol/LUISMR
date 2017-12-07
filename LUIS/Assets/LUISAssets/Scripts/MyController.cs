using Microsoft.Cognitive.LUIS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MyController : MonoBehaviour
{
    

    [SerializeField]
    private Dropdown dropdownShape;

    [SerializeField]
    private Dropdown dropdownColor;

    [SerializeField]
    private Dropdown dropdownSize;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ClickChangeColorTest()
    {
        string primaryIntent = "Change Color";
        string entityPropertyType = "Colors";
        string entityProperty = dropdownColor.options[dropdownColor.value].text;

        string entityIdType = "Shapes";
        string entityId = dropdownShape.options[dropdownShape.value].text;

        GenerateLuisResult(primaryIntent, entityIdType, entityId, entityPropertyType, entityProperty);
    }

    public void ClickChangeSizeTest()
    {
        string primaryIntent = "Change Size";
        string entityPropertyType = "Sizes";
        string entityProperty = dropdownSize.options[dropdownSize.value].text;

        string entityIdType = "Shapes";
        string entityId = dropdownShape.options[dropdownShape.value].text;

        GenerateLuisResult(primaryIntent, entityIdType, entityId, entityPropertyType, entityProperty);
    }

    private void GenerateLuisResult(string primaryIntent = "Change Color", string entityIdType = "Shapes", string entityId = "box", string entityPropertyType = "Colors", string entityProperty = "red")
    {
        if (LuisManager.Instance == null)
        {
            Debug.LogWarning("No Luis Manager");
            return;
        }

        Debug.Log("Selected object entity id: " + entityId + "  entity property: " + entityProperty + "  entity type: " + entityPropertyType);

        LuisResult result = TestLuis.GetLuisResult(primaryIntent, entityIdType, entityId, entityPropertyType, entityProperty);
        LuisManager.Instance.ProcessResult(result);
    }

    public void HandleChangeColor(string entityId, string entityProperty, LuisResult result, GameObject obj)
    {
        Debug.Log("* my color: " + entityId + " color: " + entityProperty + " gameObject:" + obj.name);
    }

    public void HandleChangeLocation(string entityId, string entityProperty, LuisResult result, GameObject obj)
    {
        Debug.Log("* my location: " + entityId + " location: " + entityProperty);
    }

    public void HandleChangeSize(string entityId, string entityProperty, LuisResult result, GameObject obj)
    {
        Debug.Log("* my size: " + entityId + " size: " + entityProperty);
    }

}