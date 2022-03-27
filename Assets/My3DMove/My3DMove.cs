using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class My3DMove : MonoBehaviour
{
    [Header("components")]
    public Transform CharacterRoot = null;

    public Transform TpCamRig = null;
    public Transform TpCam = null;

    public Transform FpRoot = null;
    public Transform FpCamRig = null;
    public Transform FpCam = null;

    public Rigidbody CharacterRig = null;

    public CapsuleCollider CapsuleCol = null;


    [Header("Now Values")]

    public bool IsCursorActive = false;

    public bool IsFPP = false;

    public Vector2 MouseMove = Vector2.zero;

    public Vector3 MoveDir = Vector3.zero;

    public Vector3 WorldMove = Vector3.zero;

    public bool IsMoving = false;

    public float CurGravity;

    [Header("Options")]

    public float MouseSpeed = 10f;

    public float MinAngle;

    public float MaxAngle;

    public float Gravity;

    [Header("TestVals")]

    public Vector3 updown;
    public float xnext;

    public Vector3 rightleft;
    public float ynext;
    void KeyInput()
    {
        float v = 0;
        float h = 0;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChaingePerspective();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ShowCursorToggle();
        }

        if (Input.GetKey(KeyCode.W)) v+=1.0f;
        if (Input.GetKey(KeyCode.S)) v-=1.0f;
        if (Input.GetKey(KeyCode.A)) h-=1.0f;
        if (Input.GetKey(KeyCode.D)) h+=1.0f;



        MouseMove = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));
        MoveDir = new Vector3(h, 0, v);
        IsMoving= (MoveDir.sqrMagnitude > 0.01f) ;
    }

    public void Move()
    {

        MoveDir.Normalize();

        WorldMove = FpRoot.TransformDirection(MoveDir);
        WorldMove *= 10f;

        CharacterRig.velocity = new Vector3(WorldMove.x, CharacterRig.velocity.y, WorldMove.z);
    }

    private void ShowCursorToggle()
    {
        IsCursorActive = !IsCursorActive;
        ShowCursor(IsCursorActive);
    }

    private void ShowCursor(bool value)
    {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void Rotation()
    {
        //1 인칭 일때
        //fp root로 좌우회전
        //fp cam rig로 상하회전
        if(IsFPP)
        {
            RotateFP();
        }
        else//3 인칭 일때
        //fp root로 좌우회전
        //tp cam rig로 좌우 및 상하회전
        {
            RotateTP();
            RotateTPFP();
        }
    }

    //1인칭일때회전 3인칭은 놔두고 1인칭 캐릭터만 회전시켜 준다.
    public void RotateFP()
    {
        float xRotPrev = FpRoot.localEulerAngles.y;
        float xRotNext = xRotPrev + MouseMove.x * Time.deltaTime * 50f * MouseSpeed;
        xnext = xRotNext;
        //if (xRotNext > 180f)
        //    xRotNext -= 360f;

        float yRotPrev = FpCamRig.localEulerAngles.x;
        float yRotNext = yRotPrev + MouseMove.y * Time.deltaTime * 50f * MouseSpeed;
        ynext = yRotNext;


        FpRoot.localEulerAngles = Vector3.up * xRotNext;
        updown = FpRoot.localEulerAngles;
        FpCamRig.localEulerAngles = Vector3.right * yRotNext;
        rightleft = FpCamRig.localEulerAngles;

    }


    //3인칭일때
    public void RotateTP()
    {
        float xRotPrev = TpCamRig.localEulerAngles.y;
        float xRotNext = xRotPrev + MouseMove.x * Time.deltaTime * 50f * MouseSpeed;
        
        //if (xRotNext > 180f)
        //    xRotNext -= 360f;

        float yRotPrev = TpCamRig.localEulerAngles.x;
        float yRotNext = yRotPrev + MouseMove.y * Time.deltaTime * 50f * MouseSpeed;



        //TpCamRig.localEulerAngles = Vector3.up * xRotNext;

        //TpCamRig.localEulerAngles = Vector3.right * yRotNext;

        TpCamRig.localEulerAngles = new Vector3(yRotNext, xRotNext, 0);
    }

    //이떄는 마우스로움직이는게아니고 키보드 입력에 따라서  회전 해야 하기때문에 따로 만듦
    public void RotateTPFP()
    {

        WorldMove = TpCamRig.TransformDirection(MoveDir);
        float curRotY = FpRoot.localEulerAngles.y;
        float nextRotY = Quaternion.LookRotation(WorldMove, Vector3.up).eulerAngles.y;

        if (!IsMoving) nextRotY = curRotY;

        if (nextRotY - curRotY > 180f) nextRotY -= 360f;
        else if (curRotY - nextRotY > 180f) nextRotY += 360f;


        FpRoot.eulerAngles = Vector3.up * Mathf.Lerp(curRotY, nextRotY, 0.1f);
    }


    

    void ChaingePerspective()
    {
        IsFPP = !IsFPP;
        FpCam.gameObject.SetActive(IsFPP);
        TpCam.gameObject.SetActive(!IsFPP);
    }

    private void Awake()
    {
        ChaingePerspective();
        ShowCursor(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();
        Rotation();
        Move();
    }
}
