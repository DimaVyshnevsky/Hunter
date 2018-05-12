using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Manager : MonoBehaviour
{
    public enum GameState
    {
        game,
        pause
    }

    [SerializeField]
    private GameObject[] enemyPrefs;
    [SerializeField]
    private Transform[] spawnPoints;
    [SerializeField]
    private int quantityEnemiesInScene = 10;

    [Header("-----UI-----")]
    [SerializeField]
    private Button continueButton;
    [SerializeField]
    private Button restartButton;

    //private delegate void GetEnemyDelegate();
    //private List<GetEnemyDelegate> listMethods = new List<GetEnemyDelegate>();
    private GameState currentGameState;
    private List<GameObject> listEnemy;
    private int quantityMissedExecution;

    private static Manager instance;
    public static Manager Instance
    {
        get
        {
            return instance;
        }
    }

    public GameState CurrentGameState
    {
        get
        {
            return currentGameState;
        }
    }

    private void Awake()
    {
        if (instance == null)        
            instance = this;    
            DG.Tweening.DOTween.Init ( false, true, DG.Tweening.LogBehaviour.Verbose ).SetCapacity ( 200, 10 );
        DOVirtual.DelayedCall(Random.Range(2, 7), () =>
        {
            AudioManager.Instance.Play(GameClips._Zombie);
        }).SetLoops(-1, LoopType.Restart);
    }

    private void OnEnable()
    {
        Base_Behavior.DeadEvent += DeadHandler;
    }

    private void OnDisable()
    {
        Base_Behavior.DeadEvent -= DeadHandler;
    }

    private void Start()
    {
        currentGameState = GameState.game;
        if (enemyPrefs != null && enemyPrefs.Length > 0)
        {
            listEnemy = Factory.Instance.CreatePool("Enemy", enemyPrefs, quantityEnemiesInScene);
            IEnumerator temp = GetGroupOfEnemies(quantityEnemiesInScene);
            StartCoroutine(temp);
        }
    }

    #region Interface

    public List<GameObject> GetEnemyLists()
    {
        return listEnemy;
    }

    #endregion

    #region Handlers

    private void DeadHandler(string tag)
    {
        if (currentGameState == GameState.pause)
        {
            if (tag == "Enemy")
            {
                quantityMissedExecution++;
                //GetEnemyDelegate temp = GetEnemy;
                //listMethods.Add(temp);
            }
            return;
        }

        switch (tag)
        {
            case "Player":
                GameOver();
                break;
            case "Enemy":
                GetEnemy();
                break;
        }
    }

    #endregion

    #region UIInterface

    public void Restart()
    {
        string nameScene = Application.loadedLevelName;
        Application.LoadLevel(nameScene);
    }

    public void Continue()
    {
        currentGameState = GameState.game;
        continueButton.interactable = false;
        Player.Instance.ResetObj();
        DOVirtual.DelayedCall(2, ()=> 
        {
            foreach (var item in listEnemy)
            {
                item.GetComponent<Enemy>().SetState(Base_Behavior.State.hunting);
            }
        });

        if (quantityMissedExecution > 0)
        {
            IEnumerator temp = GetGroupOfEnemies(quantityMissedExecution);
            StartCoroutine(temp);
            quantityMissedExecution = 0;
        }
        //if(listMethods != null && listMethods.Count > 0)
        //foreach (var item in listMethods)
        //{
            //item.Invoke();
        //}
    }

    public void Exit()
    {
        Application.Quit();
    }

    #endregion

    #region Methods

    private void GetEnemy()
    {
        GameObject enemy = Factory.Instance.GetObject("Enemy");
        if (enemy != null)
        {
            enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            enemy.SetActive(true);
            enemy.GetComponent<Enemy>().ResetObj();
        }
    }

    private void GameOver()
    {
        currentGameState = GameState.pause;
        continueButton.interactable = true;
        foreach (var item in listEnemy)
        {
            item.GetComponent<Enemy>().SetState(Base_Behavior.State.waiting);
        }
    }

    IEnumerator GetGroupOfEnemies(int quantity)
    {
        quantity = Mathf.Clamp(quantity, 0, 100);

        for (int i = 0; i < quantity; i++)
        {
            GetEnemy();
            yield return new WaitForSeconds(Random.Range(0.2f, 2));
        }
    }

    #endregion
}
