using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player
{
	public string playerName;
	public GameObject avatar;
	public int connectionId;
}

public class Client : MonoBehaviour 
{
	private const int MAX_CONNECTION = 100;

	private int port = 7777;

	private int hostId;
	private int webHostId;

	private int reliableChannel;
	private int unrealiableChannel;

	private int ourClientId;
	private int connectionId;

	private float connectionTime;
	private bool isConnected = false;
	private bool isStarted = false;
	private byte error;

	private string playerName;

	public GameObject playerPrefab;
	public Dictionary<int,Player> players = new Dictionary<int, Player>();

	private float lastMovementUpdate;
	public float movementUpdateRate = 0.1f;

	private string cmsg1 = " ";
	private string cmsg2 = " ";
	private string cmsg3 = " ";

	public void Connect()
	{
		//Check if player has a name
		string pName = GameObject.Find("NameInput").GetComponent<InputField>().text;
		if (pName == "") 
		{
			Debug.Log ("You must enter a name!");
			//return;
		}

		playerName = pName;

		NetworkTransport.Init ();
		ConnectionConfig cc = new ConnectionConfig ();


		reliableChannel = cc.AddChannel (QosType.Reliable);
		unrealiableChannel = cc.AddChannel (QosType.Unreliable);

		HostTopology topo = new HostTopology (cc, MAX_CONNECTION);

		hostId = NetworkTransport.AddHost (topo, 0);
		connectionId = NetworkTransport.Connect (hostId, "10.248.160.134", port, 0, out error);

		connectionTime = Time.time;
		isConnected = true;
	}

	public void SendMessage()
	{
		string inputMessage =  "MESSAGETOSERVER|" + playerName +'|'+ GameObject.Find("MessageField").GetComponent<InputField>().text;
		
		Send(inputMessage, unrealiableChannel);
	}

	public void ReceiveMessage(string msg)
	{
		
		cmsg1 = cmsg2;
		cmsg2 = cmsg3;
		cmsg3 = msg;
		GameObject.Find("Message1").GetComponent<UnityEngine.UI.Text>().text = cmsg1;
		GameObject.Find("Message2").GetComponent<UnityEngine.UI.Text>().text = cmsg2;
		GameObject.Find("Message3").GetComponent<UnityEngine.UI.Text>().text = cmsg3;

	}

	private void Update()
	{
		if (!isConnected)
			return;

		int recHostId; 
		int connectionId; 
		int channelId; 
		byte[] recBuffer = new byte[1024]; 
		int bufferSize = 1024;
		int dataSize;
		byte error;
		NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
		switch (recData)
		{
		case NetworkEventType.Nothing:         //1
			break;
		case NetworkEventType.ConnectEvent:    //2
			break;
		case NetworkEventType.DataEvent:       //3
			string msg = System.Text.Encoding.Unicode.GetString(recBuffer, 0, dataSize);
			Debug.Log("Receiving : " + msg);
			//this is cool shit
			string[] splitData = msg.Split('|');

			switch(splitData[0])
			{
				case "ASKNAME":
					OnAskName(splitData);
					break;
				case "CNN":
					//Spawning Player
					SpawnPlayer(splitData[1], int.Parse(splitData[2]));
					break;
				case "ASKPOSITION":
					OnAskPosition(splitData);
						break;
				case "ChatFromServer":
						ReceiveMessage(splitData[1]);
						break;
				case "DC":
					PlayerDisconnected(int.Parse(splitData[1]));
					break;
				default:
					Debug.Log("Inalid message : " + msg);
					break;
			}
			break;
		case NetworkEventType.DisconnectEvent: //4
			break;
		}
		if (Time.time - lastMovementUpdate > movementUpdateRate)
		{
			lastMovementUpdate = Time.time;
			OnAskPosition();
		}
	}

	private void Send(string message, int channelId)
	{
		Debug.Log("Sending : " + message);
		byte[] msg = Encoding.Unicode.GetBytes(message);
		NetworkTransport.Send(hostId,connectionId,channelId,msg,message.Length * sizeof(char), out error);
	}

	private void OnAskName(string[] data)
	{
		//Getting the local ID
		ourClientId = int.Parse(data[1]);
		//Send our name to the server
		Debug.Log(playerName);
		Send("NAMEIS|" + playerName, reliableChannel);
		//Create all the other player
		for(int i = 2; i < data.Length -1; i++)
		{
			string[] d = data[i].Split('%');
			SpawnPlayer(d[0],int.Parse(d[1]));
		}
	}
	private void OnAskPosition()	{
		// Send local players position
		Vector3 myPosition = players[ourClientId].avatar.transform.position;
		string m = "MYPOSITION|" + myPosition.x.ToString() + '|' + myPosition.y.ToString() + '|' + myPosition.z.ToString();
		Send(m, unrealiableChannel);
	}

	private void OnAskPosition(string[] data)
	{
		if(!isStarted)
			return;

		for (int i = 1; i < data.Length; i++)
		{
			string[] d = data[i].Split('%');
			// Prevent the server from update
			if(ourClientId != int.Parse(d[0]))
			{
				
				Vector3 position = Vector3.zero;
				position.x = float.Parse(d[1]);
				position.y = float.Parse(d[2]);
				position.z = float.Parse(d[3]);
				//Now 
				players[int.Parse(d[0])].avatar.transform.position = position;
			}
		}
		// Send local players position
		Vector3 myPosition = players[ourClientId].avatar.transform.position;
		string m = "MYPOSITION|" + myPosition.x.ToString() + '|' + myPosition.y.ToString() + '|' + myPosition.z.ToString();
		Send(m, unrealiableChannel);
	}

	private void SpawnPlayer(string playerName, int cnnId)
	{
		GameObject go = Instantiate(playerPrefab) as GameObject;

		//Check if this is the local player
		if(cnnId == ourClientId)
		{
			//Add mobility
			go.AddComponent<PlayerMotor>();
			//Remove Canvas
			GameObject.Find("Canvas").SetActive(false);
			//Add Chat Canvas
			//GameObject.Find("MessageHud").SetActive(true);
			//Set Active
			isStarted = true;
		}

		//If remote player
		Player p = new Player();
		p.avatar = go;
		p.playerName = playerName;
		p.connectionId = cnnId;
		//Adding name over player
		p.avatar.GetComponentInChildren<TextMesh>().text = playerName;
		//Adding new players stats to the list of players
		players.Add(cnnId,p);
	}

	private void PlayerDisconnected(int cnnId)
	{
		Destroy(players[cnnId].avatar);
		players.Remove(cnnId);
	}
}
//Unity Multiplayer (UNET) - Transport Layer API [C#][Stream VOD]