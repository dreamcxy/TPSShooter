using System.Collections;
using System.Collections.Generic;
public class Data
{
    public string playerName { get; set; }
    public string password { get; set; }
    public float health { get; set; }

    public List<WeaponInfo> weaponInfos { get; set; }

    public Data(string new_playerName, string new_password, float new_health, List<WeaponInfo> new_weaponInfos)
    {
        playerName = new_playerName;
        password = new_password;
        health = new_health;
        // new_weaponInfos.ForEach(i => weaponInfos.Add(i));
        weaponInfos = new List<WeaponInfo>(new_weaponInfos.ToArray());
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
