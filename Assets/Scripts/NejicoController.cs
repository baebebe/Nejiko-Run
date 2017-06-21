using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NejicoController : MonoBehaviour {
	const int MinLane = -2;
	const int MaxLane = 2;
	const float LaneWidth = 1.0f;
	const int DefaultLife = 3;
	const float StunDuration = 0.5f;

	CharacterController controller;
	Animator animator;

	Vector3 moveDirection = Vector3.zero;
	int targetLane;
	int life = DefaultLife;
	float recoverTime = 0.0f;

	public float gravity;
	public float speedZ;
	public float speedX; //수평 방향 속도의 파라미터
	public float speedJump; 
 	public float accelerationZ; // 전진 가속도의 파라미터
 
	public int Life(){
		return life;
	}

	public bool IsStan(){
		return recoverTime > 0.0f || life <= 0;
	}

	// Use this for initialization
	void Start () {
		// 필요한 컴포넌트를 자동 취득
		controller = GetComponent<CharacterController>(); //GetComponent<>() 각 오브젝트에 설정 되어있는 컴포넌트를 취득한다.
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		// 디버그용
		if(Input.GetKeyDown("left")) MoveToLeft();
		if(Input.GetKeyDown("right")) MoveToRight();
		if(Input.GetKeyDown("space")) Jump();

		if(IsStan()){
			// 움직임을 기절 상태에서 복귀 카운트를 진행한다
			moveDirection.x = 0.0f;
			moveDirection.z = 0.0f;
			recoverTime -= Time.deltaTime;
		}else{
			// 서서히 가속하여 Z방향으로 계속 전진시킨다.
			float acceleratedZ = moveDirection.z + (accelerationZ * Time.deltaTime);
			moveDirection.z = Mathf.Clamp(acceleratedZ, 0, speedZ);

			// X방향은 목표의 포지션까지의 차등 비율로 속도를 계산
			float ratioX = (targetLane * LaneWidth - transform.position.x) / LaneWidth;
			moveDirection.x = ratioX * speedX;
		}

		// if(controller.isGrounded)	{ //땅에 접지를 하고 있는지를 판단한다. CharacterController의 isGrounded에 의해 캐릭터가 지면에 있는지를 판정하여 지면에 있을 때만 플레이어의 키입력을 받도록 한다.
		// 	// Input을 감지하여 앞으로 전진한다.
		// 	// Input.GetAxis함수에 의해 수직 방향의 입력을 취득하여 전진하는 속도를 설정한다. 이때 0 이상의 값을 이용함으로써 캐릭터가 뒤로 가지 않도록 하고 있다.
		// 	if(Input.GetAxis("Vertical") > 0.0f){ // Vertical : 방향키 Up, Down
		// 		// 0.0f 이상일 때 다음 구문 실행. GetAxis리턴값은 -1 ~ 1
		// 		moveDirection.z = Input.GetAxis("Vertical") * speedZ;
		// 	}else{
		// 		moveDirection.z = 0;
		// 	}

		// 	// 방향전환
		// 	// Input.GetAxis함수에 의해 가로 방향의 입력을 취득하여 Y축을 중심을 캐릭터를 회전시킨다.
		// 	transform.Rotate(0, Input.GetAxis("Horizontal") * 3, 0); // Horizontal : 방향키  Left. Right

		// 	// 점프
		// 	// Input으로 Jump에 할당된 키 입력이 있으면 위쪽 방향으로 jumpSpeed를 적용한다. 또한, 동시에 Animator에 대해 jump 트리거를 건네서 애니메이션을 전환한다.
		// 	if(Input.GetButton("Jump")){ // Jump : 키보드 Space Bar
		// 		moveDirection.y = speedJump;
		// 		animator.SetTrigger("jump");
		// 	}
		// }

		// 중력만큼의 힘을 매 프레임에 추가
		// 중력의 가산.
		// 매 프레임마다 중력만큼이 속도를 아래쪽으로 가한다. Time.deltaTime으로 이전 프레임에서의 경과 시간을 얻을 수 있다. 이 값을 중력의 파라미터에 곱함으로써 서로 다른 프레임 속도라도 일정 비율의 값을 계속 추가하도록 하고 있다.
		moveDirection.y -= gravity * Time.deltaTime;

		// 이동 실행, 캐릭터 이동.
		// Transform의 Transform.Direction 함수에 의해 지금까지 계산한 로컬의 이동 벡터를 씬의 글로벌한 값으로 변환한다. 그것을 CharacterController이 Move함수에 전달하여 캐릭터의 이동을 실행한다.
		// 또한, 여기서도 Time.deltatime을 이용하여 프레임 속도의 차이에 의한 영향을 없애고 있다.
		Vector3 globalDirection = transform.TransformDirection(moveDirection);
		controller.Move(globalDirection * Time.deltaTime);

		//이동 후 접지하고 있으면 Y방향의 속도는 리셋한다.
		if(controller.isGrounded) moveDirection.y = 0;

		// 속도가 0 이상이면 달리고 있는 플래그를 true로 한다.
		animator.SetBool("run", moveDirection.z > 0.0f);
		// Idle및 Run 애니메이션 제어
		// Animator 현재 캐릭터의 이동 상태 파라미터를 전달한다. moveDirection의 Z 성분이 0보다 커지면 달리고 있다고 판단하여 run파라미터로 전환한다.

	}

	//왼쪽 차선으로 이동을 시작
	public void MoveToLeft(){
		if(IsStan()) return;
		if (controller.isGrounded && targetLane > MinLane) targetLane--;
	}

	// 오른쪽 차선으로 이동을 시작
	public void MoveToRight(){
		if(IsStan()) return;
		if (controller.isGrounded && targetLane < MaxLane) targetLane++;
	}

	public void Jump(){
		if (IsStan()) return;
		if(controller.isGrounded){
			moveDirection.y = speedJump;   
			// 점프 트리거를 설정
			animator.SetTrigger("jump");
		}
	}

	// CharacterController 충돌이 발생했을 때의 처리
	void OnControllerColliderHit(ControllerColliderHit hit){
		if(IsStan()) return;

		if(hit.gameObject.tag == "Robo"){
			//라이프를 줄이고 기절 생태로 전환
			life--;
			recoverTime = StunDuration;

			//데미지 트리거를 설정
			animator.SetTrigger("damage");

			//히트한 오브젝트는 삭제
			Destroy(hit.gameObject);
		}
	}

}
