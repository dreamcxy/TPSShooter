using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    public float power = 10; // 发射时的速度\力
    public float angle = 25;
    public float gravity = -10;

    public float radius = 0.001f;   // 爆炸半径

    public float bombDamage = 30;


    private Vector3 moveSpeed;
    private Vector3 gravitySpeed = Vector3.zero;
    private float dTime;
    private Vector3 currentAngle;

    bool isbomb;

    ParticleSystem bombEffect;

    private void Awake()
    {

    }

    // Start is called before the first frame update

    void Start()
    {

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        // 修正抛出角度
        Vector3 angleCorrection = player.up + player.forward;
        // Vector3 angleCorrection = new Vector3(0, 0, angle);
        Debug.LogFormat("angleCorrection :{0}", angleCorrection);
        moveSpeed = angleCorrection * power;
        currentAngle = Vector3.zero;

        bombEffect = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("炸弹扔出去了");
    }

    private void FixedUpdate()
    {
        if (!isbomb)
        {
            gravitySpeed.y = gravity * (dTime += Time.fixedDeltaTime);
            transform.position += (moveSpeed + gravitySpeed) * Time.fixedDeltaTime;
            currentAngle.z = Mathf.Atan((moveSpeed.y + gravitySpeed.y) / moveSpeed.x) * Mathf.Rad2Deg;
            transform.eulerAngles = currentAngle;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "Desert")
        {
            isbomb = true;
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            bombEffect.Play();
            this.GetComponent<SphereCollider>().isTrigger = false;
            Destroy(gameObject, 5);
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

            foreach (Collider hit in colliders)
            {
                if (hit.transform.tag == "Ragdoll" && hit.GetComponent<Rigidbody>())
                {
                    // Debug.Log("hit:{0}", hit);
                    if (hit.GetComponent<Rigidbody>())
                    {
                        Debug.Log("hit enemy....");
                        hit.GetComponent<Rigidbody>().AddExplosionForce(1, explosionPos, radius);

                        if (hit.GetComponent<EnemyStates>())
                        {
                            Debug.Log("start hit calculation...");

                            hit.GetComponent<EnemyStates>().ApplyDamage(bombDamage);
                        }
                    }


                }
            }
        }

    }

}
