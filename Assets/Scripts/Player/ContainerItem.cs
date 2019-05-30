using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    背包中每个物品的属性
    id
    名字
    最大数量
    目前数量
 */

 [System.Serializable]

public class ContainerItem{

    public int id;
    public string name;
    public int maximum;
    
    public int currentNum;

    public ContainerItem(int id, string name, int maximum, int currentNum){
        this.id = id;
        this.name = name;
        this.maximum = maximum;
        this.currentNum = currentNum;
    }

    public int Get(int value){
        if (currentNum - value < 0)
        {
            int toMuch = currentNum;
            currentNum = 0;
            return toMuch;
        }
        currentNum -= value;
        return value;
    }

    public void Set(int amount){
        currentNum += amount;
        if (currentNum > maximum)
        {
            currentNum = maximum;
        }
    }
}