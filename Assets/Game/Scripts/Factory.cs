using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObj
{
    void Init();
    void Activate();
    void Deactivate();
    void ResetObj();
    GameObject GetGameObject();
}


public class Factory : MonoBehaviour
{

    //1.
    //Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();
    //Dictionary<string, GameObject[]> dictPrefs = new Dictionary<string, GameObject[]>();

    //2.
    Dictionary<string, List<IPoolObj>> newPool = new Dictionary<string, List<IPoolObj>>();
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

    //1.
    #region Interface_1

    //public List<GameObject> CreatePool(string key, GameObject[] prefs, int quantity = 1)
    //{
    //    if (quantity <= 0 || (prefs == null || prefs.Length <= 0))
    //        return null;
    //    List<GameObject> list;
    //    bool  result = pool.TryGetValue(key, out list);
    //    if (!result)
    //    {
    //        list = new List<GameObject>();
    //        pool.Add(key, list);
    //        dictPrefs.Add(key, prefs);
    //    }

    //    for (int i = 0; i < quantity; i++)
    //    {
    //        list.Add(CreateObject(prefs[Random.Range(0, prefs.Length)]));
    //    }
    //    return list;
    //}

    //public List<GameObject> CreatePool(string key, GameObject pref, int quantity = 1)
    //{
    //    return CreatePool(key, new GameObject[1] { pref }, quantity);
    //}

    //public GameObject GetObject(string key)
    //{
    //    List<GameObject> list;
    //    bool result = pool.TryGetValue(key, out list);
    //    if (!result)
    //    {
    //        print("Current type doesn't exist, create obj"); 
    //        return null;
    //    }

    //    GameObject temp = list.Find(x => !x.activeSelf);
    //    if (!temp)
    //    {
    //        GameObject[] array = dictPrefs[key];
    //        temp = CreateObject(array[Random.Range(0, array.Length)]);
    //        list.Add(temp);
    //    }
    //    return temp;
    //}

    #endregion

    //2.
    #region Interface_2

    public List<IPoolObj> CreatePool<T>(string key, GameObject[] prefs, int quantity = 1)where T: IPoolObj
    {
        if (quantity <= 0 || (prefs == null || prefs.Length <= 0))
            return null;
        List<IPoolObj> list;
        bool result = newPool.TryGetValue(key, out list);
        if (!result)
        {
            list = new List<IPoolObj>();
            newPool.Add(key, list);
            dictPrefs.Add(key, prefs);
        }

        for (int i = 0; i < quantity; i++)
        {
            list.Add(CreateObject<T>(prefs[Random.Range(0, prefs.Length)]));
        }
        return list;
    }

    public List<IPoolObj> CreatePool<T>(string key, GameObject pref, int quantity = 1) where T : IPoolObj
    {
        return CreatePool<T>(key, new GameObject[1] { pref }, quantity);
    }

    public IPoolObj GetObject<T>(string key) where T : IPoolObj
    {
        List<IPoolObj> list;

        bool result = newPool.TryGetValue(key, out list);
        if (!result)
        {
            print("Current type doesn't exist, create obj");
            return null;
        }

        IPoolObj temp = list.Find(x => !x.GetGameObject().activeSelf);
        if (temp == null)
        {
            GameObject[] array = dictPrefs[key];
            temp = CreateObject<T>(array[Random.Range(0, array.Length)]);
            list.Add(temp);
        }
        return temp;
    }

    public List<IPoolObj> GetList(string listName)
    {
        List<IPoolObj> list;
        return newPool.TryGetValue(listName, out list) ? list : null;       
    }
    #endregion

    private IPoolObj CreateObject<T>(GameObject pref) where T : IPoolObj
    {
        GameObject temp = Instantiate(pref, Vector3.zero, Quaternion.identity);
        temp.SetActive(false);
        return temp.GetComponent<T>();
    }
}
