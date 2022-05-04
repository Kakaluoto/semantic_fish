using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Threading;

// ������Ϣ�ĸ�ʽ����Ϊ�ĸ��ֶΣ����͡�ID��λ�á���С
public class FishInfo
{
    public string type;
    public int idx;
    public float[] pos;
    public float scale;
}

// ������ÿ�������Ϣ��������ģ�Ͷ���ID��flag��Ŀ��㣬Ŀ���С
public class FishIns
{
    public GameObject obj;
    public int idx;
    public bool flag; // �����жϸ�ʵ���Ƿ�õ����£���û�и�������Ҫɾ��������δ��⵽�����Ƴ�����
    public Vector3 target;
    public float scale;
}



public class ClientV2 : MonoBehaviour
{

    static Socket socket;
    int recv_len = 0;
    byte[] readbuff = new byte[1024];

    // ��Ž��յ���Ϣ�ĵط�
    public static Queue<List<FishInfo>> recv_que = new Queue<List<FishInfo>>();

    // ��ų���������ʵ����Ϣ�ĵط���fishins
    public static List<FishIns> fishins = new List<FishIns>();

    // ��Ϸ��һ֡ʱ����
    void Start()
    {
        // ����Server
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect("127.0.0.1", 8888);

        // ���������̣߳����Ͻ�����Ϣ
        Thread recv_t = new Thread(new ThreadStart(recv));
        recv_t.Start();
    }

    // ��Ϸÿһ֡����(���߳�)
    void Update()
    {
        // ���recv_que�������ݣ��������߳��յ�����Ϣ�����룩
        if (recv_que.Count > 0)
        {
            // ����ȡ��������һ��List of FishInfo�����˴μ�⵽��ͼ�����������Ϣ��������Ϊfishinfo
            List<FishInfo> fishinfo = recv_que.Dequeue();

            // ����fishinfo��ÿ��
            foreach (FishInfo i in fishinfo)
            {
                // �����fishinfo����ڶ�Ӧ��fishins�����⵽������������������ݸ�fishinfo���½�һ��FishInsʵ�������������fishins
                if (!fishins.Exists(t => t.idx == i.idx))
                {
                    FishIns j = new FishIns();
                    create(j, i);
                    fishins.Add(j);  
                }

                // �����fishinfo����ڶ�Ӧ��fishins��������Ϣ���ж�Ӧ����Ϸģ�ͣ�������ݸ�fishinfo����¸�fishins��
                else
                {
                    FishIns j = fishins.Find(t => t.idx == i.idx);
                    set(j, i);
                }
            }

            // �����ϸ��½������ҵ�fishins������δ���µ�����˴μ��û�и������Ϣ�ˣ�������ɾ�������gameobj�Լ���Ӧ��FishInsʵ��
            List<FishIns> fishout = fishins.FindAll(t => t.flag == false);
            foreach (FishIns i in fishout)
            {
                Destroy(i.obj); // ɾ��gameobj
            }
            fishins.RemoveAll(t => t.flag == false); // ɾ��ʵ��
            

        }
    }

    // �����߳�
    void recv()
    {
        while (true)
        {
            recv_len = socket.Receive(readbuff);
            
            // ����յ���Ϣ
            if (recv_len > 0)
            {
                // �����յ���byte���鷴���л�ΪList<FishInfo>��ʽ
                string str = Encoding.Default.GetString(readbuff, 0, recv_len);
                List<FishInfo> f = JsonConvert.DeserializeObject<List<FishInfo>>(str);

                // ������Ϣ����recv_que���У��Թ����̶߳�ȡ
                recv_que.Enqueue(f);
            }
            Thread.Sleep(100);
        }
    }

    // ������Ϣ��fishinfo�е�һ����½�һ��fishins��
    void create(FishIns fishins, FishInfo fishinfo)
    {
        //ID����
        fishins.idx = fishinfo.idx;

        //�����д�����Ӧ��gameobject�����͡�λ�á���С��������Ϣ����
        fishins.obj = (GameObject)Instantiate(Resources.Load(fishinfo.type));
        fishins.obj.transform.position = new Vector3(fishinfo.pos[0], fishinfo.pos[1], fishinfo.pos[2]);
        fishins.obj.transform.localScale = new Vector3(fishinfo.scale, fishinfo.scale, fishinfo.scale);

        //����gameobject����move�ű�
        fishins.obj.AddComponent<move>().idx= fishinfo.idx;

        //���ı�־λΪ�Ѹ���
        fishins.flag = true;
        
        //����Ŀ��λ��Ϊ��Ϣ��λ�ã�ʵ���ϲ��ƶ���
        fishins.target = new Vector3(fishinfo.pos[0], fishinfo.pos[1], fishinfo.pos[2]);

        //����Ŀ���СΪ��Ϣ�еĴ�С��ʵ���ϲ����С��
        fishins.scale = fishinfo.scale;

    }

    // ������Ϣ��fishinfo�е�һ�������һ��fishins��
    void set(FishIns fishins, FishInfo fishinfo)
    {
        // �������create()�ĺ�����ͬ��
        fishins.flag = true;
        fishins.target = new Vector3(fishinfo.pos[0], fishinfo.pos[1], fishinfo.pos[2]);
        fishins.scale = fishinfo.scale;
    }

}

