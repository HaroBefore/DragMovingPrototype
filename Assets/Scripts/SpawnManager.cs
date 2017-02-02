using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    static SpawnManager instance;
    public static SpawnManager Instance
    {
        get { return instance; }
    }



    public float delay = 0.1f;
    public static int maxSpawnCnt = -1;
    public static int spawnCnt = 0;
    public int startSpawnCnt = 5;
    public bool isAllSpawned = false;

    public GameObject obstaclePrefab;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        spawnCnt = 0;
        isAllSpawned = false;
        maxSpawnCnt = startSpawnCnt;
	}

	// Update is called once per frame
	void Update () {
		
	}

    //x -10 ~ 10 / y -6 ~ 6
    void Spawn()
    {
        spawnCnt++;
        ObstacleCtrl obstacle = Instantiate(obstaclePrefab, new Vector3(Random.Range(-5f, 5f), Random.Range(-6f, 8.5f), 0f), Quaternion.identity).GetComponent<ObstacleCtrl>();
        obstacle.StartCoroutine(obstacle.CoInit());
    }

    public IEnumerator CoSpawnObstacles()
    {
        while(spawnCnt < maxSpawnCnt)
        {
            yield return new WaitForSeconds(delay);
            Spawn();
        }
    }
}
