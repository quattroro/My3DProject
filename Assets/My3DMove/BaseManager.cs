using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    EnumTypes.ManagerTypes type;

    delegate void MyEventHandler();

    MyEventHandler[] events;


    public void Init()
    {
        //�̺�Ʈ���� �߰������ش�.
    }


    public void CallEvent(int num)
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
