using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class move : MonoBehaviour
{

    public Vector3 start;       // ��ʼ��
    public Vector3 end;         // �յ�
    public Vector3 path;        // ·����ת���ã�
    public Vector3 scale_sta;   // ��ʼ��С
    public Vector3 scale_end;   // ��ֹ��С
    public float distance;      // ���루������յ�䣩��Ϊ��֤��ʱ�����Ҫ�����ٶȣ�
    public float scale_dis;     // ���루��ʼ��С����ֹ��С�䣩��Ϊ��֤��ʱ�����Ҫ�����ٶȣ�
    public float during = 0.5f; // ��ʱ�����ʱ�䣨Ӧ��������Ӧ��
    public int idx;             // ��gameobj������FishIns��ID
    float scale;                // ��ֹ��С��float�棩


    public void Start()
    {
        start = this.transform.position;
        scale_sta = this.transform.localScale;
    }


    public void Update()
    {
        // ����IDȥ�Ҹ�gameobj��Ӧ��Ŀ�����Ŀ���С
        end = ClientV2.fishins.Find(t => t.idx == this.idx).target;
        scale = ClientV2.fishins.Find(t => t.idx == this.idx).scale;
        scale_end = new Vector3(scale, scale, scale);

        // û�ε�λ��
        if (this.transform.position != end)
        {
            distance = Vector3.Distance(start, end);
            path = end - start;
            // �Ǿ���
            move2(end, during);
        }
        // �ε���
        else
        {
            // ����ǰλ����Ϊ�����
            start = this.transform.position;
            // ���ĸ�gameobj��Ӧ��FishInsΪδ����״̬
            ClientV2.fishins.Find(t => t.idx == this.idx).flag = false;
        }

        // û�䵽ָ����С
        if(this.transform.localScale != scale_end)
        {
            scale_dis = Vector3.Distance(scale_sta, scale_end);
            // �Ǿͱ�
            this.transform.localScale = Vector3.MoveTowards(transform.localScale, scale_end, (scale_dis / during) * Time.deltaTime);
        }
        // �䵽��
        else
        {
            // ����ǰ��С��Ϊ������С
            scale_sta = this.transform.localScale;
        }

    }

    public void move2(Vector3 target, float during)
    {
        // ƽ��
        this.transform.position = Vector3.MoveTowards(transform.position, target, (distance / during) * Time.deltaTime);

        // ת��
        if (path[0] > 0)
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(180, 90, 0));
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(180, -90, 0));
        }

    }


}