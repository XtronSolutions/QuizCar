using UnityEngine;
using System.Collections;

public class Car_Bullet : MonoBehaviour
{
    public float projectileSpeed = 100;
    public float damageVal;
    public GameObject bulletHit_Vehicle, bulletHit_Enfironment;
    public string FiredBy;
    // Use this for initialization
    void Start()
    {
        
        //GetComponent<Rigidbody>().AddForce(transform.forward * projectileSpeed);
        Invoke("DisableBullet", 5f);
    }

    void DisableBullet()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {

        transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime, Space.Self);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (FiredBy.Equals(collision.gameObject.name))
            return;
        if (collision.transform.CompareTag("Bullet"))
        {
            return;
        }
        else if (collision.transform.CompareTag("Environment"))
        {
            Instantiate(bulletHit_Enfironment, transform.position, transform.rotation);
        }
        else
        {
            Instantiate(bulletHit_Vehicle, transform.position, transform.rotation);
        }
        
        CarHealth health = collision.gameObject.GetComponentInChildren<CarHealth>();
        if (health!=null)
        {
            health.Damage(damageVal);
        }
        gameObject.SetActive(false);
        
    }
}