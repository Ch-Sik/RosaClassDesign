# 용어

* 인 게임의 플레이어캐릭터(PC)는 'Player', 현실 세계의 사용자는 'User'로 칭함.

* 'Spawn'은 'Instantiate'를 한 단계 추상화한 개념으로 사용함 (부대 요소들의 Instantiate, 초기화, 만약 오브젝트 풀링을 사용한다면 그것들까지 등등을 모두 포함한 개념)

* 식물마법은 일괄적으로 'Magic'으로 축약함.
  * 더 좋은 단어가 있다면 추천받음.

* 'Enemy'는 상대적인 개념으로, 'Monster'는 절대적인 개념으로 사용함. (Enemy의 Enemy는 아군)

* 'Act'은 다음 일련의 과정을 전부 통칭하는 단어

  * 입력→선딜레이(Startup / Cast)→효과발휘(Execute / Attack / 기타 등등)→후딜레이(Recoil)→종료

  * Action은 C# 문법 기능 중 하나라 혼동을 방지하기 위해 Act로 대체



# 메모

일반

* 싱글톤 패턴을 사용하는 경우, Awake에서 instance 필드를 초기화함.
  * 이후 Start에서 해당 instance를 참조하기 위해 awake에서 미리 초기화



Player 모듈 관련

* Player의 move와 attack은 일단 하나로 합쳐놓음. 서로 영향을 많이 받는데 시작부터 분리하는 것보단 일단 하나로 뭉치고 나중에 너무 비대해지면 그 때 가르는 게 좋을 것 같다고 판단함.

* PlayerMove는 InputManager.OnMove() -> PlayerMove.inputVector -> PlayerMove.Update()로 전달되는 구조. 특정 상황에서 velocity가 (0,0)이 되어야 하는 부분은 Update 내부에서 플래그를 사용하여 판단 및 업데이트하고 inputVector 필드는 건들지 않도록 하기. 



입력 관련

* 플레이어 입력과 UI 입력이 동시에 수행되지 않도록 하는 것은 InputActionMap을 활성화/비활성화하는 것으로 구현함.

* 플레이어 입력을 담당하는 InputAction 들은 PlayerInput 컴포넌트(내장)를 통해 게임에 적용됨.
  * PlayerInput 컴포넌트는 Player 오브젝트의 최상단에 위치, Broadcast Messages 방식을 사용 -> Player나 그 자식오브젝트들 안에서 OnMove, OnAttack 등의 함수를 구현하기만 하면 됨.
    * Broadcast Message는 MonoBehaviour 기능 중 하나로, Hierarchy 상의 자식 오브젝트들에게 (자신 포함) 메시지를 보내는 것. Send Message는 자식 오브젝트 말고 자기 자신만 해당.

* UI 입력을 담당하는 InputAction 들은 EventSystem 오브젝트 상의 InputSystemUIInputModule 컴포넌트(내장)를 통해 게임에 적용됨.



