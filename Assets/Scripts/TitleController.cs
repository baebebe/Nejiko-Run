using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TitleController : MonoBehaviour {
	public Text highScoreLabel;

	public void Start(){
		// 하이 스코어를 표시
		highScoreLabel.text = "High Score : " +  PlayerPrefs.GetInt("HighScore") + "m";
	}

	public void OnStartButtonClicked(){
		SceneManager.LoadScene("Main");
	}
}
