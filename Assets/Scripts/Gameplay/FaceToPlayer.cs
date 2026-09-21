using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceToPlayer : MonoBehaviour
{
    public Vector3 moveDir;
    GameObject player;
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.LogError("Cannot find player  with tag");

        rectTransform = GetComponent<RectTransform>();
        moveDir = this.transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 dirToPlayer = player.transform.position - rectTransform.position;

        
        //moveDir =   player.transform.position -this.transform.position;
        this.transform.position = player.transform.position + moveDir;
        rectTransform.position = player.transform.position + moveDir;
    }
}