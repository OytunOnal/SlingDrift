using Config;
using Game.HighwaySystem.Base;
using Game.HighwaySystem.HighwayTypes;
using Game.LevelSystem.LevelEvents;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

namespace Game.SlingSystem.Base
{
    public class SlingTowerBase : MonoBehaviour
    {
        public int ID;
        [SerializeField] private List<GameObject> _targets;

        private CornerHighwayBase _parentHighway;
        private Transform _sling;

        public void Initialize(HighwayBase parentHighway)
        {
            _parentHighway = parentHighway as CornerHighwayBase;
            _sling = transform.Find("Sling");

            foreach (GameObject target in _targets) 
            {
                target.transform.Rotate(0, -parentHighway.transform.rotation.y,0);
            }

            LevelEventBus.SubscribeEvent(LevelEventType.INIT, () =>
            {
                ResetLine();
            });
        }
        
        public void AddLine(Transform carBase)
        {
            transform.LookAt(carBase);
            _sling.transform.ChangeScaleY(Vector3.Distance(_sling.position,carBase.transform.position) / 3f);
        }

        public void ResetLine()
        {
            _sling.transform.ChangeScaleY(1f);
        }

        public Vector3 GetFirstPosition()
        {
            return _parentHighway.transform.position;
        }

        public Vector3 GetFinalPosition()
        {
            return _parentHighway.FinishPosition;
        }

        public void PunchSlingNo()
        {
            _parentHighway.PunchSlingNo();
        }

        public bool IsPassed(Transform carBase)
        {
            if (Vector3.Distance(carBase.transform.position, _parentHighway.FinishPosition) < GameConfig.Instance.CAR_PASS_TRESHOLD)
            {
                _parentHighway.SetSlingNoActive(true);
                return true;
            }
            else return false;
        }

        public bool CanStartSling(Vector3 carPos)
        {
            return Vector3.Distance(transform.position, carPos) < GameConfig.Instance.SLING_START_DISTANCE;
        }
        public bool IsCloseTo(Vector3 carPos)
        {
            if (_parentHighway.GetType() == typeof(UCornerHighway))
                return Vector3.Distance(carPos, transform.position) < GameConfig.Instance.U_CORNER_INTERACTIBLE;

            return Vector3.Distance(carPos, transform.position) < GameConfig.Instance.CORNER_INTERACTIBLE;
        }

        public int GetDirection()
        {
            if (_parentHighway.Direction == HighwayDirection.RIGHT)
                return 1;

            return -1;
        }

        internal Vector3 GetTarget(Vector3 carPos)
        {
            GameObject closestTarget = null;
            float closestDistance = float.MaxValue;

            foreach (GameObject target in _targets)
            {
                float distance = Vector3.Distance(target.transform.position, carPos);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }

            if (closestTarget != null)
            {
                return closestTarget.transform.position;
            }
            else
            {
                // Handle the case when the list of targets is empty or no closest target is found
                return Vector3.zero; // or any other appropriate default value
            }
        }
    }
}
