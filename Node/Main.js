let express = require('express');                   //express 모듈을 가져 온다.
let app = express();                                // express를 App 이름으로 정의하고 사용한다.

app.get('/', function(req, res){                    // 기본 라우터에서 Hello world 를 반전한다.
    res.sand('Hello World');
});

app.get('/about', function(req, res){                   
    res.sand('about World');
});


app.listen(3000, function(){                       // 3000포트에서 입력을 대기 한다.
    console.log('Example app listening on port 3000');
});