// 필요한 모듈 불러오기 
require('dotenv').config();             // dotenv 모듈을 사용하여 환경 변수 로드 
const express = require('express');
const bodyParser = require('body-parser');
const jwt = require('jsonwebtoken');
const bcrypt = require('bcrypt'); 

// Express 앱 생성 및 미들웨어 설정
const app = express();
app.use(bodyParser.json());

// 사용자 데이터 및 리프레시 토큰 저장소 (실제는 데이터베이스에서 진행)
const users = [];
const refreshTokens = {};

// 환경 변수에서 시크릿 키와 포트 가져오기 
const JWT_SECRET = process.env.JWT_SECRET;
const REFRESH_TOKEN_SECRET = process.env.REFRESH_TOKEN_SECRET;
const PORT = process.env.PORT || 3000;

// 회원가입 라우트
app.post('/register' , async(req,res) => {
    const {username, password} = req.body;

    if(users.find(user => user.username === username))
    {
        return res.status(400).json({error : '이미 존재하는 사용자입니다.'});
    }    

    const hashedPassword = await bcrypt.hash(password, 10);
    users.push({username , password: hashedPassword});              // hash 값으로 비밀번호 변형
    console.log(hashedPassword);
    res.status(201).json({message : '회원 가입 성공'});
})

// 로그인 라우트 
app.post('/login' , async(req,res) => {
    const {username, password} = req.body;
    const user = users.find(user => user.username === username);

    if(!user || !(await bcrypt.compare(password, user.password)))
    {
        return res.status(400).json({ error : '잘못된 사용자명 또는 비밀 번호 입니다.'});
    }

    const accessToken = generateAccessToken(username);
    console.log(accessToken);
    const refreshToken = jwt.sign({username}, REFRESH_TOKEN_SECRET);
    
    refreshTokens[refreshToken] = username;

    res.json({accessToken , refreshToken});
})

// 토큰 갱신 라우트 
app.post('/token' , async(req,res) => {
    const {refreshToken} = req.body;
    
    if(!refreshToken || !refreshTokens[refreshToken])
    {
        return res.sendStatus(401);
    }

    jwt.verify(refreshToken, REFRESH_TOKEN_SECRET, (err,user) => {
        if (err) return res.sendStatus(403);
        const accessToken = generateAccessToken(user.username);
        res.json({accessToken});
    })
})

// 로그 아웃 라우트
app.post('logout',(req,res) => {
    const {refreshToken} = req.body;
    delete refreshTokens[refreshToken];
    res.sendStatus(204);
});
// 보호된 라우트 
app.get('/protected', authenticateToken, (req, res) => {
    res.json({message : '보호된 데이터에 접근 성공' , user:req.user});
});


// 액세스 토큰 생성 함수
function generateAccessToken(username)
{
    return jwt.sign({username} , JWT_SECRET, {expiresIn: '15m'});
}

// 토큰 인증 미들웨어 
function authenticateToken(req,res,next) {
    const authHeader = req.headers['authorization'];
    const token = authHeader && authHeader.split(' ')[1];

    if(token == null) return res.sendStatus(401);

    jwt.verify(token, JWT_SECRET, (err, user) => {
      
        if(err) return res.sendStatus(403);
        req.user = user;
        next();        
    })
}

// 서버시작
app.listen(PORT, () => console.log(`서버가 포트 ${PORT} 에서 실행 중 입니다. `));