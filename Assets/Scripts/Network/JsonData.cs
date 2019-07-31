using System.Collections;
using System.Collections.Generic;

// 接收服务器消息：{"playerName": "name", "weapons": [{"InfantryBulletCurrent": 25, "Infantry": "Aksu", "InfantryBulletAll": 60}, {"HandgunBulletAll": 20, "HandgunBulletCurrent": 8, "Handgun": "Glock"}], "password": "pass", "playerState": {"currentLevel": 3, "health": 90, "experience": 90}}

// 人物的武器信息
public class WeaponInfo
{
    public string gunName { get; set; }
    public int clipAmmos { get; set; }
    public int clipLeft { get; set; }

    public WeaponInfo(string new_gunName, int new_clipAmmos, int new_clipLeft){
        gunName = new_gunName;
        clipAmmos = new_clipAmmos;
        clipLeft = new_clipLeft;
    }
}



// 登陆信息
public class LoginInfo{
    public string playerName;
    public string password;
    public string signal = "login";
}

// 人物状态。。但真正考虑的只有health
public class PlayerState {
	public float health  { get; set; }
    //public float money;
	// public string experience  { get; set; }
	// public string currentLevel  { get; set; }
    public PlayerState(float new_health){
        health = new_health;
    }
}

// 上传的Json数据格式
public class RootObject {

    public string playerName  { get; set; }
	public string password  { get; set; }
	public List<WeaponInfo> weaponInfos { get; set; }
	public PlayerState playerState { get; set; }
	public string signal  { get; set; }
}



[System.Serializable]
public class RoomInfo
{
    public string roomName { get; set; }
    public string playerName { get; set; }
    public string password { get; set; }

    // create , attend, search 
    public string signal { get; set; }

    public RoomInfo(string r, string pN, string p, string s)
    {
        roomName = r;
        playerName = pN;
        password = p;
        signal = s;
    }
}

[System.Serializable]
// 关卡信息
public class LevelInfo
{
    public int level;
    public string signal = "levelUpdate";
}

public class MonstersItem
{
    /// <summary>
    /// 
    /// </summary>
    public int monsterId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int monsterType { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int monsterHealth { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<List<int>> monsterGuardPoints { get; set; }
}

public class MonsterRoot
{
    /// <summary>
    /// 
    /// </summary>
    public int level { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int monsterNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<MonstersItem> monsters { get; set; }
}