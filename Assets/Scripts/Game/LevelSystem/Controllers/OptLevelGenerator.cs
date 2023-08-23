using System.Collections.Generic;
using Config;
using Game.CarSystem.Base;
using Game.HighwaySystem.Base;
using Game.HighwaySystem.HighwayTypes;
using Game.LevelSystem.LevelEvents;
using Game.LevelSystem.Managers;
using Game.Managers;
using UnityEngine;
using Utils;
using VContainer;
using static UnityEngine.UI.GridLayoutGroup;

namespace Game.LevelSystem.Controllers
{
    public class OptLevelGenerator : MonoBehaviour
    {
        private static int _levelIndex;
        private int _slingCount;
        private int _roadCount;
        private bool _generateUTurn;

        private HighwayDirection _straightDirection;
        private HighwayDirection _cornerDirection;

        private HighwayBase _straightHighway = null;
        private HighwayBase _corner = null;

        private PoolManager _poolManager;
        private LevelManager _levelManager;
        private CarBase _carBase;


        private readonly List<HighwayDirection> _highwayDirections = new List<HighwayDirection>
            {
                HighwayDirection.UP,
                HighwayDirection.LEFT,
                HighwayDirection.RIGHT
            };

        [Inject]
        private void Execute(PoolManager poolManager, LevelManager levelManager, CarBase carBase)
        {
            _carBase = carBase;
            _poolManager = poolManager;
            _levelManager = levelManager;
        }

        public void Initialize()
        {
            LevelEventBus.SubscribeEvent(LevelEventType.FAIL, () =>
            {
            });

            LevelEventBus.SubscribeEvent(LevelEventType.INIT, () =>
            {
                _levelIndex = 0;
                _levelManager.DeleteWholeLevels();

                _slingCount = 1;

                _roadCount = 0;
                _straightDirection = HighwayDirection.UP;
                _cornerDirection = _straightDirection;
                _corner = null;
                _generateUTurn = false;
                _straightHighway = GenerateHighway<StraightHighway>(_straightDirection, _corner, false);
                GenerateRoad(4);

                _carBase.SetCarPosition(_levelManager.GetHighwayOfLevel(0, 0).transform);
            });
        }

        public void GenerateRoad(int roadNumber)
        {
            for (int j = 0; j < roadNumber; j++)
            {

                if (_generateUTurn)
                {
                    _cornerDirection = _straightDirection;
                    GenerateUTurn();

                    if (_cornerDirection == HighwayDirection.LEFT)
                    {
                        _straightDirection = HighwayDirection.RIGHT;
                        _straightHighway = GenerateHighway<StraightHighway>(_straightDirection, _corner, false);
                    }
                    else
                    {
                        _straightDirection = HighwayDirection.LEFT;
                        _straightHighway = GenerateHighway<StraightHighway>(_straightDirection, _corner, true);
                    }

                    _roadCount++;
                    _generateUTurn = false;
                    continue;
                }

                if (_roadCount > GameConfig.Instance.LEVEL_LENGTH)
                {
                    _straightHighway = GenerateHighway<FinalHighway>(_straightDirection, _corner, false);
                    _roadCount = 0;
                    _levelIndex++;
                }
                
                if (_straightDirection == _cornerDirection)
                    _straightDirection = HighwayDirection.UP;


                if (_straightDirection == HighwayDirection.UP)
                {
                    _cornerDirection = _highwayDirections.GetRandomElementFromList(HighwayDirection.UP);
                    GenerateCorner();

                    _straightDirection = _cornerDirection == HighwayDirection.LEFT
                        ? HighwayDirection.RIGHT
                        : HighwayDirection.LEFT;
                    _straightHighway = GenerateHighway<StraightHighway>(_straightDirection, _corner, false);

                    // Random U Corner Generation
                    int rnd = Random.Range(0, 10);
                    if (rnd > GameConfig.Instance.U_CORNER_PROBABILITY)
                    {
                        _generateUTurn =true;
                    }
                    _generateUTurn = true;

                }
                else
                {
                    _cornerDirection = _straightDirection;
                    GenerateCorner();

                    _straightDirection = HighwayDirection.UP;
                    _straightHighway = GenerateHighway<StraightHighway>(_straightDirection, _corner, false);
                }

                 _roadCount ++;
            }
        }

        private void GenerateCorner()
        {
            if (_cornerDirection == HighwayDirection.LEFT)
                _corner = GenerateHighway<LeftTurnHighway>(_cornerDirection, _straightHighway);
            else
                _corner = GenerateHighway<RightTurnHighway>(_cornerDirection, _straightHighway);

            (_corner as CornerHighwayBase).SetSlingNo(_slingCount);
            (_corner as CornerHighwayBase).SetSlingNoActive(false);
            _slingCount++;
        }

        private void GenerateUTurn()
        {
            if (_cornerDirection == HighwayDirection.LEFT)
                _corner = GenerateHighway<ULeftHighway>(_cornerDirection, _straightHighway,false);
            else
                _corner = GenerateHighway<URightHighway>(_cornerDirection, _straightHighway,false);

            (_corner as CornerHighwayBase).SetSlingNo(_slingCount);
            (_corner as CornerHighwayBase).SetSlingNoActive(false);
            _slingCount++;
        }

        private T GenerateHighway<T>(HighwayDirection cornerDirection, HighwayBase highwayBase, bool rotate = true)
            where T : HighwayBase
        {
            var highway = _poolManager.GetAvailableHighWay<T>();
            highway.SetDirection(cornerDirection);
            if (highwayBase != null)
            {
                highway.transform.position = highwayBase.FinishPosition;
                if (rotate)
                    highway.transform.Rotate(highwayBase.transform.eulerAngles.y * Vector3.up);
            }

            _levelManager.AddLevel(_levelIndex, highway);
            return highway;
        }

    }
}
