var uuid = require('uuid-random');
const WebSocket = require('ws')

const wss = new WebSocket.WebSocketServer({ port: 8080 }, () => {
    console.log('server started')
})

//Object that stores player data 
var playersData = {
    "type": "playerData"
}

wss.on('connection', function connection(client) {

    //Creazione ID del player
    client.id = uuid();

    console.log(`Client ${client.id} Connected!`)

    var currentClient = playersData["" + client.id]

    //Invio dell'id del player al client
    client.send(`{"id": "${client.id}"}`)

    //Ricevimento messaggi del player
    client.on('message', (data) => {
        var dataJSON = JSON.parse(data)

        console.log("Player Message")
        console.log(dataJSON)
    })

    //Notifica al client di disconnessione
    client.on('close', () => {
        console.log('This Connection Closed!')
        console.log("Removing Client: " + client.id)
    })

})

wss.on('listening', () => {
    console.log('In ascolto su porta 8080')
})