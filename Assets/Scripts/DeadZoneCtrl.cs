﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZoneCtrl : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("MainPlayer")|| collision.CompareTag("CustomPlayer"))
        {
            PlayerCtrl player = collision.GetComponent<PlayerCtrl>();
        }
    }
}
