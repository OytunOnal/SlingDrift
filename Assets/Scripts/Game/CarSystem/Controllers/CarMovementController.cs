using Config;
using Game.CarSystem.Base;
using Game.HighwaySystem.HighwayTypes;
using Game.LevelSystem.LevelEvents;
using UnityEngine;

namespace Game.CarSystem.Controllers
{
    public class CarMovementController : MonoBehaviour
    {
        [SerializeField] ParticleSystem _smoke;
        private CarSlingController _carSlingController;
        private CarDirectionController _carDirectionController;
        
        private TrailRenderer _driftEffect;
        
        public bool IsActive;

        private bool _movingActive;
        private bool _fingerDown = false;

        public void Initialize(CarSlingController carSlingController, CarDirectionController carDirectionController)
        {
            IsActive = false;

            _carSlingController = carSlingController;
            _carDirectionController = carDirectionController;

            _driftEffect = GetComponentInChildren<TrailRenderer>();

            LevelEventBus.SubscribeEvent(LevelEventType.INIT, ()=>
            {
                _driftEffect.Clear();
            });

            LevelEventBus.SubscribeEvent(LevelEventType.STARTED, () =>
            {
                IsActive = true;
                _movingActive = true;
            });

            LevelEventBus.SubscribeEvent(LevelEventType.FAIL, ()=>
            {
                if (_smoke.isPlaying) _smoke.Stop();
                IsActive = false;
            });

            LevelEventBus.SubscribeEvent(LevelEventType.LEVEL_UP, () =>
            {
                GameConfig.Instance.CAR_SPEED *=2;
            });

            _movingActive = true;
        }
        private void FixedUpdate()
        {
            if(!IsActive)
                return;
            
            CheckInput();
            Move();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Corner"))
            {
                _carDirectionController.StopAll();
                _movingActive = true;
                _carSlingController.CanDrift(true);
            }
        }

        private void Move()
        {
            if(!_movingActive)
                return;
            
            transform.Translate(transform.forward * (Time.deltaTime * GameConfig.Instance.CAR_SPEED),Space.World);
        }

        private void CheckInput()
        {
            if (!_carSlingController.CheckAvailableSling())
                return;
            
            if (Input.GetMouseButton(0))
            {
                if (_carSlingController.OnDrifting(transform))
                {
                    if (!_smoke.isPlaying) _smoke.Play();
                    _movingActive = false;
                    _driftEffect.emitting = true;
                }
                _fingerDown = true;
            }
            else
            {
                if (_fingerDown) 
                {
                    _fingerDown = false;
                    SetMove(true);
                    _carSlingController.OnDriftingFinished(transform);
                }

                if (_smoke.isPlaying) _smoke.Stop();
                _driftEffect.emitting = false;
            }
        }

        private void SetMove(bool isMoving)
        {
            _movingActive = isMoving;
        }
    }
}
