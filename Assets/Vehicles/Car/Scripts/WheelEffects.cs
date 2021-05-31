using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (AudioSource))]
    public class WheelEffects : MonoBehaviour
    {
        public Transform SkidTrailPrefab;
        public static Transform skidTrailsDetachedParent;
        public ParticleSystem skidParticles;
        public bool skidding { get; private set; }
        public bool PlayingAudio { get; private set; }

        public float SkidmarksOffset;
        private AudioSource m_AudioSource;
        private Transform m_SkidTrail;
        private WheelCollider m_WheelCollider;


        private void Start()
        {
           
                
                skidParticles = transform.root.GetComponentInChildren<ParticleSystem>();

                if (skidParticles == null)
                {
                    Debug.LogWarning(" no particle system found on car to generate smoke particles");
                }
                else
                {
                    skidParticles.Stop();
                }

                m_WheelCollider = GetComponent<WheelCollider>();
                lastPos = m_WheelCollider.transform.position;
                m_AudioSource = GetComponent<AudioSource>();
                PlayingAudio = false;

                if (skidTrailsDetachedParent == null)
                {
                    skidTrailsDetachedParent = new GameObject("Skid Trails - Detached").transform;
                }
            
        }


        public void EmitTyreSmoke()
        {

            //if (m_WheelCollider.isGrounded)
            //{
            //    skidParticles.transform.position = transform.position - transform.up * m_WheelCollider.radius;
            //    skidParticles.Emit(1);
            //    if (!skidding)
            //    {
            //        //StartCoroutine(StartSkidTrail());
            //    }
            //}


        }


        public void PlayAudio()
        {
			if (m_AudioSource.enabled) {
				m_AudioSource.Play ();
				PlayingAudio = true;
			}
        }


        public void StopAudio()
        {
            m_AudioSource.Stop();
            PlayingAudio = false;
        }

        Vector3 lastPos;
        void Update()
        {
            //if (m_WheelCollider.attachedRigidbody.velocity.magnitude < 2)
            //{
            //    skidParticles.gameObject.SetActive(false);

            //}
            //else
            //    skidParticles.gameObject.SetActive(true);
            WheelHit hit;
            if (m_WheelCollider.GetGroundHit(out hit))
            {
                //if (m_WheelCollider.attachedRigidbody.velocity.magnitude > 2)
                //{
                //    GameObject skidmark = Instantiate(SkidTrailPrefab.gameObject);
                //    skidmark.transform.position = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
                //    skidmark.transform.eulerAngles = new Vector3(skidmark.transform.eulerAngles.x + 90f, skidmark.transform.eulerAngles.y, skidmark.transform.eulerAngles.z);
                //    skidmark.transform.LookAt(transform.up);

                //    Destroy(skidmark, 2f);
                //}
                 




                //cube.transform.localScale = Vector3(1.25, 1.5, 1);
            }
        }
        public IEnumerator StartSkidTrail()
        {
            //skidding = true;
            //m_SkidTrail = Instantiate(SkidTrailPrefab);
            //while (m_SkidTrail == null)
            //{
            //    yield return null;
            //}
            //m_SkidTrail.parent = transform;
            //m_SkidTrail.localPosition = -Vector3.forward * (m_WheelCollider.radius );
            yield return null;
        }


        public void EndSkidTrail()
        {
            if (!skidding)
            {
                return;
            }
            //skidding = false;
            //m_SkidTrail.parent = skidTrailsDetachedParent;
            //Destroy(m_SkidTrail.gameObject, 10);
        }
    }
}
