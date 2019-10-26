using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour, IUserAction, ISceneController {
    public int areaSign = -1;
    public PatrolFactory patrolFactory;
    public CrystalFactory crystalFactory;
    public PatrolActionManager actionManager;
    public Camera mainCamera;
    public GameObject player;
    public ScoreController scoreController;
    private List<GameObject> patrols;
    private List<GameObject> crystals;
    private float playerSpeed = 5f;
    private float rotateSpeed = 135f;
    private bool isGameOver = false;

    void Start () {
        SSDirector director = SSDirector.getInstance();
        director.currentSceneController = this;
        patrolFactory = Singleton<PatrolFactory>.Instance;
        crystalFactory = Singleton<CrystalFactory>.Instance;
        actionManager = gameObject.AddComponent<PatrolActionManager>() as PatrolActionManager;
        LoadResources();
        mainCamera.GetComponent<CameraMan>().target = player;
        scoreController = Singleton<ScoreController>.Instance;
	}

	void Update () {
	    for(int i = 0; i < patrols.Count; i++)
        {
            patrols[i].GetComponent<PatrolData>().areaSign = areaSign;
        }
        if(scoreController.crystal_num == 0)
        {
            GameOver();
        }
	}

    private void GameOver() {
        isGameOver = true;
        patrolFactory.stopPatrol();
        actionManager.DestroyAll();
    }

    public int GetCrystalNum() {
        return scoreController.crystal_num;
    }

    public bool GetGameOver() {
        return isGameOver;
    }

    public int GetScore() {
        return scoreController.score;
    }

    public void MovePlayer(int dir) {
        if (!isGameOver) {
            player.transform.rotation = Quaternion.Euler(new Vector3(0, dir * 90, 0));
            player.GetComponent<Animator>().SetBool("run", true);
            switch (dir) {
                case Diretion.UP:
                    player.transform.position += new Vector3(0, 0, 0.1f);
                    break;
                case Diretion.DOWN:
                    player.transform.position += new Vector3(0, 0, -0.1f);
                    break;
                case Diretion.LEFT:
                    player.transform.position += new Vector3(-0.1f, 0, 0);
                    break;
                case Diretion.RIGHT:
                    player.transform.position += new Vector3(0.1f, 0, 0);
                    break;
            }
        }
    }

    public void Restart() {
        SceneManager.LoadScene("SimplePatrols");
    }

    public void LoadResources() {
        player = Instantiate(Resources.Load("Prefabs/Player"), new Vector3(0, 9, 0), Quaternion.identity) as GameObject;
        crystals = crystalFactory.getCrystals();
        patrols = patrolFactory.getPatrols();
        for(int i = 0; i < patrols.Count; i++) {
            actionManager.GoPatrol(patrols[i]);
        }
    }

    void OnEnable() {
        Debug.Log("Scene controller onEnable");
        GameEventManager.ScoreChange += AddScore;
        GameEventManager.GameOverChange += GameOver;
        GameEventManager.CrystalChange += ReduceCrystalNumber;
    }

    private void ReduceCrystalNumber() {
        scoreController.crystal_num--;
    }

    private void AddScore() {
        scoreController.score++;
    }

    void OnDisable() {
        GameEventManager.ScoreChange -= AddScore;
        GameEventManager.GameOverChange -= GameOver;
        GameEventManager.CrystalChange -= ReduceCrystalNumber;
    }
}
