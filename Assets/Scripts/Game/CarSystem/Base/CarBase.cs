using Config;
using Game.CarSystem.Controllers;
using Game.HighwaySystem.HighwayTypes;
using Game.LevelSystem.Controllers;
using Game.LevelSystem.LevelEvents;
using Game.SlingSystem.Managers;
using Game.View;
using UnityEngine;
using Zenject;

namespace Game.CarSystem.Base
{
    public class CarBase : MonoBehaviour
    {
        [SerializeField] private GameObject _carLeftAnchor;
        [SerializeField] private GameObject _carRightAnchor;

        #region Controllers

        private CarMovementController _carMovementController;
        private CarDirectionController _carDirectionController;
        private CarSlingController _carSlingController;

        #endregion

        private PlayerView _playerView;
        private SlingManager _slingManager;
        private OptLevelGenerator _optLevelGenerator;
        private bool _isActive;

        [Inject]
        private void OnInstaller(SlingManager slingManager, PlayerView playerView, OptLevelGenerator optLevelGenerator)
        {
            _slingManager = slingManager;
            _playerView = playerView;
            _optLevelGenerator = optLevelGenerator;
        }

        private void OnApplicationQuit()
        {
            LevelEventBus.InvokeEvent(LevelEventType.FAIL);
        }

        public void Initialize()
        {
            _carDirectionController = new CarDirectionController(transform);
            _carSlingController = new CarSlingController(_carDirectionController,_slingManager,_playerView,_carLeftAnchor,_carRightAnchor);

            _carMovementController = GetComponent<CarMovementController>();
            _carMovementController.Initialize(_carSlingController,_carDirectionController);

            LevelEventBus.SubscribeEvent(LevelEventType.INIT, () =>
            {
                transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                GameConfig.Instance.CAR_SPEED = 50;
                GameConfig.Instance.SLING_SPEED = 160;
                gameObject.SetActive(true);
            });

            LevelEventBus.SubscribeEvent(LevelEventType.STARTED, ()=>
            {
                _isActive = true;
            });

            LevelEventBus.SubscribeEvent(LevelEventType.FAIL,() 
                =>
            {
                _isActive = false;
            });
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Edge")
            {
                if (!_isActive)
                    return;
                LevelEventBus.InvokeEvent(LevelEventType.FAIL);
            }
            else if (other.tag == "Final")
            {
                var final = other.GetComponentInParent<FinalHighway>();
                if (final != null)
                {
                    _carDirectionController.StopAll();
                    final.FinishLevel(this.transform);
                }
                LevelEventBus.InvokeEvent(LevelEventType.LEVEL_UP);
            }
            else if (other.tag == "Perfect")
            {
                _playerView.Perfect();
                _carSlingController.Perfect();
            }
            else if (other.tag == "FinalEnd")
            {
                if (transform.localScale.x < 1)
                    transform.localScale = new Vector3(transform.localScale.x + 0.1f,
                                                        transform.localScale.y + 0.1f,
                                                        transform.localScale.z + 0.1f);
                GameConfig.Instance.CAR_SPEED /= 2;
                GameConfig.Instance.CAR_SPEED += 5;
                GameConfig.Instance.SLING_SPEED += 10;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Corner")
            {
                _optLevelGenerator.GenerateRoad(1);
            }
        }

        public void SetCarPosition(Transform objeTransform)
        {
            transform.position = objeTransform.position;
            transform.eulerAngles = objeTransform.eulerAngles;
        } 
    }
}

