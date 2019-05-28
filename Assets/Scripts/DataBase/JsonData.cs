using System.Collections;
using System.Collections.Generic;

// 接收服务器消息：{"playerName": "name", "weapons": [{"InfantryBulletCurrent": 25, "Infantry": "Aksu", "InfantryBulletAll": 60}, {"HandgunBulletAll": 20, "HandgunBulletCurrent": 8, "Handgun": "Glock"}], "password": "pass", "playerState": {"currentLevel": 3, "health": 90, "experience": 90}}


public class JsonData
{
    public string playerName { get; set; }
    public string password { get; set; }
    public float health { get; set; }

    public List<WeaponInfo> weaponInfos { get; set; }

    public JsonData(string new_playerName, string new_password, float new_health, List<WeaponInfo> new_weaponInfos)
    {
        playerName = new_playerName;
        password = new_password;
        health = new_health;
        new_weaponInfos.ForEach(i => weaponInfos.Add(i));
        // weaponInfos = new List<WeaponInfo>(new_weaponInfos.ToArray());
    }
}


public class WeaponInfo
{
    public string gunName { get; set; }
    public int clipAmmos { get; set; }
    public int clipAll { get; set; }

    public WeaponInfo(string new_gunName, int new_clipAmmos, int new_clipAll){
        gunName = new_gunName;
        clipAmmos = new_clipAmmos;
        clipAll = new_clipAll;
    }
}

public class LogInInfo{
    public string playerName;
    public string password;
    public string signal = "2";
}


public class PlayerState {
	public string health  { get; set; }
	public string experience  { get; set; }
	public string currentLevel  { get; set; }
}

public class RootObject {
	public string playerName  { get; set; }
	public string password  { get; set; }
	public List<WeaponInfo> weapons { get; set; }
	public PlayerState playerState { get; set; }
	public string sign  { get; set; }
}