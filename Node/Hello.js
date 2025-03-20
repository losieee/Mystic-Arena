// HTTP 모듈 로딩
let http = require("http");

// HTTP 서버를 Listen 상태로 8000포트 사용하여 만든다.
http.createServer(function (request, response){
    
    // response Http 타입 해더로 정의
    response.writeHead(200, {'Content-type' : "text/plain"});

    response.end('Hello World');
}).listen(8000);            // 8000번 포트를 사용한다.

console.log("Server running at http://127.0.0.1:8000")
