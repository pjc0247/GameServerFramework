GSF
====

아이디어
----
* __GSF__는 풀스택 프레임워크가 아닙니다.
    * 서버의 로직에 디펜던시가 없는 아주 바닥부분의 작업과,
    * 로직에 디펜던시가 생기는, 때때로 서버 개발자가 직접 짜는것이 더 명확한 부분에 선을 긋습니다.  
* 하지만 몇가지 범용적인 기능은 별도의 패키지로써 선택적으로 사용할 수 있습니다. 
    * 가져다 쓰는게 더 편할것 같으면 별도로 제공되는 패키지를 받아서 쓰세요. 바로 연동됩니다.

서비스 만들기
----
서비스는 게임의 기능적인 한 부분임과 동시에, 각각의 인스턴스가 하나의 세션을 담당합니다.<br>
<br>
아래는 `Attack` 패킷을 받으면 데미지를 계산해서 돌려주는 간단한 게임 서버의 예제입니다.<br>
예제의 `OnAttack` 메소드는 __GSF__에 의해 자동으로 실행되며, 수동으로 핸들러를 등록하지 않아도 됩니다.
```cs
using GSF;

class MyGameService : Service {
    // 세션마다 1개씩 생성되므로
    // 여기에 세션 데이터를 저장하도록 합니다.
    private PlayerInfo Player;
    private PlayerInfo Opponent;

    // 패킷 핸들러
    public OnAttack(AttackPacket packet) {
        var damage = Player.Atk - Opponent.Def;

        Opponent.Hp -= damage; 
        SendPacket(new DamagePacket() {
            Damage = damage
        });
    }
}
```

서버 실행시키기
----
```cs
Server.Create(9916)                 // 포트 번호를 지정합니다.
.WithService<EchoService>("/echo")  // 서비스의 구현체와, 경로를 지정합니다.
    .Run();
```


로그인의 처리
----
__GSF__에서의 로그인은 웹소켓 연결과 동시에 처리되며, 클라이언트는 서버에 연결 할 때 로그인 정보와 같이 명시하여 접속합니다. 
```
ws://127.0.0.1/my_game?user_type=facebook&user_id=FACEBOOK_ID&access_token=ACCESS_TOKEN&version=1.0.0
```
로그인 파라미터의 목록
* __version__
    * 클라이언트 버전입니다. 서버 버전과 불일치 할 경우 서버는 연결을 거절 할 수 있습니다.
* __user_type__
    * 로그인 할 IDP 종류를 명시합니다. (예: facebook, guest, kakao) 
* __user_id__
    * IDP에서 발급된 유니크 아이디입니다.
* access_token
    * IDP에서 발급받은 액세스 토큰입니다. `user_type`에 따라서 사용하지 않는 경우도 있습니다. 

만약 클라이언트가 지정한 로그인 정보가 부정확할 경우, 서버는 에러와 함께 연결을 거부할 수 있습니다.<br>
<br>
로그인이 완료되면 `Service` 객체가 생성되고, 서로간에 패킷을 주고받을 수 있는 상태가 됩니다.<br>
서버는 `Service` 객체 내부에서 `CurrentUserId` 프로퍼티에 접근하여 로그인된 유저의 아이디를 가져올 수 있습니다. 이는 클라이언트를 식별하는 고유 아이디로 사용됩니다. 
```cs
var userId = CurrentUserId;
``` 

지연된 로그인
----
만약 경우에 따라, 클라이언트가 일단 서버에 접속해야 하고 후에 다시 제대로 된 로그인을 수행해야 할 경우 지연된 로그인을 구현하도록 합니다.<br>
<br>
지연된 로그인을 사용하는 경우 로그인 절차는 아래와 같습니다.
* 클라이언트가 `guest` 타입으로 접속
    ```cs
    ws://127.0.0.1/my_game?user_type=guset&user_id=0&version=1.0.0
    ```

    * 이 단계에서 클라이언트의 `user_id`는 신뢰할 수 없는 값입니다. `user_id`와 관련된 작업은 지양해 주세요.
