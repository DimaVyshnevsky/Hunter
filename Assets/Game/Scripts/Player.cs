using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Base_Behavior
{
    [Header("-----Movement-----")]
    [SerializeField]
    private float Speed = 20;
    [SerializeField]
    private float TurnSpeed = 100;
    [Header("-----Fire-----")]
    [SerializeField]
    private FireSystem[] fireSystems;

    [Header("-----Effects-----")]
    [SerializeField]
    private GameObject exposion;
    [SerializeField]
    private GameObject fireLight;
    [SerializeField]
    private ParticleSystem fire;

    private FireSystem currentFireSystem;
    private Rigidbody rig;
    private float MovementInputValue;
    private float TurnInputValue;
    private int index;

    private static Transform playerTransform;
    public static Transform PlayerTransform
    {
        get
        {
            return playerTransform;
        }
    }

    private static Player instance;
    public static Player Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            playerTransform = transform;
        }
        rig = GetComponent<Rigidbody>();
        if (fireSystems != null && fireSystems.Length > 0)
            currentFireSystem = fireSystems[0];
        currentFireSystem.ActivateFireSystem(true);
        ResetObj();
    }

    protected override void OnEnable()
    {
        //если не вызывать вручную, могут сами не сработать
        base.OnEnable();
        Enemy.MakeDamageEvent += MakeDamage;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Enemy.MakeDamageEvent -= MakeDamage;
    }

    private void Update()
    {
        if (currentState == State.dead)
            return;
        MovementInputValue = Input.GetAxis("Vertical");
        TurnInputValue = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.X))
            Attack(0);
        if (Input.GetKeyUp(KeyCode.X))
            Attack(1);
        if (Input.GetKey(KeyCode.X))
            Attack(2);

        if (Input.GetKeyDown(KeyCode.Q))
            ChangeFireSystem(false);

        if (Input.GetKeyDown(KeyCode.W))
            ChangeFireSystem(true);
    }

    private void FixedUpdate()
    {
        if (currentState == State.dead)
            return;
        Move();
        Turn();
    }

    #region Interface

    public override void ResetObj()
    {
        fire.Stop();
        exposion.SetActive(false);
        fireLight.SetActive(false);
        base.ResetObj();
    }

    #endregion

    #region States

    private void Attack(int type)
    {
        if (currentFireSystem == null || fireSystems == null || fireSystems.Length == 0)
            return;
        if (currentFireSystem.CurrentWeaponType == FireSystem.WeaponsType.Launcher)
        {
            if(type == 0)
                currentFireSystem.Fire();
            else
                return;
        }
        else
            currentFireSystem.Fire();
    }

    public override void Die()
    {
        AudioManager.Instance.Play(GameClips._Tank_Explosion);
        if (exposion && fire && fireLight)
        {
            fire.Stop();
            fire.Play();
            exposion.SetActive(true);
            fireLight.SetActive(true);
        }

        currentState = State.dead;
        if (DeadEvent != null)
            DeadEvent(gameObject.tag);
    }

    #endregion

    #region Tank_Behaviour

    private void Move()
    {
        Vector3 movement = transform.forward * MovementInputValue * Speed * Time.deltaTime;
        rig.MovePosition(rig.position + movement);
    }

    private void Turn()
    {
        float turn = TurnInputValue * TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rig.MoveRotation(rig.rotation * turnRotation);
    }

    private void ChangeFireSystem(bool next)//next if true, previous if false
    {
        if (fireSystems == null || fireSystems.Length == 0)
            return;

        currentFireSystem.ActivateFireSystem(false);
        if (next)
        {
            index++;
            if (index < fireSystems.Length)
                currentFireSystem = fireSystems[index];
            else
            {
                index = 0;
                currentFireSystem = fireSystems[0];
            }
        }
        else
        {
            index--;
            if (index < 0)
            {
                index = fireSystems.Length - 1;
                currentFireSystem = fireSystems[index];
            }
            else
                currentFireSystem = fireSystems[index];
        }
        currentFireSystem.ActivateFireSystem(true);
    }

    #endregion
}
