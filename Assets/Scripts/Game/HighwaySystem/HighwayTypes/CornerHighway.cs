using System;
using System.Collections.Generic;
using Game.HighwaySystem.Base;
using UnityEngine;
using Utils;

namespace Game.HighwaySystem.HighwayTypes
{
    public class CornerHighway : CornerHighwayBase
    {
        public override void SetDirection(HighwayDirection direction)
        {
            if(direction == Direction || direction == HighwayDirection.UP)
                return;

            Direction = direction;
            transform.eulerAngles = new Vector3(0, (int)direction * 0.5f);

            transform.ChangePositionWithChild("FinishPosition");
        }
    }
}
