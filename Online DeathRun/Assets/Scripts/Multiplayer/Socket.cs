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

    //La funzione start viene chiamata all'avvio dell'applicazione
    void Start()
    {

        socket = new WebSocket("ws://deathrun-server.glitch.me");
        socket.Connect();

        //WebSocket onMessage function
        socket.OnMessage += (sender, e) =>
        {

            //Se il tipo di dato è testo
            if (e.IsText)
            {
                JObject jsonObj = JObject.Parse(e.Data);

                if (jsonObj["id"] != null)
                {
                    //conversione dei dati iniziali del player ricevuti dal server da JSon a playerData Object
                    PlayerData tempPlayerData = JsonUtility.FromJson<PlayerData>(e.Data);
                    playerData = tempPlayerData;
                    Debug.Log("player ID is " + playerData.id);
                    return;
                }

            }

        };

        //in caso di chiusura della connessione
        socket.OnClose += (sender, e) =>
        {
            Debug.Log(e.Code);
            Debug.Log(e.Reason);
            Debug.Log("Connection Closed!");
        };
    }

    //La funzione update esegue le istruzioni una volta per frame
    void Update()
    {
        if (socket == null)
        {
            return;
        }

        //se il player viene configurato correttamente
        if (player != null && playerData.id != "")
        {
            //ottenimento della posizione del player e del timestamp attuale
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
            string messageJSON = "{\"message\": \"Messaggio dal client\"}";
            socket.Send(messageJSON);
        }
    }

    private void OnDestroy()
    {
        socket.Close();
    }

}