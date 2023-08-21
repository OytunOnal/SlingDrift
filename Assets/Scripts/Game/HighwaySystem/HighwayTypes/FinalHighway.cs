using DG.Tweening;
using Game.CarSystem.Base;
using Game.HighwaySystem.Base;
using Game.LevelSystem.LevelEvents;
using UnityEngine;

namespace Game.HighwaySystem.HighwayTypes
{
    public class FinalHighway : HighwayBase
    {
        public override void SetDirection(HighwayDirection direction)
        {
            if(direction == Direction)
                return;
            
            Direction = direction;
            transform.eulerAngles = new Vector3(0,(int)direction);

        }

        public void FinishLevel(Transform car)
        {
            var finishPos = FinishPosition;
            switch (Direction)
            {
                case HighwayDirection.UP:
                    finishPos.x -= 20;
                    break;
                case HighwayDirection.LEFT:
                    finishPos.z += 20;
                    break;
                case HighwayDirection.RIGHT:
                    finishPos.z -= 20;
                    break;
            }
            car.transform.DOLookAt(finishPos, 0.1f);

        }
        
    }
}
