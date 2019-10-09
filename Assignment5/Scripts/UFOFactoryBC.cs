using UFO.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOFactory : System.Object {
    private static UFOFactory instance;                                        //单实例
    private List<GameObject> usedUFOList = new List<GameObject>();            //使用列表
    private List<GameObject> freeUFOList = new List<GameObject>();           //空闲列表
    private GameObject UFOItem;

    public static UFOFactory getInstance() {
        if (instance == null) instance = new UFOFactory();
        return instance;
    }

    public GameObject getUFO() {
        if(freeUFOList.Count == 0) {
            GameObject newUFO = Camera.Instantiate(UFOItem);
            usedUFOList.Add(newUFO);
            return newUFO;
        }
        else {
            GameObject oldUFO = freeUFOList[0];
            freeUFOList.RemoveAt(0);
            oldUFO.SetActive(true);
            usedUFOList.Add(oldUFO);
            return oldUFO;
        }
    }

    public void detectLandingUFOs() {
        for(int i = 0; i < usedUFOList.Count; i++) {
            if(usedUFOList[i].transform.position.y <= -8) {
                usedUFOList[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                usedUFOList[i].SetActive(false);
                freeUFOList.Add(usedUFOList[i]);
                usedUFOList.Remove(usedUFOList[i]);
                i--;
            }
        }
    }

    public void RecyclingUFO(GameObject UFOObject) {
        UFOObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        UFOObject.SetActive(false);
        freeUFOList.Add(UFOObject);
        usedUFOList.Remove(UFOObject);
    }
    
    public bool isLaunching() {
        return (usedUFOList.Count > 0);
    }

    public void initItems(GameObject ufoItem) {
        UFOItem = ufoItem;
    }
}

public class UFOFactoryBC: MonoBehaviour {
    public GameObject ufoItem;

    private void Awake() {
        UFOFactory.getInstance().initItems(ufoItem);
    }
}
