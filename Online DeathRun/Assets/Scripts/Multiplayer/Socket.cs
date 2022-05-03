using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json.Linq;

public class Socket : MonoBehaviour
{
    WebSocket socket;
    public GameObject player;
    public PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {

        socket = new WebSocket("ws://deathrun-server.glitch.me");
        socket.Connect();

        socket.OnMessage += (sender, e) =>
        {

            //If received data is type text...
            if (e.IsText)
            {
                JObject jsonObj = JObject.Parse(e.Data);
                
                //ottenimento id del server dati
                if (jsonObj["id"] != null)
                {
                    //conversione dati player iniziali da json a PlayerData object
                    PlayerData tempPlayerData = JsonUtility.FromJson<PlayerData>(e.Data);
                    playerData = tempPlayerData;
                    Debug.Log("player ID is " + playerData.id);
                    return;
                }

            }

        };

        //quando la connessione si chiude
        socket.OnClose += (sender, e) =>
        {
            Debug.Log(e.Code);
            Debug.Log(e.Reason);
            Debug.Log("Connection Closed!");
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (socket == null)
        {
            return;
        }

        //se il player è stato caricato correttamente inizio a inviare i dati al server
        if (player != null && playerData.id != "")
        {
            //Prendo la posizione del player
            playerData.xPos = player.transform.position.x;
            playerData.yPos = player.transform.position.y;
            playerData.zPos = player.transform.position.z;

            System.DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
            double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
            playerData.timestamp = timestamp;

            string playerDataJSON = JsonUtility.ToJson(playerData);
            socket.Send(playerDataJSON);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            string messageJSON = "{\"message\": \"Some Message From Client\"}";
            socket.Send(messageJSON);
        }
    }

    private void OnDestroy()
    {
        //chiusura del socket
        socket.Close();
    }

}
