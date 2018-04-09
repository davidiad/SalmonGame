using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{

    PooledObject prefab;
    List<PooledObject> availableObjects = new List<PooledObject>();
    //public int maxNumObjects = 23;

    public static ObjectPool GetPool (PooledObject prefab)
    {
        GameObject obj;
        ObjectPool pool;
        if (Application.isEditor) {
            obj = GameObject.Find(prefab.name + " Pool");
            if (obj) {
                pool = obj.GetComponent<ObjectPool>();
                if (pool) {
                    return pool;
                }
            }
        }
        obj = new GameObject(prefab.name + " Pool");
        pool = obj.AddComponent<ObjectPool>();
        pool.prefab = prefab;
        return pool;
    }

    public PooledObject GetObject()
    {
        PooledObject obj;
        int lastAvailableIndex = availableObjects.Count - 1;
        if (lastAvailableIndex >= 0) {
            obj = availableObjects[lastAvailableIndex];
            availableObjects.RemoveAt(lastAvailableIndex);
            obj.gameObject.SetActive(true);
        }
        else {
            //if (transform.childCount <= maxNumObjects)
            //{
                obj = Instantiate<PooledObject>(prefab);
                obj.transform.SetParent(transform, false);
                obj.Pool = this;
            //}
            //else
            //{ // need to return some object, even if no new ones are allowed
            //    obj = availableObjects[0];
            //    obj.gameObject.SetActive(true);
            //}
        }
        Debug.Log("The # of NPFs is now: " + this.transform.childCount);
        return obj;
    }

    public void AddObject(PooledObject o)
    {
        Object.Destroy(o.gameObject);
    }
}