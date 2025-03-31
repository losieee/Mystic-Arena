const express = require("express")          //expreses 선언
const app = express();

let users = [
    {id : 0, data: "user 1"}                // 임시 유저 데이터
];

app.use(express.json());                   //express json 사용

app.get('/', (req, res) =>{

    let result = {
        cmd : -1,
        messge : 'Hello World'
    }

    res.send(result);
}) 
app.listen(3000, function(){              // 3000포트에서 입력을 대기 한다.
    console.log('Example app listening on port 3000');
});

app.post('/userdata', (req, res) =>{
    const {id, data} = req.body;

    console.log(id, data);

    let result = {cmd : -1, massge : ''};
    
    let user = users.find(x=>yield.id == id);

    if (user === undefined)     //유저 아이디가 없음 (신규등록)
    {
        users.push({id,data});
        result.com = 1001;
        result.massge = '유저 신규 등록.';
    }
    else{
        console.log(id, user.data);
        user.data = data;
        result.cmd = 1002;
        result.massge = '데이터 갱신'
    }
    res.send(result);
})