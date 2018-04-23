using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour {

	public Transform buttonPrefab;
	public Transform soundbankPrefab;
	public Text description;
	float yValue;

	void Awake () { 
		description.text = "" + Application.persistentDataPath;

		// GET MIDI LIST

 		DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/midi/");
 		FileInfo[] info = dir.GetFiles("*.mid");

 		DirectoryInfo dir2 = new DirectoryInfo(Application.persistentDataPath + "/");
 		FileInfo[] info2 = dir2.GetFiles("*.mid");

 		RectTransform buttonParent = buttonPrefab.parent.GetComponent<RectTransform>();
 		buttonParent.sizeDelta = new Vector2 (buttonParent.sizeDelta.x, ((info.Length + info2.Length) * 30));

 		AddEntry(info, buttonPrefab, true, Application.streamingAssetsPath + "/midi/");
 		AddEntry(info2, buttonPrefab, false, Application.persistentDataPath + "/");

 		buttonPrefab.gameObject.SetActive(false);

 		// GET SOUND BANK LIST

 		DirectoryInfo dir3 = new DirectoryInfo(Application.streamingAssetsPath + "/CSharpSynth/");
 		FileInfo[] info3 = dir3.GetFiles("*.sf2");

 		RectTransform soundbankParent = soundbankPrefab.parent.GetComponent<RectTransform>();
 		soundbankParent.sizeDelta = new Vector2 (soundbankParent.sizeDelta.x, (info3.Length * 30));

 		AddEntry(info3, soundbankPrefab, true, Application.streamingAssetsPath, false);

 		soundbankPrefab.gameObject.SetActive(false);
	}

	void AddEntry(FileInfo[] info, Transform prefab, bool resetY, string path, bool isMidi = true) {
		if (resetY) {
			yValue = -15;
		}
		bool first = true;
 		foreach (FileInfo f in info) {
 			Vector3 pos = prefab.transform.position;
 			Transform clone = Instantiate(prefab, Vector3.zero, transform.rotation) as Transform;
 			clone.SetParent(prefab.parent.transform, false);
 			clone.name = path;
 			clone.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,yValue,0);
 			yValue -= 30;

 			if (first && resetY) {
 				clone.GetComponent<Toggle>().isOn = true;
 				first = false;
 				if (isMidi) {
 					CurrentMidi.path = path;
 					CurrentMidi.currentMidiTitle = f.Name;
				} else {
 					CurrentMidi.currentSoundbankTitle = f.Name;
				}
 			}

 			foreach (Transform child in clone) {
 				if (child.tag == "ButtonLabel") {
 					child.GetComponent<Text>().text = f.Name;
 				} else if (child.tag == "ButtonScore") {
 					if (PlayerPrefs.HasKey(f.Name + "score")) {
 						child.GetComponent<Text>().text = "High Score: " + PlayerPrefs.GetInt(f.Name + "score");
 					}
 				}
 			}
 		}
	}

	public void SetSong(Text buttonText) {
		CurrentMidi.currentMidiTitle = buttonText.text;
 		CurrentMidi.path = buttonText.transform.parent.transform.name;
	}

	public void SetSoundbank(Text buttonText) {
		CurrentMidi.currentSoundbankTitle = buttonText.text;
		PlayerPrefs.SetString("soundbank", CurrentMidi.currentSoundbankTitle);
	}

	public void LoadIt() {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
	}

	public void LoadCredits() {
        SceneManager.LoadScene(2, LoadSceneMode.Single);
	}

	void Update() {
		if (Input.GetKeyDown("escape")) {
			Application.Quit();
		}
	}
}
