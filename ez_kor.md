GSF.Ez
====

__GSF.Ez__는 빌드가 필요 없는 게임 서버 환경을 제공합니다.


config.json
----
게임 서버의 초기화가 필요한 경우, exe와 같은 폴더 안에 `config.json` 파일을 생성하여 서버 환경 설정을 수행할 수 있습니다.

```json
{
    "WorldProperty" : {
        "test" : 1234,
        "name" : "rini"
    },
    "OptionalWorldProperty" : {
        "rini" : "genius"
    },

    "WorldPropertyDataSource" : "http://www.naver.com",
    "OptionalWorldPropertyDataSource" : "http://www.naver.com"
}
```

* __WorldProperty, OptionalWorldProperty__ : `Dictionary`형태로 정적인 데이터를 설정합니다.
* __WorldPropertyDataSource, OptionalWorldPropertyDataSource__ : `URL`로써 동적인 데이터를 설정할 때 사용합니다.

```
DataSource가 나중에 실행됩니다. 겹치는 KEY에 대해서는 치환이 수행됩니다.
```
