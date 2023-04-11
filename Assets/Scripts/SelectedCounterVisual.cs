using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField]
    private ClearCounter clearCounter;

    private void Start()
    {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(
        object sender,
        Player.OnSelectedCounterChangedEventArgs e
    )
    {
        throw new NotImplementedException();
    }
}
