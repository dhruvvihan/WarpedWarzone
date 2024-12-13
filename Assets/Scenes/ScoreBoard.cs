using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ScoreBoard : MonoBehaviour
{
    public static ScoreBoard Instance;

    public Text blueTeamText;
    public Text redTeamText;

    public int blueTeamScore = 0;
    public int redTeamScore = 0;

    private PhotonView view;

    void Awake()
    {
        view = GetComponent<PhotonView>();
        Instance = this;
    }

    public void PlayerDied(int playerTeam)
    {
        if (playerTeam == 2)
        {
            blueTeamScore++;
        }

        if (playerTeam == 1)
        {
            redTeamScore++;
        }

        view.RPC("UpdateScore", RpcTarget.All, blueTeamScore, redTeamScore);
    }

    [PunRPC]
    void UpdateScore(int blueScore, int redScore)
    {
        blueTeamScore = blueScore;
        redTeamScore = redScore;

        blueTeamText.text = blueTeamScore.ToString();
        redTeamText.text = redTeamScore.ToString();
    }
}
