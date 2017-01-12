GSF.Ez
====

セットアップ・ビルドが必要ないゲームサーバー。

Requirements (client-side)
----
* GSF.Ez.Packet.dll
* GSF.Packet.dll
* Newtonsoft.Json
* WebSocketClient

Connect
----
```cs
var ws = new WebSocket("ws://localhost:9916/echo?version=1.0.0&userType=guest&userId=1");
```

* `version`は必ず`1.0.0`を記入して下さい。 


Join/Leave
----
```cs
SendPacket(new JoinPlayer() {
    Player = new Player() {
        UserId = 1234,
        Property = new Dictionary<string, object>() {
            {"nickname", "min-soo"},
            {"type", "cookie-maker"}
        }
    }
});
```
最初、接続が成功したら`JoinPlayer`パケットを伝送して下さい。<br>

* `UserId`はClientで生成したランダムなintegerです。
* `Property`はプレイヤーのニックネームとかレベルのダーターを含めます。

```cs
SendPacket(new LeavePlayer() {
});
```
或は、Clientから接続を切れてもサーバーは`LeavePlayer`を送信します。
