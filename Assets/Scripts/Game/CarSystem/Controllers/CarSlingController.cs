using Config;
using DG.Tweening;
using Game.CarSystem.Base;
using Game.LevelSystem.LevelEvents;
using Game.SlingSystem.Base;
using Game.SlingSystem.Managers;
using Game.View;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.CarSystem.Controllers
{
    public class CarSlingController
    {
        private CarDirectionController _carDirectionController;
        private Stopwatch _stopwatch = new Stopwatch();
        private readonly SlingManager _slingManager;
        private SlingTowerBase _targetSling;
        private readonly PlayerView _playerView;

        private readonly GameObject _carLeftAnchor;
        private readonly GameObject _carRightAnchor;

        private int _targetSlingIndex;
        private bool _canStartDrift = false;
        private bool _isPerfect = false;
        private int _perfectCount = 1;

        public CarSlingController(CarDirectionController carDirectionController, SlingManager slingManager, 
            PlayerView playerView, GameObject carLeftAnchor, GameObject carRightAnchor)
        {
            _slingManager = slingManager;
            _carDirectionController = carDirectionController;
            _playerView = playerView;
            _carLeftAnchor = carLeftAnchor;
            _carRightAnchor = carRightAnchor;


            LevelEventBus.SubscribeEvent(LevelEventType.INIT, ()=>
            {
                _perfectCount = 1;
                _isPerfect = false;
                _canStartDrift = false;
                _targetSling = null;
                _targetSlingIndex = 0;
            });        
        }

        public void CanDrift(bool canDrift)
        {
             _targetSling?.PunchSlingNo();
            _canStartDrift = canDrift;
        }

        public bool CheckAvailableSling()
        {
            _targetSling = _slingManager.GetSlingByID(_targetSlingIndex);
            return _targetSling != null;
        }

        public bool OnDrifting(Transform carBase)
        {
            Vector3 carPos = carBase.position;
            if(_targetSling.CanStartSling(carPos))
            {
                _targetSling.AddLine(carBase);

                if (_targetSling.IsCloseTo(carPos))
                {
                    _stopwatch.StartStopwatch();
                    _carDirectionController.StopAll();

                    //rotate around pole
                    carBase.RotateAround(_targetSling.transform.position, _targetSling.transform.up * _targetSling.GetDirection(),
                        Time.deltaTime * GameConfig.Instance.SLING_SPEED);

                    //car rotation
                    RotateCarToDriftPos(carBase);

                    return true;
                }
            }

            return false;
        }

        private void RotateCarToDriftPos(Transform carBase)
        {
            Vector3 targetAnchorPosition;
            float maxAngleDifference = 60f;

            Vector3 slingTowerPosition = _targetSling.transform.position;
            Vector3 carToSlingTower = slingTowerPosition - carBase.position;
            Vector3 carForward = carBase.forward;

            float angleDifference = Vector3.Angle(carForward, carToSlingTower);

            if (angleDifference > maxAngleDifference)
            {
                if (_targetSling.GetDirection() != 1)
                {
                    targetAnchorPosition = _carRightAnchor.transform.position; // Right anchor position
                }
                else
                {
                    targetAnchorPosition = _carLeftAnchor.transform.position; // Left anchor position
                }

                Quaternion targetRotation = Quaternion.LookRotation(carToSlingTower, carBase.up);
                Quaternion newRotation = Quaternion.RotateTowards(carBase.rotation, targetRotation, GameConfig.Instance.SLING_ROTATE_SPEED * Time.deltaTime);
                carBase.rotation = newRotation;

                // Rotate around the target anchor position
                carBase.RotateAround(targetAnchorPosition, carBase.up * _targetSling.GetDirection(), newRotation.eulerAngles.y - carBase.rotation.eulerAngles.y);
            }
        }

        public void OnDriftingFinished(Transform carBase)
        {
            float driftTime = _stopwatch.StopStopwatch();
            _targetSling.ResetLine();
            if (_targetSling.IsPassed(carBase))
            {
                if (_isPerfect) 
                    _perfectCount++;
                else _perfectCount = 1;

                _canStartDrift = false;
                _carDirectionController.Handle(_slingManager.GetSlingByID(++_targetSlingIndex), driftTime, _targetSling.GetDirection());
                _playerView.UpdateCounter(_perfectCount);
                _isPerfect = false;
            }
        }

        public void Perfect()
        {
            _isPerfect = true;
        }
    }
}
