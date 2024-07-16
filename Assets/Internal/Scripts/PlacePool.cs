using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
[RequireComponent(typeof(ARRaycastManager),typeof(ARPlaneManager),typeof(ARAnchorManager))]
public class PlacePool : MonoBehaviour
{
    [SerializeField] private GameObject _poolPrefab;
    [SerializeField] private Canvas _setUpCanvas;
    [SerializeField] private Canvas _poolCanvas;
    [SerializeField] private GameObject _poolUI;
    [SerializeField] private GameObject _settingsUI;

    private GameObject _setPool;
    private ARAnchor _anchor;
    private ARRaycastManager _aRRaycastManager;
    private ARPlaneManager _planeManager;
    private ARAnchorManager _anchorManager;
    private List<ARRaycastHit> _hits= new List<ARRaycastHit>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        _poolCanvas.enabled = false;
        _aRRaycastManager = GetComponent<ARRaycastManager>();
        _planeManager = GetComponent<ARPlaneManager>();
        _anchorManager = GetComponent<ARAnchorManager>();
        _poolUI.SetActive(true);
        _settingsUI.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;

    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;

    }

    private void FingerDown(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0||_setPool!=null) { return; }
        ARCast(finger);
    }

    private void DisablePlanes()
    {
            _poolCanvas.enabled = true;
            _setUpCanvas.enabled = false;
            _planeManager.requestedDetectionMode=PlaneDetectionMode.None;
        foreach (var plane in _planeManager.trackables)
         {
             plane.gameObject.SetActive(false);
         } 
    }

    private void EnablePlanes()
    {
        _poolUI.SetActive(true);
        _settingsUI.SetActive(false);
        _poolCanvas.enabled = false;
        _setUpCanvas.enabled = true;
        foreach (var plane in _planeManager.trackables)
        {
            plane.gameObject.SetActive(true);
        }
        _planeManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;

    }

    private void ARCast(EnhancedTouch.Finger finger)
    {
        if (_aRRaycastManager.Raycast(finger.currentTouch.screenPosition,_hits,TrackableType.PlaneWithinPolygon))
        {
            
                Pose pose = _hits[0].pose;
                GameObject obj = Instantiate(_poolPrefab, new Vector3( pose.position.x, pose.position.y-1, pose.position.z), pose.rotation);
            if (obj.GetComponent<ARAnchor>() == null)
            {
                obj.AddComponent<ARAnchor>();
            }
            _setPool = obj;
                DisablePlanes();

           

        }
    }

    public void Reset()
    {
        Destroy(_setPool);
        _setPool = null;
        EnablePlanes();
    }

    public void ShowSettings(bool enable)
    { 
        _poolUI.SetActive(!enable);
        _settingsUI.SetActive(enable);
    }

    public void ScalePool(float size)
    {
        if (_setPool !=null)
        {
            _setPool.transform.localScale = new Vector3(size, _setPool.transform.localScale.y, size);
        }
    }
}
