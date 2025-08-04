# 🏹신궁

<img alt="Static Badge" src="https://img.shields.io/badge/%ED%94%84%EB%A1%9C%EC%A0%9D%ED%8A%B8%20%EA%B8%B0%EA%B0%84%3A-2025.05~2025.06-FAB040?style=flat-square&logoColor=white">

|멀티 매칭 화면|플레이 화면|
|:---:|:---:|
|<img src="https://raw.githubusercontent.com/j1sung/God_of_Archer/main/gifs/multi.gif" width="400" alt="멀티 플레이 GIF"/>|<img src="https://raw.githubusercontent.com/j1sung/God_of_Archer/main/gifs/play.gif" width="400" alt="게임 플레이 GIF"/>|

> 조선의 국궁을 테마로 한 멀티 활 데스매치 FPS 게임.
<br>플레이어는 현실적인 물리가 적용된 활을 사용하여 적을 물리친다.
>
<br>

## 💁‍♂️ 프로젝트 팀원
| Server | Designer | Client | Client |
|:---:|:---:|:---:|:---:|
| <img src="https://github.com/Lim-Dolphin.png?size=120" width="100"/> | <img src="https://github.com/Developer-EJ.png?size=120" width="100"/> | <img src="https://github.com/sunsi-game.png?size=120" width="100"/> | <img src="https://github.com/j1sung.png?size=120" width="100"/> |
| [임백규](https://github.com/Lim-Dolphin) | [김은재](https://github.com/Developer-EJ) | [김호영](https://github.com/sunsi-game) | [안지성](https://github.com/j1sung) |
<br>

## 📝 주요기능
### 1. 멀티 접속 시스템
 - 2 vs 2 Host-Client 구조 표현
 - Host가 방을 생성하면 Client들이 접속하는 방식으로 동작
 - 기본적인 위치 / 상태 동기화 등 멀티플레이의 핵심 기능 포함
### 2. 화살 시스템
 - 화살 발사
 - 화살 수거
 - 중앙 관리
<br>

## 💻 Tech Stack
### | Work Stack
<div align="left">
 &nbsp;&nbsp;&nbsp;<img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/blender/blender-original.svg" width="40" height="40"/>
 &nbsp;<img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/unity/unity-original.svg" width="40" height="40"/>
 <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg" width="40" height="40"/>
 <img src="https://media.licdn.com/dms/image/v2/C4D0BAQFgm5g8rrdzPg/company-logo_200_200/company-logo_200_200/0/1630460711618/exit_games_logo?e=2147483647&v=beta&t=U1RPD7XVp9E-ex118pvgff__5uPKLsEnJCcqMJ4PMeU" width="40" height="40"/>
</div>
<div align="left">
  <img alt="Static Badge" src="https://img.shields.io/badge/Blender-E87D0D?style=flat-square&logo=Blender&logoColor=white">
  <img alt="Static Badge" src="https://img.shields.io/badge/Unity-black?style=flat&logo=Unity&logoColor=white">
  <img alt="Static Badge" src="https://img.shields.io/badge/Photon-004480?style=flat-square&logo=Photon&logoColor=white">
</div>

### | Version Control
<div align="left">
  <!-- GitHub -->
  &nbsp;&nbsp;&nbsp;<img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/github/github-original.svg" width="40" height="40"/>
  <!-- Git -->
  &nbsp;&nbsp;<img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/git/git-original.svg" width="40" height="40"/>
</div>
<div align="left">
<img alt="Static Badge" src="https://img.shields.io/badge/Github-181717?style=flat-square&logo=github&logoColor=white">
 <img alt="Static Badge" src="https://img.shields.io/badge/Git-F05032?style=flat-square&logo=git&logoColor=white">
</div>

### | Tools
<div align="left">
  <!-- Notion (공식 SVG) -->
  &nbsp;&nbsp;<img src="https://upload.wikimedia.org/wikipedia/commons/4/45/Notion_app_logo.png" width="40" height="40"/>
  <!-- Jira -->
  &nbsp;&nbsp;&nbsp;<img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/jira/jira-original.svg" width="40" height="40"/>
  <!-- Confluence -->
  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/confluence/confluence-original.svg" width="40" height="40"/>
</div>
<div align="left">
<img alt="Static Badge" src="https://img.shields.io/badge/Notion-000000?style=flat-square&logo=notion&logoColor=white">
 <img alt="Static Badge" src="https://img.shields.io/badge/Jira-0052CC?style=flat-square&logo=jira&logoColor=white">
 <img alt="Static Badge" src="https://img.shields.io/badge/Confluence-172B4D?style=flat-square&logo=confluence&logoColor=white">
</div>
<br>

## 🗂 Directory
```
📂 Assets
 ├─📂 Photon
 ├─📂 Scripts
 ├─📂 Prefabs
 ├─📂 Materials
 └─📂 Scenes  
    ├─📂 LBK_Network
    |     - Network_lab.scene (메인 Scene)
    └─📂 LBK_Assets
       ├─ 📂 Animation
       ├─ 📂 AudioSource
       ├─ 📂 Prefabs (게임에 필요한 Prefab)
       |   - NetworkRunner.prefab (게임에 사용되는 네트워크 러너)
       |   - Player_Network_Fin_chung.prefab (청나라 게임 플레이어 프리팹)
       |   - Player_Network_Fin_Josen.prefab (조건 게임 플레이어 프리팹)
       ├─ 📂 Scenen
       |   - Conquer Test.scene (미완 Scene)
       |   - Death Match Test.scene (데스메치 Scene)
       |   - Practice Test.scene (미완 Scene)
       ├─ 📂 Script
       |   ├─📂 Beacon (봉화 조작 스크립트, 구현 실패)
       |   ├─📂 GameScript (Game Mechanism 관련 스크립트)
       |   |  - Gameplay.cs (게임 Manager)
       |   ├─📂 Menu (Fusion Menu 관련 스크립트) → 📂_JS > Scenes > Editor & Runtime 내부 존재
       |   ├─📂 Player (Player 관련 스크립트)    
       |   ├─📂 UI (UI 관련 스크립트)      
       |   └─📂 Weapons (무기 관련 스크립트)
       ├─ 📂 Settings
       └─ 📂 Sprite
```

## 🚩 Build & Run
#### 1. 빌드 세팅
- `Assets → Scenes → LBK_Assets → Scene → DeathMatch → Gameplay → Start_player_cnt` 값 설정(반드시 2의 배수로 설정, 최소 2, 최대 10)
  
#### 2. 빌드
- 경로 `Assets > Scenes > LBK_Assets, LBK_Network`

```
[Build Setting] → 빌드 씬 4가지 포함 Build!
- Network_lab.scene
- ConquerTest.scene
- DeathMatchTest.scene
- PracticeTest.scene
```
#### 2. 빌드된 파일 실행  `God_Of_Archer.exe`
<br>

## 🎮 Play & Control

### | Play Method
1. 유니티 에디터로 게임을 시작 → 마스터 클라이언트 서버 오픈
2. 나머지 플레이어가 빌드된 실행 파일로 게임을 접속
3. (현재)총 4인이 모이면 게임이 시작 → 2:2 매칭 데스매치 시작!

### | Control
|조작|버튼|
|:---:|:---:|
|움직임|W,A,S,D|
|달리기|Shift|
|활 줍기|E|
|활 쏘기|마우스 좌클릭|
|활 초기화|R|
- 시연 영상 링크 : [신궁 플레이](https://drive.google.com/file/d/1sfewZwva9eQGLppeQQJ2SbsLQTAoakjw/view?usp=sharing)
- Notion : [노션 작업](https://acute-library-43c.notion.site/19de57fa71f28068ad51fa3b40ad7889?source=copy_link)
- Jira : [Jira 백로그](https://roadofmartialts.atlassian.net/jira/software/projects/SCRUM/boards/1)
- Confluence : [Confluence](https://roadofmartialts.atlassian.net/wiki/spaces/ergVlj2bCyn3/overview)
# God_of_Archer
