using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class My3DMove : MonoBehaviour
{
    //[Header("============components============")]

    [System.Serializable]
    public class Com
    {
        public Transform CharacterRoot = null;

        public Transform TpCamRig = null;
        public Transform TpCam = null;

        public Transform FpRoot = null;
        public Transform FpCamRig = null;
        public Transform FpCam = null;

        public Rigidbody CharacterRig = null;

        public CapsuleCollider CapsuleCol = null;
    }


    public Com com = new Com();

    [Header("============Now Values============")]

    public Vector2 MouseMove = Vector2.zero;

    public Vector3 MoveDir = Vector3.zero;

    public Vector3 WorldMove = Vector3.zero;

    public bool IsCursorActive = false;

    public bool IsFPP = true;

    public bool IsMoving = false;

    public bool IsGrounded = false;

    public bool IsJumping = false;

    public bool IsFalling = false;

    public bool IsSlip = false;

    public float CurGravity;//���� ���ν�Ƽ�� y��

    private float LastJump;

    public float CurGroundSlopAngle;

    public float CurFowardSlopAngle;

    public Vector3 CurGroundNomal;

    public Vector3 CurGroundCross;

    public Vector3 CurHorVelocity;

    public float MoveAccel;

    //public Vector3 Capsuletopcenter = Vector3.zero;

    //public Vector3 Capsulebottomcenter = Vector3.zero;

    public Vector3 Capsuletopcenter => new Vector3(transform.position.x, transform.position.y + com.CapsuleCol.height - com.CapsuleCol.radius, transform.position.z);
    public Vector3 Capsulebottomcenter => new Vector3(transform.position.x, transform.position.y + com.CapsuleCol.radius, transform.position.z);

    [Header("============Options============")]

    public float RotMouseSpeed = 10f;

    public float MoveSpeed;

    public float MinAngle;

    public float MaxAngle;

    public float Gravity;//�߷°�(�����Ӵ����� ���������� ��)

    public float JumpPower = 120;//������ �ϸ� �ش� ������ curgravity���� �ٲ��ش�.

    public float JumpcoolTime = 1f;

    public LayerMask GroundMask;

    public float MaxSlop = 70;


    [Header("============TestVals============")]

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

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKey(KeyCode.W)) v+=1.0f;
        if (Input.GetKey(KeyCode.S)) v-=1.0f;
        if (Input.GetKey(KeyCode.A)) h-=1.0f;
        if (Input.GetKey(KeyCode.D)) h+=1.0f;



        MouseMove = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));
        MoveDir = new Vector3(h, 0, v);
        IsMoving= (MoveDir.sqrMagnitude > 0.01f) ;
        if (IsMoving)
            MoveAccel = Mathf.Lerp(MoveAccel, 1.0f, 0.1f);
        else
            MoveAccel = Mathf.Lerp(MoveAccel, 0.0f, 0.1f);
    }

    public void Move()
    {
        
        MoveDir.Normalize();

        if (IsFPP)
            WorldMove = com.FpRoot.TransformDirection(MoveDir);
        else
            WorldMove = com.TpCamRig.TransformDirection(MoveDir);


        WorldMove *= MoveSpeed;

        if(IsSlip)
        {
            Vector3 temp = -CurGroundCross;
            com.CharacterRig.velocity = new Vector3(WorldMove.x, CurGravity, WorldMove.z);
        }
        else
        {
            com.CharacterRig.velocity = new Vector3(WorldMove.x, CurGravity, WorldMove.z);//������ ����ߴ� ����
            //com.CharacterRig.velocity = new Vector3(CurHorVelocity.x*MoveAccel, CurGravity, CurHorVelocity.z* MoveAccel);//�̰� ���������϶��� �̿��ϵ���
        }
        
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



    public void CheckFront()
    {
        RaycastHit hit;
        CurFowardSlopAngle = 0;
        bool cast = Physics.CapsuleCast(Capsuletopcenter, Capsulebottomcenter, com.CapsuleCol.radius, WorldMove + Vector3.down, out hit, 5.0f,  GroundMask);
        if(cast)
        {
            CurFowardSlopAngle = Vector3.Angle(hit.normal, Vector3.up);



        }
        
    }

    private void OnDrawGizmosSelected()
    {
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Capsuletopcenter, com.CapsuleCol.radius);
        Gizmos.DrawWireSphere(Capsulebottomcenter, com.CapsuleCol.radius);

        if (CurFowardSlopAngle!=0)
        {

        }
    }

    public void CheckGround()
    {
        IsGrounded = false;
        IsSlip = false;
        CurGroundSlopAngle = 0;
        if(Time.time>=LastJump+0.2f)//�����ϰ� 0.2�� ������ ����˻縦 ���� �ʴ´�.
        {
            RaycastHit hit;
            bool cast = Physics.SphereCast(com.CapsuleCol.transform.position, com.CapsuleCol.radius - 0.2f, Vector3.down, out hit, 0.2f, GroundMask);
            if (cast)
            {
                IsGrounded = true;
                CurGroundSlopAngle = Vector3.Angle(hit.normal, Vector3.up);
                CurGroundNomal = hit.normal;
                if(CurGroundSlopAngle>=MaxSlop)
                {
                    IsSlip = true;
                }
                else
                {
                    IsSlip = false;
                }
            }
        }
        CurGroundCross = Vector3.Cross(CurGroundNomal, Vector3.up);
    }


    //��� ȸ���� �Ϸ�� ������ �����ؾ� �Ѵ�.
    //x,z���� �������� ��� y���� �������� ���� ����
    public void HorVelocity()
    {
        CurHorVelocity = com.FpCamRig.forward;


        if(IsSlip)
        {
            //�������� ���� �ٴ� ��簢�� -�� �ؼ� ȸ���� ��Ŵ
        }
        CurHorVelocity = Quaternion.AngleAxis(-CurGroundSlopAngle, CurGroundCross) * CurHorVelocity;//�̷������� ���͸� ȸ����ų �� �ִ�. ���� �������� �ʴ´�.

    }

    public void Falling()
    {
        float deltacof = Time.deltaTime * 10f;
        if(IsGrounded)
        {
            CurGravity = 0;
        }
        else
        {
            CurGravity -= deltacof * Gravity;
        }
    }

    public void Jump()
    {
        if(Time.time>=LastJump+JumpcoolTime)
        {
            LastJump = Time.time;
            IsJumping = true;
            CurGravity = JumpPower;
        }
        
    }


    public void Rotation()
    {
        //1 ��Ī �϶�
        //fp root�� �¿�ȸ��
        //fp cam rig�� ����ȸ��
        if(IsFPP)
        {
            RotateFP();
        }
        else//3 ��Ī �϶�
        //fp root�� �¿�ȸ��
        //tp cam rig�� �¿� �� ����ȸ��
        {
            RotateTP();
            RotateTPFP();
        }
    }

    //1��Ī�϶�ȸ�� 3��Ī�� ���ΰ� 1��Ī ĳ���͸� ȸ������ �ش�.
    public void RotateFP()
    {
        float xRotPrev = com.FpRoot.localEulerAngles.y;
        float xRotNext = xRotPrev + MouseMove.x * Time.deltaTime * 50f * RotMouseSpeed;
        xnext = xRotNext;
        //if (xRotNext > 180f)
        //    xRotNext -= 360f;

        float yRotPrev = com.FpCamRig.localEulerAngles.x;
        float yRotNext = yRotPrev + MouseMove.y * Time.deltaTime * 50f * RotMouseSpeed;
        ynext = yRotNext;


        com.FpRoot.localEulerAngles = Vector3.up * xRotNext;
        updown = com.FpRoot.localEulerAngles;
        com.FpCamRig.localEulerAngles = Vector3.right * yRotNext;
        rightleft = com.FpCamRig.localEulerAngles;

    }


    //3��Ī�϶�
    public void RotateTP()
    {
        float xRotPrev = com.TpCamRig.localEulerAngles.y;
        float xRotNext = xRotPrev + MouseMove.x * Time.deltaTime * 50f * RotMouseSpeed;
        
        //if (xRotNext > 180f)
        //    xRotNext -= 360f;

        float yRotPrev = com.TpCamRig.localEulerAngles.x;
        float yRotNext = yRotPrev + MouseMove.y * Time.deltaTime * 50f * RotMouseSpeed;



        //TpCamRig.localEulerAngles = Vector3.up * xRotNext;

        //TpCamRig.localEulerAngles = Vector3.right * yRotNext;

        com.TpCamRig.localEulerAngles = new Vector3(yRotNext, xRotNext, 0);
    }

    //�̋��� ���콺�ο����̴°Ծƴϰ� Ű���� �Է¿� ����  ȸ�� �ؾ� �ϱ⶧���� ���� ����
    public void RotateTPFP()
    {

        WorldMove = com.TpCamRig.TransformDirection(MoveDir);
        float curRotY = com.FpRoot.localEulerAngles.y;
        float nextRotY = Quaternion.LookRotation(WorldMove, Vector3.up).eulerAngles.y;

        if (!IsMoving) nextRotY = curRotY;

        if (nextRotY - curRotY > 180f) nextRotY -= 360f;
        else if (curRotY - nextRotY > 180f) nextRotY += 360f;


        com.FpRoot.eulerAngles = Vector3.up * Mathf.Lerp(curRotY, nextRotY, 0.1f);
    }


    

    void ChaingePerspective()
    {
        IsFPP = !IsFPP;
        com.FpCam.gameObject.SetActive(IsFPP);
        com.TpCam.gameObject.SetActive(!IsFPP);
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
        Falling();
        KeyInput();

        CheckGround();
        CheckFront();

        

        Rotation();
        HorVelocity();
        Move();
    }
}
