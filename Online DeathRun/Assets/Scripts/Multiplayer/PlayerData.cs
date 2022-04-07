using System;

//dati del player inviati al socket
[Serializable]
public struct PlayerData
{
    public string id;
    public float xPos;
    public float yPos;
    public float zPos;
    public double timestamp;
}

