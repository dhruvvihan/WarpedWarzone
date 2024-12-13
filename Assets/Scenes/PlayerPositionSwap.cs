using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerPositionSwap : MonoBehaviourPun
{
    public float maxSwapRange = 10f;  // Max range for swapping
    private bool isCooldown = false;  // Cooldown flag
    private float cooldownTime = 120f; // 2 minutes cooldown

    void Update()
    {
        // Check for ability trigger (e.g., press "F" key)
        if (Input.GetKeyDown(KeyCode.F) && !isCooldown)
        {
            TrySwap();
        }
    }

    void TrySwap()
    {
        // Cast a ray to find the target player (instead of using LayerMask, we'll check for the Player tag)
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxSwapRange))
        {
            GameObject targetPlayer = hit.collider.gameObject;

            // Check if the hit object has a PhotonView and is tagged as "Player"
            if (targetPlayer != null && targetPlayer.CompareTag("Player") && targetPlayer != this.gameObject)
            {
                // Swap positions via RPC (Remote Procedure Call)
                photonView.RPC("SwapPositions", RpcTarget.All, targetPlayer.GetPhotonView().ViewID);
                StartCoroutine(StartCooldown());
            }
            else
            {
                Debug.Log("No valid player found for swapping.");
            }
        }
        else
        {
            Debug.Log("No player in front to swap with.");
        }
    }

    [PunRPC]
    void SwapPositions(int targetViewID)
    {
        // Find the target player using their PhotonView ID
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);
        if (targetPhotonView != null)
        {
            GameObject targetPlayer = targetPhotonView.gameObject;

            // Swap positions
            Vector3 tempPosition = targetPlayer.transform.position;
            targetPlayer.transform.position = this.transform.position;
            this.transform.position = tempPosition;

            Debug.Log("Swapped positions with: " + targetPlayer.name);
        }
    }

    IEnumerator StartCooldown()
    {
        isCooldown = true;
        Debug.Log("Swap ability on cooldown for 2 minutes.");

        // Wait for 120 seconds (2 minutes)
        yield return new WaitForSeconds(cooldownTime);

        isCooldown = false;
        Debug.Log("Swap ability ready to use.");
    }
}
