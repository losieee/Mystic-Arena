require('dotenv').config();             //dotenv 모듈을 사용하여 환경 변수 로드 
const express = require('express');
const bodyParser = require('body-parser');
const jwt = require('jsonwebtoken');
const bcrypt = require('bcrypt'); 

//Express 앱 생성 및 미들웨어 설정
const app = express();
app.use(bodyParser.json());

//사용자 데이터 및 리프레시 토큰 저장소 (실제는 데이터베이스에서 진행)
const users = [];
const refreshTokens = {};

//환경 변수에서 시크릿 키와 포트 가져오기 
const JWT_SECRET = process.env.JWT_SECRET;
const REFRESH_TOKEN_SECRET = process.env.REFRESH_TOKEN_SECRET;
const PORT = process.env.PORT || 3000;

console.log(JWT_SECRET);
console.log(REFRESH_TOKEN_SECRET);
console.log(PORT);

// 회원가입 라우트
app.post('/register', async (req, res) => {
    const { username, password } = req.body;

    if(users.find(user => user.username === username)) {
        return res.status(400).json({ message: '이미 존재하는 사용자입니다.' });
    }

    const hashedPassword = await bcrypt.hash(password, 10);
    users.push({ username, password: hashedPassword });             // hash 값으로 비밀버호 변경
    console.log(hashedPassword);
    res.status(201).json({ message: '회원가입 성공' });
});

// 엑세스 토근 생성 함수
function generateAccessToken(username) {
    return jwt.sign({ username }, JWT_SECRET, { expiresIn: '15s' });

    if(token === null) return res.sendStatus(401);

    jwt.verify(token, JWT_SECRET, (err, user) => {
        if(err) return res.sendStatus(403);
        req.user = user;
        next();
    });
}

// 서버시작
app.listen(PORT, () => console.log(`서버가 포트 ${PORT} 에서 실행중입니다.`));




