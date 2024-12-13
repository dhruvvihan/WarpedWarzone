using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsernameDisplay : MonoBehaviour
{
    public Text usernameText;
    public Text teamText;
    public PhotonView view;

    void Start()
    {
        if (view.IsMine)
        {
            gameObject.SetActive(false);
        }

        usernameText.text = view.Owner.NickName;

        if (view.Owner.CustomProperties.ContainsKey("Team"))
        {
            int team = (int)view.Owner.CustomProperties["Team"];
            teamText.text = "Team " + team;
        }

    }

}
