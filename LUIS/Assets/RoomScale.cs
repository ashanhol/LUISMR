using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScale : MonoBehaviour
{
    /// <summary>
    /// Room Scale (true) or Stationary (false)
    /// </summary>
    [Tooltip("Room Scale (true) or Stationary (false)")]
    public bool NeedRoomScale = true;

    [Tooltip("Call Recenter() if in Stationary mode")]
    public bool RecenterWhenNoRoomScale = false;

    /// <summary>
    /// Objects that need to be positioned relative to the camera postion when the app starts 
    /// (only for Room Scale) 
    /// </summary>
    [Tooltip("Objects that need to be positioned relative to the camera postion when the app starts \n(only for Room Scale)")]
    public Transform[] CameraRelativeObjects;

    /// <summary>
    /// Floor object. It will become active in Room Scale  
    /// </summary>
    [Tooltip("Floor object. It will be made active in Room Scale")]
    public GameObject floorObject;

    /// <summary>
    /// Minimal Y of the objects that need to be positioned relative to the camera 
    /// (in case if the app starts with the camera placed too low (table, floor) 
    /// </summary>
    [Tooltip("Minimal Y of the objects that need to be positioned relative to the camera \n(in case if the app starts with the camera placed too low (table, floor)")]
    public float minRelativeObjectHeight = 1.5f;

    /// <summary>
    /// Camera's parent object to move to zero height (may be not zero in design time)
    /// </summary>
    [Tooltip("Camera's parent object to move to zero height \n(may be not zero in design time)")]
    public Transform CameraParentToMoveToZero;

    private bool previousValue = true;
    private bool firstEstablishedTracking = false;
    private Dictionary<Transform, Vector3> relativePositions;

    private bool doRecenter = false;

    private void Awake()
    {
        Vector3 cameraPos = Camera.main.transform.position;

        doRecenter = RecenterWhenNoRoomScale && !NeedRoomScale;

        relativePositions = new Dictionary<Transform, Vector3>();
        if (NeedRoomScale && CameraRelativeObjects != null && CameraRelativeObjects.Length > 0)
        {

            foreach (var t in CameraRelativeObjects)
            {
                var posDelta = t.position - cameraPos;
                relativePositions.Add(t, posDelta);
            }
        }

        if (CameraParentToMoveToZero != null)
        {
            CameraParentToMoveToZero.localPosition = new Vector3(CameraParentToMoveToZero.localPosition.x, 0, CameraParentToMoveToZero.localPosition.z);
        }
    }

    private void Start()
    {
        previousValue = UnityEngine.XR.XRDevice.SetTrackingSpaceType(NeedRoomScale ? UnityEngine.XR.TrackingSpaceType.RoomScale : UnityEngine.XR.TrackingSpaceType.Stationary);
        UnityEngine.XR.WSA.WorldManager.OnPositionalLocatorStateChanged += WorldManager_OnPositionalLocatorStateChanged;

        if (previousValue == NeedRoomScale)
        {
            firstEstablishedTracking = true;
            SetInitialRelativePositions();
            floorObject.SetActive(NeedRoomScale);

            CheckForRecenter();
        }
    }

    private void WorldManager_OnPositionalLocatorStateChanged(UnityEngine.XR.WSA.PositionalLocatorState oldState, UnityEngine.XR.WSA.PositionalLocatorState newState)
    {
        if (newState == UnityEngine.XR.WSA.PositionalLocatorState.Active)
        {
            previousValue = UnityEngine.XR.XRDevice.SetTrackingSpaceType(NeedRoomScale ? UnityEngine.XR.TrackingSpaceType.RoomScale : UnityEngine.XR.TrackingSpaceType.Stationary);

            CheckForRecenter();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (previousValue != NeedRoomScale)
        {
            UnityEngine.XR.TrackingSpaceType newValue = NeedRoomScale ? UnityEngine.XR.TrackingSpaceType.RoomScale : UnityEngine.XR.TrackingSpaceType.Stationary;
            var currentTracking = UnityEngine.XR.XRDevice.GetTrackingSpaceType();

            if (currentTracking != newValue)
            {
                previousValue = UnityEngine.XR.XRDevice.SetTrackingSpaceType(newValue);
            }
            else
            {
                previousValue = currentTracking == UnityEngine.XR.TrackingSpaceType.RoomScale;
            }

            if (floorObject != null)
                floorObject.SetActive(NeedRoomScale);
        }
        else if (!firstEstablishedTracking)
        {
            firstEstablishedTracking = true;

            CheckForRecenter();

            SetInitialRelativePositions();
        }
    }

    private void SetInitialRelativePositions()
    {
        Vector3 cameraPos = Camera.main.transform.position;

        foreach (var t in relativePositions)
        {
            t.Key.position = cameraPos + t.Value;
            if (t.Key.position.y < minRelativeObjectHeight)
            {
                t.Key.position = new Vector3(t.Key.position.x, minRelativeObjectHeight, t.Key.position.z);
            }
        }
    }

    private void CheckForRecenter()
    {
        if (doRecenter && UnityEngine.XR.XRDevice.GetTrackingSpaceType() == UnityEngine.XR.TrackingSpaceType.Stationary)
        {
            doRecenter = false;
            UnityEngine.XR.InputTracking.Recenter();
        }

    }
}