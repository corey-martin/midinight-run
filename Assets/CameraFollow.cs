using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityMidi;

public class CameraFollow : MonoBehaviour {

	public MidiPlayer midiPlayer;
	float zPosStart;
	float zPosEnd;
    float speed = 90;
    float startTime;
    float journeyLength;

    public GameObject failTextObj;
    public Text scoreText;
    int score = 0;
    public Text successText;
    public Text multiplierText;

    BoxCollider myCol;

    bool dead = false;
    bool done = false;

    public Transform walls;

    AudioSource audioSource;
    float distCovered = 0;
    float z = 0;

    void Awake() {
    	audioSource = GetComponent<AudioSource>();

		if (!PlayerPrefs.HasKey(CurrentMidi.currentMidiTitle + "score")) {
			PlayerPrefs.SetInt(CurrentMidi.currentMidiTitle + "score", score);
		}
    }

    void Start() {
        startTime = Time.time;
		zPosStart = transform.position.z;
		zPosEnd = zPosStart + midiPlayer.MidiFile.Tracks[0].MidiEvents.Length + 250;
		Debug.Log(zPosEnd);
        journeyLength = (zPosEnd - zPosStart);
        failTextObj.SetActive(false);
        successText.gameObject.SetActive(false);
        multiplierText.gameObject.SetActive(false);

        myCol = GetComponent<BoxCollider>();
        myCol.enabled = false;
        StartCoroutine(StartGame());
	}

	IEnumerator StartGame() {
		yield return new WaitForSeconds(1);
		myCol.enabled = true;
	}

	void Update() {
		if (!dead && !done) {
	        distCovered += Time.deltaTime * speed;
	        float fracJourney = distCovered / journeyLength;
	        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Lerp(zPosStart, zPosEnd, fracJourney));
	        walls.position = new Vector3(walls.position.x, walls.position.y, Mathf.Lerp(zPosStart, zPosEnd, fracJourney));

	        if (Input.GetAxis("Horizontal") != 0) {
	        	transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, 0);
	        }

	        int limit = 95;
	        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -limit, limit), transform.position.y, transform.position.z);

			if (Input.GetKey("space")) {
				if (Input.GetKey("up")) {
					speed = 300;
					multiplierText.text = "4X Multiplier";
				} else {
					speed = 160;
					multiplierText.text = "2X Multiplier";
				}
				multiplierText.gameObject.SetActive(true);
			} else {
				speed = 90;
				multiplierText.gameObject.SetActive(false);
			}


			int zDiff = (int)Mathf.Floor((distCovered - z) * speed);
			score += zDiff;
			scoreText.text = "Score: " + score;
			if (score > PlayerPrefs.GetInt(CurrentMidi.currentMidiTitle + "score")) {
				PlayerPrefs.SetInt(CurrentMidi.currentMidiTitle + "score", score);
			}
			z = distCovered;
		}

		if ((dead || successText.gameObject.activeSelf) && Input.GetKeyDown("space")) {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
		}

		if (Input.GetKeyDown("escape")) {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Pickup") {
			audioSource.Play();
			if (speed > 100) {
				score += 20000;
			} else if (speed > 200) {
				score += 50000;
			} else {
				score += 10000;
			}

		} else if (other.gameObject.tag == "Death") {
			failTextObj.SetActive(true);
			dead = true;

		} else if (other.gameObject.tag == "EndZone") {
			successText.gameObject.SetActive(true);
			done = true;
		} else {
			// 
		}
	}
}
