GSF
====

GSFについて
----

サービス
----
サービスはゲームサーバーの機能的な一部分とおもに、一つ一つのインスタンスが一つのセッションを担当します。<br>
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

サーバーを実行する
----

ログインの処理
----
__GSF__のログインはWebSocketのコネクトと同時に処理されます。　クライアントはサーバーへコネクトする時必ずログイン情報を一緒に渡してください。

Lazy ログイン
----

HealthCheck
----

