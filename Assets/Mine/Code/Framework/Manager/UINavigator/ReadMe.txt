본 문서는 UINavigator의 사용 방법을 설명하는 것을 목적으로 한다.


1. 기본적인 구조


UIView와 해당 UIView들을 배치할 UIContainer로 구성되어 있다.
UIView와 UIContainer는 각각 Sheet, Page, Modal 그리고 SheetContainer, PageContainer, ModalContainer로 구성되어 있다.
각 UIView는 적합한 UIContainer의 자식 오브젝트로써 배치되어야 한다.
또한, 각 UIView에 해당하는 게임 오브젝트들은 용도에 따른 적합한 UIView를 상속한 개체를 컴포넌트로 추가해야한다.


    1-1. Sheet의 특징

    1) History를 관리되지 않음
    2) 하위 UIView들의 History 관리
    3) Container 기준으로 한번에 하나만 활성화 됨
    4) 미리 씬에 배치해둠
    

    1-2. Page의 특징

    1) History가 관리됨
    2) Container 기준으로 한번에 하나만 활성화 됨
    3) 미리 씬에 배치해둠
    

    1-3. Modal의 특징

    1) History가 관리됨
    2) Container 기준으로 한번에 중첩되서 활성화할 수 있음
    3) 미리 씬에 배치해두지 않고 ModalContainer에 인스펙터 상에 프리팹을 등록해두고 사용



2. 주요 API


> 지정한 Transform의 가장 인접한 Container 찾기

UIContainer<T>.cs
    Of(Transform transform, bool isCached)


> 이름으로 Container 찾기

UIContainer<T>.cs
    Find(string containerName)


> 지정한 Sheet로 전환

SheetContainer.cs
    public async UniTask NextAsync<T>() where T : Sheet


> 지정한 Page로 전환

PageContainer.cs
    public async UniTask<T> NextAsync<T>() where T : Page
    
> 이전 Page로 전환

PageContainer.cs
    public async UniTask PrevAsync<T>() where T : Page


> 지정한 Modal 활성화

ModalContainer.cs
    public async UniTask<T> NextAsync<T>() where T : Modal
    
> 현재 Modal 종료

ModalContainer.cs
    public async UniTask PrevAsync<T>() where T : Modal


> 최상단 UIView 종료 및 이전 UIView로 전환

UIContainer.cs
    public static async UniTask BackAsync()