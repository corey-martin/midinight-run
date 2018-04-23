using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour {

	public void LoadMainMenu() {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
	}
}
