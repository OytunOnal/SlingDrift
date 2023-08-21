using UnityEngine;
using DG.Tweening;
using Game.LevelSystem.LevelEvents;
using Game.SlingSystem.Base;
using System.Collections;

namespace Game.CarSystem.Controllers
{
    public class CarDirectionController
    {
        public Tween _handleAnim;
        private Transform _carBase;
        private Coroutine _rotateAndMoveCoroutine;

        public CarDirectionController(Transform carBase)
        {
            _carBase = carBase;

            LevelEventBus.SubscribeEvent(LevelEventType.FAIL, () =>
            {
                StopAll();
            });
        }

        public void Handle(SlingTowerBase slingTower, float driftTime, int direction)
        {
            if (driftTime > 0.65f) { driftTime = 0.65f; }

            if (driftTime < 0.3f)
            {
                driftTime = 0.3f;
            }

            _rotateAndMoveCoroutine = CoroutineRunner.Instance.StartCoroutine(RotateAndMoveCoroutine(slingTower,  driftTime));

            //_rotateAndMoveCoroutine = CoroutineRunner.Instance.StartCoroutine(RotateAndMoveCoroutine(210,direction,driftTime,slingTower, SetMove));
        }

        private IEnumerator RotateAndMoveCoroutine(SlingTowerBase slingTower, float driftTime)
        {
            yield return new WaitForSeconds(0.3f*driftTime);

            Vector3 closestTarget = slingTower.GetTarget(_carBase.position);
            Vector3 carFirstTarget = closestTarget;
            carFirstTarget.x += 20 * driftTime;
            _handleAnim = _carBase.transform.DOLookAt(carFirstTarget, 0.9f * driftTime)
                .OnComplete(() =>
                _handleAnim = _carBase.transform.DOLookAt(closestTarget, 0.8f * driftTime)
                    .OnComplete(() =>
                    {
                        closestTarget = slingTower.GetTarget(_carBase.position);
                        _handleAnim = _carBase.transform.DOLookAt(closestTarget, 0.7f * driftTime)
                                .OnComplete(() =>
                                _handleAnim = null);
                    })
                );
        }

        internal void StopAll()
        {
            if (_rotateAndMoveCoroutine != null)
                CoroutineRunner.Instance.StopCoroutine(_rotateAndMoveCoroutine);
            if (_handleAnim != null && _handleAnim.IsPlaying())
            {
                _handleAnim.Kill();
                _handleAnim = null;
            }
        }

        //private IEnumerator RotateAndMoveCoroutine(float angle,int slingDirection,float driftTime, SlingTowerBase slingTower, Action<bool> SetMove)
        //{
        //    Quaternion targetRotation;
        //    Vector3 targetDirection;

        //    Vector3 closestTarget = slingTower.GetTarget(_carBase.position); 

        //    Vector3 driftDirection;
        //    Vector3 currentPosition;
        //    Quaternion currentRotation;

        //    float driftDistance = 0.7f * driftTime; // Adjust the distance as needed
        //    float driftDistanceConst = driftDistance;
        //    float angleConst = angle*driftTime;

        //    targetDirection = _carBase.right * slingDirection;
        //    driftDirection = _carBase.right * -slingDirection;

        //    //sling release rotation
        //    targetRotation = Quaternion.LookRotation(targetDirection, _carBase.up);
        //    float elapsedTime = 0;
        //    while (driftDistance > 0)
        //    {
        //        currentRotation = Quaternion.RotateTowards(_carBase.rotation, targetRotation, angle * Time.deltaTime);
        //        currentPosition = _carBase.position;
        //        currentPosition += (Time.deltaTime * GameConfig.Instance.CAR_SPEED) * (closestTarget - _carBase.position + _carBase.forward).normalized;
        //        driftDistance -= 0.05f;
        //        angle *= 0.9f;
        //        _carBase.SetPositionAndRotation(currentPosition, currentRotation);
        //        elapsedTime += Time.deltaTime;
        //        yield return new WaitForFixedUpdate();
        //    }

        //    targetDirection *= -1;
        //    driftDirection *= -1;
        //    targetRotation = Quaternion.LookRotation(targetDirection, _carBase.up);
        //    angle = angleConst;

        //    while (driftDistance < driftDistanceConst)
        //    {
        //        currentRotation = Quaternion.RotateTowards(_carBase.rotation, targetRotation, 1.5f*angle * Time.deltaTime);
        //        currentPosition = _carBase.position + (Time.deltaTime * GameConfig.Instance.CAR_SPEED) * driftDistance * driftDirection;
        //        currentPosition += (Time.deltaTime * GameConfig.Instance.CAR_SPEED) * 0.9f * (closestTarget - _carBase.position + _carBase.forward).normalized;
        //        driftDistance += 0.025f;
        //        angle *= 0.9f;
        //        _carBase.SetPositionAndRotation(currentPosition, currentRotation);
        //        _carBase.position = currentPosition;
        //        yield return new WaitForFixedUpdate();
        //    }
        //    driftDistance = driftDistanceConst;

        //    for (int i = 0; i < 2; i++)
        //        //for (int i=0; driftDistanceConst > 0.2f; i++)
        //    {
        //        angle = angleConst;

        //        targetRotation = Quaternion.LookRotation(targetDirection, _carBase.up);
        //        elapsedTime = 0;
        //        //drift and rotate
        //        while (driftDistance > 0 )
        //        {
        //            currentRotation = Quaternion.RotateTowards(_carBase.rotation, targetRotation, angle * Time.deltaTime);
        //            currentPosition = _carBase.position + (Time.deltaTime * GameConfig.Instance.CAR_SPEED) * driftDistance * driftDirection;
        //            currentPosition += (Time.deltaTime * GameConfig.Instance.CAR_SPEED) * 0.9f * ((closestTarget - _carBase.position) + _carBase.forward).normalized;
        //            driftDistance -= 0.025f;
        //            angle *= 0.9f;
        //            _carBase.SetPositionAndRotation(currentPosition, currentRotation);
        //            elapsedTime += Time.deltaTime;
        //            yield return new WaitForFixedUpdate();
        //        }

        //        angleConst *= 0.9f;
        //        driftDistanceConst *= 0.9f;

        //        closestTarget = slingTower.GetTarget(_carBase.position);

        //        targetDirection *= -1;
        //        driftDirection *= -1;

        //        // look at closest target
        //        _handleAnim = _carBase.transform.DOLookAt(closestTarget, elapsedTime*0.65f);

        //        while (driftDistance < driftDistanceConst)
        //        {
        //            currentPosition = _carBase.position + (Time.deltaTime * GameConfig.Instance.CAR_SPEED) * driftDistance * driftDirection;
        //            currentPosition += (Time.deltaTime * GameConfig.Instance.CAR_SPEED) * 0.9f * (closestTarget - _carBase.position + _carBase.forward).normalized;
        //            driftDistance += 0.05f;
        //            _carBase.position = currentPosition;
        //            yield return new WaitForFixedUpdate();
        //        }

        //        _handleAnim.Kill();
        //    }

        //    //closestTarget = slingTower.GetTarget(_carBase.position);
        //    //_handleAnim = _carBase.transform.DOLookAt(closestTarget, 50*Time.deltaTime);
        //    SetMove(true);
        //}
    }
}