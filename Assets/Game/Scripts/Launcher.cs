using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : FireSystem
{
    [SerializeField]
    private GameObject explosionEffect;

    protected override void Start()
    {
        base.Start();
        Factory.Instance.CreatePool(explosionEffect.name, explosionEffect, quantityObjsInPool);
    }
}
