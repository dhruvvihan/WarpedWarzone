using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.UI;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PlayerController : MonoBehaviourPunCallbacks
{
    PhotonView view;
    GameObject controller;
    public int playerTeam;
    private Dictionary<int,int> playerTeams = new Dictionary<int,int>();

    void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    void Start()
    {
        if(view.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            playerTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        }
        AssignPlayerToSpawnArea(playerTeam);
    }

    void AssignPlayerToSpawnArea(int team)
    {

        GameObject spawnArea1 = GameObject.Find("SpawnArea1");
        GameObject spawnArea2 = GameObject.Find("SpawnArea2");

        if(spawnArea1 == null || spawnArea2 == null)
        {
            Debug.Log("Error Spawn Area Not Found");
            return;
        }

        Transform spawnPoint = null;

        if(team == 1)
        {
            spawnPoint = spawnArea1.transform.GetChild(Random.Range(0, spawnArea1.transform.childCount));
        }

        if (team == 2)
        {
            spawnPoint = spawnArea2.transform.GetChild(Random.Range(0, spawnArea2.transform.childCount));
        }

        if (spawnPoint != null)
        {
            controller = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Player"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { view.ViewID });
            Debug.Log("Instantiated Player Controller At Spawn Point");
        }
        else
        {
            Debug.LogError("No Available Spawn Point For Team " + team);
        }

    }

    void AssignTeamToAllPlayers()
    {
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Team"))
            {
                int team = (int)player.CustomProperties["Team"];
                playerTeams[player.ActorNumber] = team;

                AssignPlayerToSpawnArea(team);
            }
        }
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }

}
