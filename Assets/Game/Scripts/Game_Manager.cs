using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Game_Manager : MonoBehaviour
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
    private List<Enemy> listEnemy = new List<Enemy>();
    private int quantityMissedExecution;

    private static Game_Manager instance;
    public static Game_Manager Instance
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
        else
            return;
        DG.Tweening.DOTween.Init ( false, true, DG.Tweening.LogBehaviour.Verbose ).SetCapacity ( 200, 10 );
        DOVirtual.DelayedCall(Random.Range(7, 15), () =>
        {
            Audio_Manager.Instance.Play("_Zombie_" + Random.Range(1, 4));
        }).SetLoops(-1, LoopType.Restart);
    }

    private void OnEnable()
    {
        Hunter_Base.DeadEvent += DeadHandler;
    }

    private void OnDisable()
    {
        Hunter_Base.DeadEvent -= DeadHandler;
    }

    private void Start()
    {
        currentGameState = GameState.game;
        if (enemyPrefs != null && enemyPrefs.Length > 0)
        {
            List<IPoolObj> list = Factory.Instance.CreatePool<Enemy>("Enemy", enemyPrefs, quantityEnemiesInScene);
            foreach (var item in list)
            {
                item.Init();
                listEnemy.Add(item.GetGameObject().GetComponent<Enemy>());
            }     
            
            IEnumerator temp = GetGroupOfEnemies(quantityEnemiesInScene);
            StartCoroutine(temp);
        }
    }

    #region Handlers

    private void DeadHandler(string tag)
    {
        if (currentGameState == GameState.pause)
        {
            if (tag == "Enemy")           
                quantityMissedExecution++;
            return;
                //GetEnemyDelegate temp = GetEnemy;
                //listMethods.Add(temp);      
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
        Player.Instance.Restart();
        DOVirtual.DelayedCall(2, ()=> 
        {
            foreach (var item in listEnemy)
                item.SetState(Hunter_Base.State.hunting);            
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
        IPoolObj enemy = Factory.Instance.GetObject<Enemy>("Enemy");
        if (enemy != null)
        {
            enemy.GetGameObject().transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            enemy.ResetObj();
            enemy.Activate();
        }
    }

    private void GameOver()
    {
        currentGameState = GameState.pause;
        continueButton.interactable = true;
        foreach (var item in listEnemy)
            item.SetState(Hunter_Base.State.waiting);
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
