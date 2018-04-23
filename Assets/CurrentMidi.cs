using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentMidi : MonoBehaviour {

    private static CurrentMidi instance;
    public static string currentMidiTitle;
    public static string path;
    public static string currentSoundbankTitle;

	void Awake() {	
        if(!instance) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
	}
}
