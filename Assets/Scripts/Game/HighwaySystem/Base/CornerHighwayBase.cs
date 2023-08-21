using DG.Tweening;
using Game.HighwaySystem.Base;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CornerHighwayBase : HighwayBase
{
    [SerializeField] private TMP_Text _slingNo;
    [SerializeField] private Color _slingPassedColor;
    [SerializeField] private Color _slingNotPassedColor;
    public override void SetDirection(HighwayDirection direction)
    {
        throw new System.NotImplementedException();
    }

    public void SetSlingNo(int no)
    {
        _slingNo.text = no.ToString();
    }

    public void SetSlingNoActive(bool active)
    {
        if (active) 
        {
            _slingNo.color = _slingPassedColor;
        }
        else
        {
            _slingNo.color = _slingNotPassedColor;
        }
    }

    public void PunchSlingNo()
    {
        _slingNo.transform.DOPunchScale(Vector3.one * 1.2f, 0.5f).OnComplete(() =>
            _slingNo.color = _slingPassedColor
        );
    }
}
