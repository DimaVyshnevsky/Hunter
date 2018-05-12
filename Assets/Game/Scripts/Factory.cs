using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{
    Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();
    Dictionary<string, GameObject[]> dictPrefs = new Dictionary<string, GameObject[]>();

    private static Factory instance;
    public static Factory Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)        
            instance = this; 
    }

    #region Interface

    public List<GameObject> CreatePool(string key, GameObject[] prefs, int quantity = 1)
    {
        if (quantity <= 0 || (prefs == null || prefs.Length <= 0))
            return null;
        List<GameObject> list;
        bool  result = pool.TryGetValue(key, out list);
        if (!result)
        {
            list = new List<GameObject>();
            pool.Add(key, list);
            dictPrefs.Add(key, prefs);
        }

        for (int i = 0; i < quantity; i++)
        {
            list.Add(CreateObject(prefs[Random.Range(0, prefs.Length)]));
        }
        return list;
    }

    public List<GameObject> CreatePool(string key, GameObject pref, int quantity = 1)
    {
        return CreatePool(key, new GameObject[1] { pref }, quantity);
    }

    public GameObject GetObject(string key)
    {
        List<GameObject> list;
        bool result = pool.TryGetValue(key, out list);
        if (!result)
        {
            print("Current type doesn't exist, create obj"); 
            return null;
        }

        GameObject temp = list.Find(x => !x.activeSelf);
        if (!temp)
        {
            GameObject[] array = dictPrefs[key];
            temp = CreateObject(array[Random.Range(0, array.Length)]);
            list.Add(temp);
        }
        return temp;
    }

#endregion

    private GameObject CreateObject(GameObject pref)
    {
        GameObject temp = Instantiate(pref, Vector3.zero, Quaternion.identity);
        temp.SetActive(false);
        return temp;
    }
}