* 양측간의 적절한 처리 수행 후
* 제대로 로그인을 수행할 준비가 되면 지연된 로그인 수행

서버측에서 지연된 로그인을 수행하는 코드는 아래와 같습니다.<br>
로그인이 성공할 경우 세션에 바인딩 된 `CurrentUserId`가 변경될 수 있습니다.
```cs
await ProcessLogin("user_type", "user_id", "access_token");
```

로그인 서비스 구현하기
----
__GSF__는 기본적으로 Facebook 로그인을 지원합니다.<br>
만약 페이스북 이외에 카카오 등 추가적인 로그인 방법이 필요하다면 `IDProvider` 인터페이스를 구현하여 로그인 메소드를 추가할 수 있습니다.
```cs
[IDProviderCode("kakao")]
class KakaoTalk : IDProvider {
    public Task<bool> IsValidToken(string userId, string accessToken) {
        /* 카카오톡 서버에 userId, accessToken이 일치하는지 물어봅니다 */
    }
}
```

로컬 토큰
----
서버에서 각 IDP에 로그인 정보가 일치함을 물어보는것은 코스트가 존재하는 작업입니다.<br>
만약 빠른 재접속이거나, 매치서버에서 게임서버로 옮겨타는 등 사실상 IDP에 로그인을 다시 물어보지 않아도 되는 경우 __로컬 토큰__ 기능을 사용할 수 있습니다.<br>
<br>
로컬 토큰은 __이미 인증된 유저__에 대해서 __GSF__가 발급해주며, 이를 이용해 IDP 서버에 다녀오지 않고도 재인증을 수행할 수 있습니다.

* __user_type__은 `local` 로 지정합니다.
* __user_id__는 기존에 IDP로 로그인된 유저의 아이디를 그대로 사용합니다.
* __access_token__은 __GSF__가 발급한 값을 사용합니다.
```
ws://127.0.0.1/my_game?user_type=local&user_id=USER_ID&access_token=ACCESS_TOKEN&version=1.0.0
```

헬스체크
----
`ICheckable`을 구현한 메소드는 주기적으로 검사를 받으며, 검사에 실패한 경우 객체를 해제하는 로직을 작성할 수 있습니다.<br>
헬스체크 기능을 사용해 현재 세션이 유효한지, 혹은 어떠한 오브젝트가 올바른 상태를 가지고 있는지를 주기적으로 검사할 수 있습니다.<br>
<br>
아래 예제는 `Service`에 헬스체크를 구현하여, 오래동안 응답이 없는 세션을 자동으로 닫아주는 예제입니다.
```cs
class MyGameService : Service, ICheckable
{   
    public bool OnHealthCheck() {
        // 60초동안 메세지 없는 세션
        if (LastResponseTime >= TimeSpan.FromSeconds(60))
            return false;

        return true;
    }
    public void OnDispose() {
        // 세션을 닫는다
        ErrorClose(CloseStatusCode.Away, "healtcheck failure");
    }
}
```

```cs
// 체크 대상으로 등록
HealthChecker.Add(checkableObject);

// 체크 대상에서 제외
HealthChecker.Remove(checkableObject);
```

* `OnDispose`가 실행된 경우 `HealthChecker.Remove`는 자동적으로 호출됩니다. 


패킷 프로토콜
----
__GSF__는 기본적으로 웹소켓 위에서 동작하지만, 사용하는 프로토콜에는 제한이 없습니다.<br>
`IPacketProtocol`을 상속하여 패킷이 네트워크로 어떻게 전송되고, 네트워크로부터 받은 데이터를 어떻게 패킷으로 변환하는지를 지정합니다.<br>
<br>
아래는 __GSF__에 기본적으로 내장된 `GSF.Packet.Json.JsonProtocol`의 소스코드입니다.
```cs
public class JsonProtocol : IPacketProtocol
{
    public PacketBase Deserialize(string data)
    {
        return (PacketBase)JsonConvert.DeserializeObject(data);
    }

    public string Serialize<T>(T packet)
        where T : PacketBase
    {
        var setting = new JsonSerializerSettings()
        {
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
        };
        var json = JsonConvert.SerializeObject(packet);

        return json;
    }
}
```
```
웹소켓/바이너리 프로토콜은 TODO
https://github.com/pjc0247/Merona.cs/tree/master/doc/guide/custom_marshal
```

