const express = require('express');
const fs = require('fs');   // fs 모듈 추가
const router = express.Router();

// 초기 자원 설정
const initalResources = {
    user_count : 0,
}

// 글로벌 플레이어 객체 초기화 
global.players = {};            // 글로벌 객체 초기화 

// 로그인 처리 (간이로 처리 나중에는 토큰 받아서 하는 형태로 진행)
router.post('/login', (req, res) => {
    const { name, password } = req.body;

    if (!global.players[name]) {
        return res.status(404).send({ message: '플레이어를 찾을 수 없습니다.' });
    }

    if (password !== global.players[name].password) {
        return res.status(401).send({ message: '비밀번호가 틀렸습니다.' });
    }

    const player = global.players[name];

    // 응답 데이터 로그
    const responsePayload = {
        playerName: player.playerName,
        metal: player.resources.player_count
    };

    console.log("Login response payload:", responsePayload); // 응답 데이터 로그 추가
    res.send(responsePayload);
});


// 플레이어 등록 (http://localhost:4000/api/register)
router.post('/register' , (req, res) => {
    const {name , password} = req.body;

    console.log(name,password);
    if(global.players[name])
    {
        return res.status(400).send({ message : '이미 등록된 사용자입니다.'});
    }

    global.players[name] = {
        playerName: name,   // playerName을 설정
        password: password,
        resources:{
            player_count : 0,
        },
        planets:[]
    };

    saveResources();            // 자원 저장
    res.send({message : '등록 완료' , Player:name});
});

// 플레이어 삭제
router.delete('/player/:name', (req, res) => {
    const { name } = req.params;
    const { password } = req.body;

    if (!global.players[name]) {
        return res.status(404).send({ message: '플레이어를 찾을 수 없습니다.' });
    }

    if (password !== global.players[name].password) {
        return res.status(401).send({ message: '비밀번호가 틀렸습니다.' });
    }

    delete global.players[name];
    saveResources();
    res.send({ message: '플레이어가 성공적으로 삭제되었습니다.' });
});

// 자원 수집 메서드 
router.post('/collect/:playerName' , (req, res) => {
    const player = global.players[req.params.playerName];

    if(!player)
    {
        return res.status(404).send({message:'플레이어를 찾을 수 없습니다. '});
    }

    console.log(player.resources);

    saveResources();            // 자원 저장
    res.send(player.resources);
});

// 자원 저장 함수
function saveResources() {
    fs.writeFileSync('resources.json', JSON.stringify(global.players, null, 2)); // JSON 파일로 저장
}

module.exports = router;            // 라우터 내보내기 