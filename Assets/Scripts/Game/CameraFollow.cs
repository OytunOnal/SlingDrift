using Game.LevelSystem.LevelEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private Transform _carCamera;
    [SerializeField] private Transform _target;
    private Vector3 _cameraOffset;
    private Vector3 _targetCameraPosition;
    [SerializeField] private float _smoothTime = 0.2f; // Adjust the _smoothTime as needed
    private Vector3 _velocity = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        _carCamera = this.transform;
        _cameraOffset = _carCamera.transform.position - _target.position;
        _targetCameraPosition = _carCamera.transform.position;

    }

    private void FixedUpdate()
    {
        // Set the target camera position to follow the car without drift
        _targetCameraPosition = _target.position + _cameraOffset;
        _carCamera.position = new Vector3(_carCamera.position.x,
                                            _carCamera.position.y,
                                            _targetCameraPosition.z);

        _carCamera.position = Vector3.SmoothDamp(_carCamera.position, _targetCameraPosition, ref _velocity,
                    _smoothTime);
    }
}
