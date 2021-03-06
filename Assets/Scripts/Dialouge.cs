﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Dialouge : MonoBehaviour {
	//StreamReader reader = new StreamReader("file.txt");
	bool talk = false, npcTalk = false, playerTalk = false, proceed = false;
	public bool talking = false;
	private Vector2 pos;
	public TextAsset textFile;
	string[] dialogLines;
	string textDisplayed;
	bool check = true;
	string npcName;
	public Material render;

	public float letterPause = 0.1f;

	TypeText typeWriter;

	bool option2 = false, option3 = false, choice = false, option1 = false;

	public string tag;
	public GUISkin talkBox = null;
	public GUISkin nameBox = null;

	bool force = false;

	public itemCheck checker;
	public ScoreMaster master;

	void Start () {
		string a = "aaaabv";
		print (a);
		//Records player pos	ition, and transfers speech data from file to array
		pos = transform.position;
		string text = textFile.text;
		if (textFile != null) {
			// Add each line of the text file to
			// the array using the new line
			// as the delimiter
			dialogLines = Regex.Split (text, "\r\n");
			print ("Done");
		}
		tag = Application.loadedLevelName;
		checkFile (tag);
	}

	void OnGUI()
	{

		if (npcTalk) {
			render.mainTexture = Resources.Load(npcName) as Texture2D;
			GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
			GUI.DrawTexture(new Rect(Screen.width-500, Screen.height - 576, 500, 576), render.mainTexture);
			GUI.skin = talkBox;
			GUI.Box (new Rect (0, Screen.height - Screen.height/4, Screen.width-500, Screen.height/4), textDisplayed);
			GUI.skin = nameBox;
			GUI.Box (new Rect (Screen.width - 800, (Screen.height - Screen.height/4 - 75), 300, 75), npcName);

		}
		if (playerTalk) {
			render.mainTexture = Resources.Load("protag") as Texture2D;
			GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
			GUI.DrawTexture(new Rect(0,Screen.height - 576, 500, 576), render.mainTexture);
			GUI.skin = talkBox;
			GUI.Box (new Rect (500, Screen.height - Screen.height/4, Screen.width-500, Screen.height/4), textDisplayed);
			GUI.skin = nameBox;
			GUI.Box (new Rect (500, Screen.height - Screen.height/4 - 75, 300, 75), "Hunter");
		}
	}


	//When you approach an NPC collider, the talk variable is set to true
	//And that NPC's tag is recorded
	bool once = true;
	void OnTriggerEnter(Collider other) {
		tag = other.tag;
		talk = true;
		checker.checkItem (tag);

		if (tag.Contains("Voice") && once){
				force = true;
			once = false;
				checkFile (tag);
			}
		}

	//When you leave, talk is set to false

	void OnTriggerExit(Collider other)
	{
		talk = false;
	}
	
	//The tag is then used in the checkFile method to see who we are talking to
	void Update () {

		if (Input.GetKey (KeyCode.E) && talk){
			print(tag);
			checkFile(tag);
		}


//		//Example of how this system has NPC's display alternate dialog.
//		//If a certain condition is met (in this case, a collectable in the move class is collected)
//		//Then the code will search for the tag of the NPC whoose dialog changes
//		//And then changes the tag
//		//Due to the way the dialog system works, this will change what the NPC says
//		//Reasonably elegant and efficient, if a bit hard-code-y
//		//The check variable is present so this change only happens once, and not over and over
//		if (PlayerMove2.collected && check) {
//			GameObject Text2 = GameObject.FindWithTag("Text2");
//			Text2.tag = Text2.tag + "A";
//			print (Text2.tag);
//			check = false;
//		}
	}

	void checkFile(string start)
	{
		//Start at the beginning of the file
		int length = dialogLines.GetLength(0);

		for (int i = 0; i < length; ++i) {
			//Search through the file, and when the tag variable of the NPC you are interacting with matches the line
			string temp = dialogLines[i];
			print (tag.Equals(temp));
			print (tag);
			print (temp);
			//Halt player movement, set relevant variables, and read the dialog
			if (temp.Equals(tag))
			{
				PlayerMoveAnimated.canMove = false;
				PlayerMove2.isPaused = true;
				talking = true;
				textDisplayed = temp;
				readDialog(i);
			}
		}
		if (tag == "Young Man")
			master.getKnifeFromNPC ();
		master.checkCritical ();
		talk = false;
	}

	void readDialog(int line)
	{
		string temp = dialogLines [line+1];
		//Debug.Log(temp);

		//Depending on who is talking first, the relevant dialog box is displayed
		//In both cases, starts the dialog coroutine
		if (temp.Contains ("NPC") || temp.Contains("Bartend") || temp.Contains("Scruffy")) {
			temp = dialogLines [line];
			npcName = temp;
			//npcTalk = true;
			//Debug.Log("Proceed1");
			StartCoroutine(npcTalking (line));
		}
		if (temp.Contains("Hunter"))
		    {
			temp = dialogLines [line];
			npcName = temp;
			StartCoroutine(npcTalking (line));
			//playerTalk = true;
			//playerTalking(line);
		}
	}

	IEnumerator npcTalking(int line)
	{
		string temp;
		while (true) {
			choice = false;
			option2 = false;
			option3 = false;
			line++;
			temp = dialogLines [line];
			//print ("TEMP:" + temp);
			//print (temp);
			temp = temp.Replace("\r", "").Replace("\n", "");
			//Bool variables display relevant dialog boxes depending on who is talking
			//Then fetches the first line of dialog from that character
			if (temp.Equals ("Hunter"))
			{

				playerTalk = true;
				npcTalk=false;
				line++;
				temp = dialogLines [line];
			}
			if (temp.Equals ("NPC") || temp.Equals("Bartend") || temp.Equals("Scruffy")) {
				playerTalk = false;
				npcTalk= true;
				line++;
				temp = dialogLines [line];
			}

			if (temp.Equals ("Change"))
			{
				playerTalk = false;
				npcTalk= false;
				talking = false;
				PlayerMove2.isPaused = false;
				PlayerMoveAnimated.canMove = true;
				line++;
				temp = dialogLines[line++];
				print (temp.Equals("1.BarScene"));
				string newTemp = temp.Replace("\r", "").Replace("\n", "");
				print (newTemp.Equals("1.BarScene"));
				print (temp);
				print (newTemp);
				print ("Loading Level");
				Application.LoadLevel(newTemp);
				
				return true;
			}
			//If file reads end, ends the conversation
			if (temp.Equals ("End"))
			{
				playerTalk = false;
				npcTalk= false;
				talking = false;
				PlayerMove2.isPaused = false;
				PlayerMoveAnimated.canMove = true;
					if (force)
					{
					line++;
						temp = dialogLines[line++];
					string newTemp = temp.Replace("\r", "").Replace("\n", "");
					print (newTemp);
					print (newTemp.Equals("1.BarScene"));
					if (!newTemp.Equals("None") || !newTemp.Contains ("Kid"))
					    	//Application.LoadLevel(newTemp);
					force = false;
					}
				return true;
			}

			
			//Leads into multiple dialog choice
			if (temp.Contains ("PickOption"))
			{
				choice = true;
				line++;
				temp = dialogLines [line];
				print ("TEMP:" + temp);
			}
							//	textDisplayed = temp;
			yield return StartCoroutine(TypeHelper(temp, playerTalk, npcTalk, npcName));
			//Waits so that dialog flows properly
			yield return new WaitForSeconds(0.5f);
			//And does not progress until player hits key
			yield return StartCoroutine (WaitForKeyPress ());
			_keyPressed = false;

			//If a multiple dialog tree, goes through file to find the right dialog tree
			//Was having issues with .equals, so using .contains
			//Can have issues if we ever use the string of characters in it,
			//But if so, we just change the delimiters
			//Easy fix
			if (choice)
			{
				int val = 1;
				if (option2)
				{
					val = 2;
					bool found = false;
					while(!found)
					{
						line++;
						if (dialogLines[line].Contains("0100")) 
						{
							found = true;
							//line++;
						}
					}
					//line = line + 10;
				}
				if (option3)
				{
					bool found = false;
					while(!found)
					{
						line++;
						if (dialogLines[line].Contains("0010")) 
						{
							found = true;
							//line++;
						}
					}
					//line = line + 10;
				}
				master.Score(val);
			}
			//Debug.Log ("Proceed2");
		}

	}

	bool _keyPressed = false;

	//Waits for player to press key
	//If a dialog choice, lets player pick choice
	public IEnumerator WaitForKeyPress()
	{
		while(!_keyPressed)
		{
			if(Input.GetKeyDown(KeyCode.Z))
			{
				_keyPressed = true;
				//print("Pressed.");
				break;
			}
			if(Input.GetKeyDown(KeyCode.X))
			{
				_keyPressed = true;
				option2 = true;
				//print("Pressed.");
				break;
			}
			if(Input.GetKeyDown(KeyCode.C))
			{
				_keyPressed = true;
				option3 = true;
				//print("Pressed.");
				break;
			}
			//print("Awaiting key input.");
			yield return 0;
		}
	}


	IEnumerator TypeHelper (string line, bool player, bool npc, string npcNm)  //responsible for printing each letter individually
	{
		textDisplayed = "";
		npcTalk = npc;
		playerTalk = player;
		npcName = npcNm;
		foreach (char letter in line.ToCharArray())
			//the line referred to here is the same line referred to in DialogueBox.cs
		{
			//this entire foreach loop is responsible for printing the message string
			//if (sound)
			//	audio.PlayOneShot (sound); 
			//for example, a typewriter sound, set in editor
			textDisplayed = textDisplayed + letter;
			//yield return 0;
			yield return new WaitForSeconds (letterPause); 
			//speed of printing, set in editor
		}
	}	

	
}
