using Microsoft.Cognitive.LUIS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneScript : MonoBehaviour {

    public void ChangeColorHandler(string id, string color, LuisResult result, GameObject go)
    {
        Color fromstring;
        ColorUtility.TryParseHtmlString(color, out fromstring);
        gameObject.GetComponent<Renderer>().material.color = fromstring;
    }

    public void ChangeSizeHandler(string id, string size, LuisResult result, GameObject go)
    {
        switch (size)
        {
            case "small":
                gameObject.transform.localScale = new Vector3(.25f, .25f, .25f);
                break;
            case "medium":
                gameObject.transform.localScale = new Vector3(.5f, .5f, .5f);
                break;
            case "large":
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                break;
        }
    }


}
