using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerUserNameManager : MonoBehaviour
{
    [SerializeField] private InputField usernameInput;
    [SerializeField] private Text errorText;

    void Start()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = PlayerPrefs.GetString("username");
        }
    }

    public void PlayerUsernameInputValueChanged()
    {
        string username = usernameInput.text;
        if (!string.IsNullOrEmpty(username) && username.Length <= 20)
        {
            PhotonNetwork.NickName = username;
            PlayerPrefs.SetString("username", username);
            errorText.text =  "" ;
            MenuManager.instance.OpenMenu("TitleMenu");
        }
        else
        {
            errorText.text = "Username Must Not Be Empty Or More Than 20 Charachters";
        }
    }
}
