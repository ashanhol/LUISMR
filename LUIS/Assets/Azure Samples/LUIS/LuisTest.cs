using Microsoft.Cognitive.LUIS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
#if !UNITY_WSA || UNITY_EDITOR

using System.Security.Cryptography.X509Certificates;

#endif
using System.Threading.Tasks;
using UnityEngine;

public class LuisTest : MonoBehaviour
{
    public string AppId;
    public string AppKey;
    public bool Verbose = true;
    public string Domain = "westus";
    [SerializeField]
  //  private TextMesh luisOutput;


    // Use this for initialization
    void Awake()
    {

#if !UNITY_WSA || UNITY_EDITOR
        //This works, and one of these two options are required as Unity's TLS (SSL) support doesn't currently work like .NET
        //ServicePointManager.CertificatePolicy = new CustomCertificatePolicy();

        //This 'workaround' seems to work for the .NET Storage SDK, but not event hubs. 
        //If you need all of it (ex Storage, event hubs,and app insights) then consider using the above.
        //If you don't want to check an SSL certificate coming back, simply use the return true delegate below.
        //Also it may help to use non-ssl URIs if you have the ability to, until Unity fixes the issue (which may be fixed by the time you read this)
        ServicePointManager.ServerCertificateValidationCallback = CheckValidCertificateCallback; //delegate { return true; };
#endif
    }

#if !UNITY_WSA || UNITY_EDITOR
    public bool CheckValidCertificateCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool valid = true;

        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        valid = false;
                    }
                }
            }
        }
        return valid;
    }
#endif

    public void GetLuisPrediction(string LuisInput)
    {
        Task.Run(()=> GetLuisEntities(LuisInput));
    }

    private async Task GetLuisEntities(string LuisInput)
    {
        var entity = new Microsoft.Cognitive.LUIS.Entity();
        Debug.Log(entity.GetHashCode());
        var client = new LuisClient(AppId, AppKey, Verbose, Domain);
        //  var luisResults = await client.Predict("This is a test
        var luisResults = await client.Predict(LuisInput);
        ProcessResults(luisResults);



    }


    bool newTextAvailable = false;
    string newText = string.Empty;

    private void Update()
    {
        if(newTextAvailable)
        {
            newTextAvailable = false;
     //       luisOutput.text = newText;
        }
    }


    private void ProcessResults(LuisResult res)
    {

        Debug.Log(res.OriginalQuery);
        Debug.Log(res.TopScoringIntent.Name);

        newText = res.TopScoringIntent.Name;
        newTextAvailable = true;
//#if UNITY_WSA

//        UnityEngine.WSA.Application.InvokeOnAppThread(() =>
//        {
//            // Notify of complete
//            luisOutput.text = res.TopScoringIntent.Name;
//        }, false);
//#endif

        List<string> entitiesNames = new List<string>();
        var entities = res.GetAllEntities();
        foreach (Entity entity in entities)
        {
            entitiesNames.Add(entity.Name);
            Debug.Log(entity.Name);
        }

        if (res.DialogResponse != null)
        {
            if (res.DialogResponse.Status != "Finished")
            {
                Debug.Log(res.DialogResponse.Prompt);
            }
            else
            {
                Debug.Log("Finished");
            }
        }
    }
}

