using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour {
	Vector3 diff;

	// 추적대상 파라미터
	public GameObject target;  // 카메라가 추적하는 대상의 오브젝트를 Inspector뷰에서 지정한다.
	public float followSpeed;

	// Use this for initialization
	void Start () {
		// 추적거리 계산.
		// 대상과 어느 정도의 거리를 유지하면서 추적할지를 시작 시의 위치에 따라 계산해둔다.
		diff = target.transform.position - transform.position;	
	}
	
	// Update is called once per frame

	// LateUpdate함수는 일반적인 Update 함수와 마찬가지로 매 프레임에 호출되고 있지만. 그순서가 Update함수의 처리가 모두 끝난 후로 정해져 있다.
	void LateUpdate () {
		// 선형 보간 함수에 의한 유연한 움직임.
		transform.position = Vector3.Lerp(transform.position, target.transform.position - diff, Time.deltaTime * followSpeed);
	}
}
