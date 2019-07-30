using UnityEngine;

public class SyncStates:MonoBehaviour{


}

public class Position
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    
    public Position(float n_x, float n_y, float n_z)
    {
        x = n_x;
        y = n_y;
        z = n_z;
    }
}
public class Rotaion
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public Rotaion(float n_x, float n_y, float n_z)
    {
        x = n_x;
        y = n_y;
        z = n_z;
    }
}


public class AvatarStates:Message
{
    public float health;


    public float forward;
    public float strafe = 0f;


    public Position position;

    public Rotaion rotation;

    public bool isRun;
    public bool isReload;
    public bool isFire;
    public bool isInTank;
    public bool avatarId;
    public bool isLocal;

    public string playerID
        ;
    public string signal = "sync";
}

public abstract class Message
{
    public string tag;

    public void UnPack(string jsonStr) {
    }
}

public class TankStates : Message
{

}

