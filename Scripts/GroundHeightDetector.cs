using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHeightDetector : MonoBehaviour
{
    // a group that handles sensor data
    private GoogleARCore.Examples.Common.DetectedPlaneGenerator _planeGenerator;
    private GoogleARCore.Examples.Common.PlaneDiscoveryGuide _planeDiscoveryGuide;
    private GoogleARCore.Examples.Common.PointcloudVisualizer _pointCloud;
    private GoogleARCore.Examples.ObjectManipulation.AndyPlacementManipulator _andyManipulator;
    private GoogleARCore.Examples.ObjectManipulation.ManipulationSystem _manipulationSystem;
    private GoogleARCore.Examples.ObjectManipulation.ObjectManipulationController _objectManipulationController;

    void Awake()
    {
        //get references from scene
        _planeGenerator = FindObjectOfType<GoogleARCore.Examples.Common.DetectedPlaneGenerator>();
        _planeDiscoveryGuide = FindObjectOfType<GoogleARCore.Examples.Common.PlaneDiscoveryGuide>();
        _pointCloud = FindObjectOfType<GoogleARCore.Examples.Common.PointcloudVisualizer>();
        _andyManipulator = FindObjectOfType<GoogleARCore.Examples.ObjectManipulation.AndyPlacementManipulator>();
        _manipulationSystem = FindObjectOfType<GoogleARCore.Examples.ObjectManipulation.ManipulationSystem>();
        _objectManipulationController = FindObjectOfType<GoogleARCore.Examples.ObjectManipulation.ObjectManipulationController>();
    }

    void Start()
    {
        CalibrateVirtualRealFloors();
        ToggleDetectorState(false);
    }

    // align the floors
    void CalibrateVirtualRealFloors()
    {
        Transform cameraHolder = FindObjectOfType<GoogleARCore.ARCoreSession>().transform;
        var calibratedPlayArea = new Vector3(0, transform.position.y, 0) * cameraHolder.transform.localScale.y;
        cameraHolder.position -= calibratedPlayArea;
        print(calibratedPlayArea);
    }

    // toggle off detectors, toggle on tamagochi and ground
    void ToggleDetectorState(bool setActive)
    {
        Transform creatureContainer = FindObjectOfType<CreatureContainerFlag>().transform;
        foreach(Transform child in creatureContainer)
        {
            child.gameObject.SetActive(!setActive);
        }

        _planeGenerator.gameObject.SetActive(setActive);
        _planeDiscoveryGuide.gameObject.SetActive(setActive);
        _pointCloud.gameObject.SetActive(setActive);
        _andyManipulator.gameObject.SetActive(setActive);
        _manipulationSystem.gameObject.SetActive(setActive);
        _objectManipulationController.gameObject.SetActive(setActive);
    }
}
