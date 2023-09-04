using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LivingEntity : MonoBehaviourPun
{
    [PunRPC]

    public virtual void ChangeProp()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ChangeProp", RpcTarget.Others);
        }
    }
}
