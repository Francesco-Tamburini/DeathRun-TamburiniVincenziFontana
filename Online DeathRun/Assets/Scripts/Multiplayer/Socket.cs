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

    //La funzione start viene chiamata all'avvio del programma
    void Start()
    {

        socket = new WebSocket("ws://localhost:8080");
        socket.Connect();

        //WebSocket onMessage function
        socket.OnMessage += (sender, e) =>
        {
            if (e.IsText)
            {
                JObject jsonObj = JObject.Parse(e.Data);

                //Ottenimento dell'id del server
                if (jsonObj["id"] != null)
                {
                    //conversione di playerdata
                    PlayerData tempPlayerData = JsonUtility.FromJson<PlayerData>(e.Data);
                    playerData = tempPlayerData;
                    Debug.Log("player ID is " + playerData.id);
                    return;
                }

            }

        };

        socket.OnClose += (sender, e) =>
        {
            Debug.Log(e.Code);
            Debug.Log(e.Reason);
            Debug.Log("Connection Closed!");
        };
    }

    //La funzione update viene chiamata una volta per frame
    void Update()
    {
        if (socket == null)
        {
            return;
        }

        //Se il player viene caricato correttamente vengono inviati i dati al server
        if (player != null && playerData.id != "")
        {
            //Ottenimento della posizione del player
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
            string messageJSON = "{\"message\": \"Messaggio dal client\"}";
            socket.Send(messageJSON);
        }
    }

    private void OnDestroy()
    {
        //chiusura del socket con chiusura dell'app
        socket.Close();
    }

}