using UnityEngine;
using System.Collections;

public class CarHealth : MonoBehaviour
{
    float Health = 100f;
    public float TotalHealth = 100;
    public GameObject TotalHealthBar;
    public GameObject RemainingHealthBar;
    public GameObject DeathExplosion;
    // Use this for initialization
    void Start()
    {
        Health = TotalHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //public void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.CompareTag(Constants.TAG_BULLET))
    //    {
    //        Health -= Constants.BulletHealthHit;
    //        collision.gameObject.SetActive(false);
    //    }
    //    else if (collision.transform.CompareTag(Constants.TAG_MISSILE))
    //    {
    //        Health -= Constants.MissileHealthHit;
    //    }
    //    else if (collision.transform.CompareTag(Constants.TAG_BOMB))
    //    {
    //        Health -= Constants.BombHealthHit;
    //    }
    //    UpdateHealthBar();
    //}
    public void Damage(float damageVal)
    {
        Health -= damageVal;
        UpdateHealthBar();
    }
    public void UpdateHealthBar()
    {
        RemainingHealthBar.transform.localScale = new Vector3(1f,1f,Health / TotalHealth);
        if (Health <= 0)
        {
            Instantiate(DeathExplosion,transform.position, transform.rotation);
            GetComponentInParent<Rigidbody>().isKinematic = true;

            gameObject.GetComponentInParent<vehicleHandling>().DestroyCar();
        }
    }
}