서버 환경
----
__GSF__로 구현된 서버는 여러가지 환경에서 구동될 수 있습니다.<br>
Visual Studio에서 바로 실행한 로컬 서버가 될수도 있고, ec2 인스턴스 위에 올려진 실제 서버, 혹은 ec2위에 올라갔지만 몇가지 환경설정이 약간 다른 베타 서버가 될수도 있습니다.<br>
* 로컬에서는 DB 없이 인메모리로 실행, __Indented__ JSON 프로토콜 사용
* 베타 환경에서는 ASDF DB에 연결
* 라이브 환경에서는 QWER DB에 연결

<br>
이를 위해 `GSF`는 서버 환경을 구분하여, 각각의 환경에 맞는 프로토콜/ DB/ 구현체등을 사용할 수 있는 기능을 제공합니다.
```
var env = ServerEnv.CurrentEnv;
``` 

매치메이킹
----
```
CastleInvader/Server에서 옮겨와야 함
```
만약 게임에 매치메이킹 서비스가 필요하다면, 미리 구현된 `GSF.MatchMaking` 서비스를 이용할 수 있습니다.<br>
<br>
__GSF__의 매치메이킹 시스템을 사용하기 위해서는 GSF의 매치메이커의 구조에 대해 알아야 합니다. 이는 크게 두가지 시스템으로 나누어져 있습니다.<br> 

* __매치 메이커__
    * __유저를 알맞은 상대와 매칭시키기 위한 분류기입니다. elo 레이팅, 혹은 Area에 기반한 매칭 로직은 여기에 작성합니다.__
    * 한개 이상의 __매치 큐__를 가집니다.
    * 유저가 매칭을 요청하면 알맞은 조건에 따라 유저를 적절한 큐에 담습니다.
    * 주기적으로 큐를 검사하여 매칭을 만들 수 있는지 검사합니다.
* __매치 큐__
    * __매칭을 기다리는 유저들이 대기하는 큐입니다. 큐 자체는 단순한 FILO 구조이며, 큐 자체의 자료구조 로직에 집중합니다. 로컬 큐인지, REDIS인지, MYSQL인지를 이곳에 구현합니다.__
    * 일반적인 큐 인터페이스를 제공합니다. 매치 메이커가 Enqueue를 요청하면 넣고, Peek을 요청하면 데이터를 가져올 수 있어야 합니다. 

아래는 유저의 레이팅에 따라 매치메이킹을 제공하는 __매치 매이커__의 간단한 예제입니다. 
```cs
class MyMatchMaker : IMatchMaker {
    private IMatchQueue Rating_0_500;
    private IMatchQueue Rating_501_1000;
    private IMatchQueue Rating_1001_1500;
    private IMatchQueue Rating_1501_2000;
    private IMatchQueue Nantoo;

    // queueTypeHint는 큐 타입을 지정하는 유저 데이터입니다.
    // 이 값을 이용하여, 어떤 큐에 넣을지를 필터링할 수 있습니다.
    public void Enqueue(Service player, int queueTypeHint) {
        if (queueTypeHint == MyQueueType.Nantoo)
            Nantoo.Enqueue(player);
        else {
            if (player.Rating <= 500)
                Rating_0_500.Enqueue(player);
            else if (player.Rating <= 1000)
                Rating_501_1000.Enqueue(player);
            else if (player.Rating <= 1500)
                Rating_1001_1500.Enqueue(player);
            else
                Rating_1501_2000.Enqueue(player);
        }
    }
}
```

매치메이킹 플로우
----

* __매치 서버__에서 __매치 토큰__ 발급
* 클라이언트는 토큰을 가지고 __게임 서버__에 연결
* __게임 서버__는 알맞은 매치 검색