using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyCtrl : MonoBehaviour {

    public Transform playerTr;
    Vector3 offset;

    private void Awake()
    {
        offset = transform.position - playerTr.position;
    }

    private void FixedUpdate()
    {
        transform.position = playerTr.position + offset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Die");
        if (GameManager.Instance.gameState == eGameState.gamePlaying)
        {
            if (collision.CompareTag("Obstacle"))
            {
                StartCoroutine(GameManager.Instance.CoGameLose());
            }
        }
    }
}
