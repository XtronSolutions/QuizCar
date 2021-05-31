// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmoothSyncMovement.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
//  Smoothed out movement for network gameobjects
// </summary>                                                                                             
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Smoothed out movement for network gameobjects
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class SmoothSyncMovement : Photon.Pun.MonoBehaviourPun, IPunObservable
    {
        public float SmoothingDelay = 5;
		public Transform transformToObserve;

        public void Awake()
        {
            bool observed = false;
            foreach (Component observedComponent in this.photonView.ObservedComponents)
            {
                if (observedComponent == this)
                {
                    observed = true;
                    break;
                }
            }
            if (!observed)
            {
                Debug.LogWarning(this + " is not observed by this object's photonView! OnPhotonSerializeView() in this class won't be used.");
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                //We own this player: send the others our data
				stream.SendNext(transformToObserve.position);
				stream.SendNext(transformToObserve.rotation);
            }
            else
            {
                //Network player, receive data
                correctPlayerPos = (Vector3)stream.ReceiveNext();
                correctPlayerRot = (Quaternion)stream.ReceiveNext();
            }
        }

        private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
        private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

        public void Update()
        {
            if (!photonView.IsMine)
            {   
				if (transformToObserve == null)
					return;

				//Update remote player (smooth this, this looks good, at the cost of some accuracy)
				transformToObserve.position = Vector3.Lerp(transformToObserve.position, correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
				transformToObserve.rotation = Quaternion.Lerp(transformToObserve.rotation, correctPlayerRot, Time.deltaTime * this.SmoothingDelay);
            }
        }

    }
}