using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;
using UnityEditor.Experimental.GraphView;

public class UnityToNode : MonoBehaviour
{
    public Button btnGetExample;
    public string host;
    public string port;
    public string idUrl;
        private void OnParticleUpdateJobScheduled()
    {
        
    }

    // Start is called before the first frame update
    public void Start()
    {
       this.btnGetExample.onClick.AddListener(() => 
        {
            var url = string.Format("{0}:{1}/{2}", host, port, idUrl);

            Debug.Log(url);
            StartCoroutine(this.GetData(url, (raw) =>{
                var res = JsonConvert.DeserializeObject<Protocols.Packets.common>(raw);
                Debug.LogFormat("{0}, {1}", res.cmd, res.message);
            }));
        });
    }

    public IEnumerator GetData(string url, System.Action<string> callback)
    {
        var webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        Debug.Log("Get : " + webRequest.downloadHandler.text);
        if(webRequest.result == UnityWebRequest.Result.ConnectionError 
            || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("너트워크 환경이 좋지 않아 통신 불가능");
        }
        else
        {
            callback(webRequest.downloadHandler.text);
        }
    }
}
