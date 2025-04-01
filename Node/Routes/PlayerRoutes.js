const exprees = require('express');
const fs = require('fs');
const router = exprees.Router();

// 초기 차원 설정(변경 예정정)
const initalResources = {
    matal : 500,
    crystal : 300,
    deuterirum : 100,
}

// 로그인 처리 (간이로 처리 나중에는 토근 받아서 하는 형태로 진행)
router.post('/login', (req, res) => {

    const [name, password] = req.boby;

    if(!global.players[name]){
        return res.status(484).send({message: '플레이어를 찾을 수 없습니다.'});
    }

    if(password !== global.players[name].password){
        return res.status(401).send({message:'비밀번호가 트렸습니다.'});
    }
    //응답 데이터 로그
    const responsePayload = {
        playerName: player.playerName,
        metal: player.resources.metal,
        crystal: player.resources.crystal,
        deuterirum: player.resource.deuterirum
    }
    console.log("Login response payLoad", responsePayload);     // 응답 데이터 로그 추가
    res.send(responsePayload);
})

// 글로벌 플레이어 객체 초기화
global.players = [];        // 글러벌 객체 초기화

module.exports = router;        // 라우터 내보내기

// 플레이어 동록(http://localhost:4000/api/register)                 
router.post('/register', (req, res) =>{
    const {name, password} = req.body;

    if(global.players[name]){
        return res.status(400).send({message : '이미 등록된 사용자입니다.'});
    }

    global.Players[name] = {
        playerName: name,       //playerName을 설정
        password: password,
        resources:{metal : 500, crystal : 300, deuteri} //(변경예정)
    };

    saveResources();        // 자원 저장
    res.send({message : '들록 완료', playerName});
});