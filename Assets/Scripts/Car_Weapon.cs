using UnityEngine;
using System.Collections;

public class Car_Weapon : MonoBehaviour
{
    public Transform BulletEmitPoint;
    BulletPooler bulletPoolScript;
    public bool IsAICar;
    public GameObject BulletFireEffect;
    bool CanFire = false;
   GameObject BulletPrefab;
    // Use this for initialization
    void Start()
    {
        BulletPrefab = Resources.Load("Weapons/Bullet") as GameObject;
        bulletPoolScript = FindObjectOfType<BulletPooler>();
       
    }

    IEnumerator FireCoroutine()
    {
        RaycastHit hit;
       while(CanFire)
        {
//            SoundManager.Instance.PlayBulletFireSound();
            //((GameObject)Instantiate(BulletPrefab, BulletEmitPoint.position, BulletEmitPoint.rotation)).GetComponent<Car_Bullet>().FiredBy = transform.parent.name;
            //if (Physics.Raycast(BulletEmitPoint.position, BulletEmitPoint.transform.forward, out hit, 500f))
            //{
            //    if (hit.transform.tag == Constants.TAG_PLAYER_CAR || hit.transform.tag == Constants.TAG_PLAYER_CAR)
            //    {
            //        flag++;
            //        avoidSenstivity -= 1;
            //        Debug.DrawLine(pos, hit.point, Color.blue);
            //    }
            //}

            GameObject bullet = bulletPoolScript.GetPooledObject();

            if (bullet != null)
            {
                bullet.GetComponent<Car_Bullet>().FiredBy = transform.parent.name;
                bullet.transform.position = BulletEmitPoint.position;
                bullet.transform.rotation = BulletEmitPoint.rotation;
                BulletFireEffect.GetComponent<ParticleSystem>().Play();
                bullet.SetActive(true);
            }

            yield return new WaitForSeconds(0.15f);
        }
        
       

    }
    // Update is called once per frame
    void Update()
    {


    }

    public void FireBulletPressed()
    {
        CanFire = true;
        StartCoroutine(FireCoroutine());
    }
    public void FireBulletReleased()
    {
        CanFire = false;
        StopCoroutine(FireCoroutine());
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsAICar)
        {
            if (other.CompareTag(Constants.TAG_AI_CAR + "Weapon") || other.CompareTag(Constants.TAG_PLAYER_CAR + "Weapon"))
            {
                //Time.timeScale = 0.2f;
                CanFire = true;
                StartCoroutine(FireCoroutine());
            }
        }


    }
    public void OnTriggerExit(Collider other)
    {
        if (IsAICar)
        {
            if (other.CompareTag(Constants.TAG_AI_CAR + "Weapon") || other.CompareTag(Constants.TAG_PLAYER_CAR + "Weapon"))
            {
                CanFire = false;
                //Time.timeScale = 0.2f;
                StopCoroutine(FireCoroutine());
            }
        }

    }
}
