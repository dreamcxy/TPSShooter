using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Container:MonoBehaviour{
    // 物品列表
    public List<ContainerItem> items;


    private void Awake() {
        items = new List<ContainerItem>();
        items.Add(new ContainerItem(101, "Ammo_Infantry", 180, 0));
        items.Add(new ContainerItem(102, "Ammo_Handgun", 120, 0));
    }


    public void Add(ContainerItem item){
        var containerItem = GetContainerItem(item.id);
        if(containerItem != null){
            Put(item.id, item.currentNum);
        }else{
            items.Add(new ContainerItem(item.id, item.name, item.maximum, item.currentNum));
        }
        
    }

    public void Put(int itemId, int amount){
        var containerItem = items.Where(x => x.id == itemId).FirstOrDefault();
        if(containerItem == null) return ;
        containerItem.Set(amount);
    }

    // 从id的item中拿出amount的数量的东西
    public int TakeFromContainer(int itemId, int amount){
        var containerItem = GetContainerItem(itemId);
        if(containerItem == null)   return -1;
        return containerItem.Get(amount);
    }

    public int GetAmountRemaining(int itemId){
        var containerItem = GetContainerItem(itemId);
        if(containerItem == null) return -1;
        return containerItem.currentNum;
    }


    // 从items中寻找第一个为id的ContainerItem对象
    public ContainerItem GetContainerItem(int itemId){
        var containerItem = items.Where(x => x.id == itemId).FirstOrDefault();
        return containerItem;
    }

}
