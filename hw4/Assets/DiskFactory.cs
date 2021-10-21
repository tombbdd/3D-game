using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

//用于生产飞碟的工厂
public class DiskFactory : MonoBehaviour {
    public GameObject diskPrefab;
    //保存飞碟
    private List<DiskComponent> used = new List<DiskComponent>();     //used是用来保存正在使用的飞碟 
    private List<DiskComponent> free = new List<DiskComponent>();     //free是用来保存未激活的飞碟
    private int num = 1;
    //根据游戏的当前轮数，随机的为飞碟分配一个身份（对应不同分值），前面的关卡没有高分的飞碟
    private int getCharacter(int round)
    {
        int character = 0;
        if (round == 0)
        {
            character = 0;
        }
        else if (round == 1)
        {
            character = Random.Range(-2f, 1f) > 0 ? 1 : 0;
        }
        else
        {
            character = Random.Range(-3f, 2f) > 0 ? 1 : 2;
        }
        return character;
    }
    private float getStartX(int round)
    {
        //随机初始化一个丢出飞碟的位置
        if (round == 0)
        {
            return UnityEngine.Random.Range(-0.5f, 0.5f);
        }
        else if (round == 1)
        {
            return UnityEngine.Random.Range(-1.3f, 1.3f);
        }
        else
        {
            return UnityEngine.Random.Range(-2f, 2f);
        }
    }
    private void Awake()
    {
        //利用提前制作好的预制生成游戏对象
        diskPrefab = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Disk"), Vector3.zero, Quaternion.identity);
        diskPrefab.SetActive(false);
    }
    //生成一个飞碟
    public GameObject GetDisk(int round)
    {
        GameObject newdisk = null;
        if(free.Count > 0)
        {
            newdisk = free[0].gameObject;
            free.Remove(free[0]);
        }
        else
        {
            newdisk = GameObject.Instantiate<GameObject>(diskPrefab, Vector3.zero, Quaternion.identity);
            newdisk.AddComponent<DiskComponent>();
        }
        //通过关卡得到飞碟的属性
        int disk_character = getCharacter(round);
        //通过关卡得到飞碟的起始位置
        float RanX = getStartX(round);
        //选择生成飞碟的样式
        switch (disk_character)
        {
            case 0:
                {
                    newdisk.GetComponent<DiskComponent>().score = 1;
                    newdisk.GetComponent<DiskComponent>().speed = 5.0f;
                    newdisk.GetComponent<DiskComponent>().direction = new Vector3(RanX, 1.5f, 0);
                    newdisk.GetComponent<Renderer>().material.color = Color.white;
                    break;
                }
            case 1:
                {
                    newdisk.GetComponent<DiskComponent>().score = 2;
                    newdisk.GetComponent<DiskComponent>().speed = 6.0f;
                    newdisk.GetComponent<DiskComponent>().direction = new Vector3(RanX, 1, 0);
                    newdisk.GetComponent<Renderer>().material.color = Color.yellow;
                    break;
                }
            case 2:
                {
                    newdisk.GetComponent<DiskComponent>().score = 3;
                    newdisk.GetComponent<DiskComponent>().speed = 7.0f;
                    newdisk.GetComponent<DiskComponent>().direction = new Vector3(RanX, 0.5f, 0);
                    newdisk.GetComponent<Renderer>().material.color = Color.red;
                    break;
                }
        }
        used.Add(newdisk.GetComponent<DiskComponent>());
        newdisk.name = "disk_" + num;
        num++;
        return newdisk;
    }
    //free掉已用过的飞碟
    public void FreeDisk(GameObject disk)
    {
        DiskComponent temp = null;
        foreach (DiskComponent i in used)
        {
            if (disk.GetInstanceID() == i.gameObject.GetInstanceID())
            {
                temp = i;
            }
        }
        if(temp != null)
        {
            temp.gameObject.SetActive(false);
            free.Add(temp);
            used.Remove(temp);
        }
    }

}
