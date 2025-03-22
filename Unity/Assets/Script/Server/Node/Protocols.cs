using System.Collections;
using System.Collections.Generic;
// using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Protocols 
{
    public class Packets
    {
        public class common
        {
            public int cmd;             // 명령 숫자 표시
            public string message;      // 메세지
        }

        public class req_data : common
        {
            public int id;              // id 를 받아서 한다.
            public string data;         // 전달 데이터
        }
    }
}
