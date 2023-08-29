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
  * 특히 '이동 공격'이 가능해야할지 아직 안정했으므로




입력 관련

* 플레이어 입력과 UI 입력이 동시에 수행되지 않도록 하는 것은 InputActionMap을 활성화/비활성화하는 것으로 구현함.
* 플레이어 입력을 담당하는 InputAction 들은 Actions Asset 참조 방식을 사용함
  * [공식 문서](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.6/manual/Workflows.html)에서 제시하는 workflow들 중 3번째 방식에 해당
  
* Dialogue 입력은 UI와 분리함.
  * 대화 이벤트 진행 중에도 일시정지할 수 있다는 가능성 때문에 분리
    * 일시정지 메뉴 UI 조작하려다가 그와 동시에 '아무키를 눌러 다음 대화로 진행' 해버릴까봐

* UI 입력을 담당하는 InputAction 들은 EventSystem 오브젝트 상의 InputSystemUIInputModule 컴포넌트(내장)를 통해 게임에 적용됨.
  * 그냥 기본 설정 그대로 둔 것에 불과함.
  * 혹시나 키보드를 통해 UI 조작을 가능하게 하려는 경우 상호작용 가능한 UI 관련 컴포넌트들 (Button, Slider 등)의 Navigation 항목만 잘 설정해주면 됨.




