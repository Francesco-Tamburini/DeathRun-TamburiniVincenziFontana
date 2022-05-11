
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
        
        //Funzione standard OnMessage websocket
        socket.OnMessage += (sender, e) =>
        {
            //Se il tipo di dato ricevuto è testo
            if (e.IsText)
            {
                JObject jsonObj = JObject.Parse(e.Data);

                //ottenimento dell'id del server
                if (jsonObj["id"] != null)
                {
                    //conversione dei dati del player da json a oggetto playerData
                    PlayerData tempPlayerData = JsonUtility.FromJson<PlayerData>(e.Data);
                    playerData = tempPlayerData;
                    Debug.Log("player ID is " + playerData.id);
                    return;
                }

            }

        };

        //In caso di disconnessione del client
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

        //Se il player viene configurato in maniera corretta si procede con l'ottenimento dei dati
        if (player != null && playerData.id != "")
        {
            //Ottenimento della posizione del player e del timestamp del messaggio
            playerData.xPos = player.transform.position.x;
            playerData.yPos = player.transform.position.y;
            playerData.zPos = player.transform.position.z;

            System.DateTime epochStart =  new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
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
        socket.Close();
    }

}