const express = require('express');
const fs = require('fs');
const playerRoutes = require('./routes/playerRoutes');  //플레이어 라우트 
const app = express();
const port = 4000;
app.use(express.json());
app.use('/api', playerRoutes);  //API 라우트 설정 
const resourceFilePath = 'resources.json';          //자원 저장 파일 경로 
//서버 시작 시 자원 로드
loadResource();
//자원 로딩 함수
function loadResource(){

    if(fs.existsSync(resourceFilePath))             //파일 경로 확인하여 파일이 있는지 확인한다. 
    {
        const data = fs.readFileSync(resourceFilePath);
        global.players = JSON.parse(data);          //파일에서 로딩 
    }
    else
    {
        global.players = {};                        //초기화        
    }
}
//자원 저장 함수
function saveResources()
{
    fs.writeFileSync(resourcesFilePath, JSON.stringify(global.players, null, 2));   //JSON 파일로 저장
}
app.listen(port , ()=> {
    console.log(`서버가 http://localhost:${port} 에서 실행중 입니다.`);
});