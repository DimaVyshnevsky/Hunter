using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public class MoverBullet : WeaponBase, IPoolObj
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
            Deactivate();
            return;
        }

        if (Speed < SpeedMax)      
            Speed += SpeedMult * Time.fixedDeltaTime;      
    }

    #region Interface

    public void Init()
    {
        init = true;
        if (!damage)
            damage = GetComponent<Damage>();
        if (startSpeed == 0)
            startSpeed = Speed;
    }

    public void Activate()
    {
        if (!init)
            Init();
        startPosition = transform.position;
        direction = transform.forward;
        gameObject.SetActive(true);      
        fire = true;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void ResetObj()
    {
        if (!init)
            Init();
        fire = false;
        Speed = startSpeed;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #endregion
}
