using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MoverBullet : WeaponBase
{
    [SerializeField]
    private float LifeDistanse = 20;
      
    private Vector3 direction;
    private Vector3 startPosition;

    private void FixedUpdate()
    {
        if (!fire )
			return;		    
        transform.position += direction * Speed;
        if (Vector3.Distance(startPosition, transform.position) > LifeDistanse)
        {
            ResetObj();
            gameObject.SetActive(false);
        }

        if (Speed < SpeedMax)      
            Speed += SpeedMult * Time.fixedDeltaTime;      
    }

    #region Interface

    public override void Init()
    {
        init = true;
        if (!damage)
            damage = GetComponent<Damage>();
        if (startSpeed == 0)
            startSpeed = Speed;
    }

    public override void Activate()
    {
        if (!init)
            Init();
        gameObject.SetActive(true);      
        startPosition = transform.position;
        direction = transform.forward;
        fire = true;
    }

    public override void ResetObj()
    {
        if (!init)
            Init();
        fire = false;
        Speed = startSpeed;
    }

    #endregion
}
