using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CurState
{
    public bool IsCursorActive = false;

    public bool IsFPP = true;

    public bool IsMoving = false;

    public bool IsRunning = false;

    public bool IsGrounded = false;

    public bool IsJumping = false;

    public bool IsFalling = false;

    public bool IsSlip = false;

    public bool IsFowordBlock = false;

    public bool IsOnTheSlop = false;

    public bool IsAttacked = false;

    public bool IsOutofControl = false;
}

public class Character3DMove : MonoBehaviour
{
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

        //public CAnimationComponent animator = null;

        public Animator animator = null;
    }


    public Com com = new Com();

    [Header("============Now Values============")]

    public Vector2 MouseMove = Vector2.zero;

    public Vector3 MoveDir = Vector3.zero;

    public Vector3 WorldMove = Vector3.zero;

    public bool IsCursorActive = false;

    public bool IsFPP = true;

    public bool IsMoving = false;

    public bool IsRunning = false;

    public bool IsGrounded = false;

    public bool IsJumping = false;

    public bool IsFalling = false;

    public bool IsSlip = false;

    public bool IsFowordBlock = false;

    public bool IsOnTheSlop = false;

    public bool IsAttacked = false;

    public bool IsOutofControl = false;

    public float CurGravity;//���� ���ν�Ƽ�� y��

    private float LastJump;

    public float CurGroundSlopAngle;

    public float CurFowardSlopAngle;

    public Vector3 CurGroundNomal;

    public Vector3 CurGroundCross;

    public Vector3 CurHorVelocity;

    public Vector3 CurVirVelocity;

    public float MoveAccel;

    public Vector3 Capsuletopcenter => new Vector3(transform.position.x, transform.position.y + com.CapsuleCol.height - com.CapsuleCol.radius, transform.position.z);
    public Vector3 Capsulebottomcenter => new Vector3(transform.position.x, transform.position.y + com.CapsuleCol.radius, transform.position.z);

    [Header("============Options============")]

    public float RotMouseSpeed = 10f;

    public float MoveSpeed;

    public float RunSpeed;

    public float MinAngle;

    public float MaxAngle;

    public float Gravity;//�߷°�(�����Ӵ����� ���������� ��)

    public float JumpPower = 120;//������ �ϸ� �ش� ������ curgravity���� �ٲ��ش�.

    public float JumpcoolTime = 1f;

    public LayerMask GroundMask;

    public float MaxSlop = 70;

    public float SlopAccel;//(�߷°��� ���� �̲������� ���������� ��)

    [Header("============TestVals============")]

    public Vector3 updown;
    public float xnext;

    public Vector3 rightleft;
    public float ynext;

    public Vector3 testtart;
    public Vector3 testend;


    public Transform testcube;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!IsRunning)
                IsRunning = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (IsRunning)
                IsRunning = false;
        }
        //if (com.animator == null)
        //{
        //    com.animator = (CAnimationComponent)ComponentManager.GetI.GetMyComponent(EnumTypes.eComponentTypes.AnimatorCom);
        //}

        MouseMove = new Vector2(0, 0);
        MoveDir = new Vector3(h, 0, v);
        IsMoving = false;

        Input.GetAxisRaw("Mouse ScrollWheel");//���� �ܾƿ��� ���

        //���� ���� ���� ������ �� ����.
        if (!com.animator.GetBool(EnumTypes.eAnimationState.Attack))
        {
            if (Input.GetKey(KeyCode.W)) v += 1.0f;
            if (Input.GetKey(KeyCode.S)) v -= 1.0f;
            if (Input.GetKey(KeyCode.A)) h -= 1.0f;
            if (Input.GetKey(KeyCode.D)) h += 1.0f;



            MouseMove = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));
            MoveDir = new Vector3(h, 0, v);
            IsMoving = (MoveDir.sqrMagnitude > 0.01f);


            if (IsMoving)
            {
                MoveAccel = Mathf.Lerp(MoveAccel, 1.0f, 0.1f);
                com.animator.SetBool("Idle", false);


                com.animator.SetBool("Move", true);
                if (IsRunning)
                    com.animator.SetInt("MoveNum", 1);
                else
                    com.animator.SetInt("MoveNum", 0);

            }
            else
            {
                MoveAccel = Mathf.Lerp(MoveAccel, 0.0f, 0.1f);
                com.animator.SetBool("Idle", true);
                com.animator.SetBool("Move", false);


            }
        }
    }

    //������
    public void Rolling()
    {
        //���� �־�� ������ ����
        if (!IsGrounded)
            return;





    }

    IEnumerator Rolling_Coroutine()
    {





        yield break;
    }




    public void Move()
    {

        MoveDir.Normalize();

        if (IsFPP)
            WorldMove = com.FpRoot.TransformDirection(MoveDir);
        else
            WorldMove = com.TpCamRig.TransformDirection(MoveDir);


        WorldMove *= (IsRunning) ? RunSpeed : MoveSpeed;


        if (IsFowordBlock && !IsGrounded || IsJumping && IsGrounded || IsJumping && IsFowordBlock)
        {
            WorldMove.x = 0;
            WorldMove.z = 0;
        }


        if (IsOnTheSlop)
        {
            CurVirVelocity = new Vector3(0, CurGravity + SlopAccel, 0);//�߷°��� ���ο����� �̲��������� ���ӵ���

            //CurVirVelocity = new Vector3(0, 0, 0);
            if (IsSlip)
            {
                CurHorVelocity = new Vector3(WorldMove.x, 0.0f, WorldMove.z);
                Vector3 temp = -CurGroundCross;
                //CurHorVelocity = new Vector3(WorldMove.x, 0, WorldMove.z);
                temp = com.FpRoot.forward;
                CurHorVelocity = Quaternion.AngleAxis(-CurGroundSlopAngle, CurGroundCross) * CurHorVelocity;//���ο� ���� y�� �̵�����
                CurHorVelocity *= MoveSpeed;
                CurHorVelocity *= -1.0f;
                //com.CharacterRig.velocity = new Vector3(CurHorVelocity.x, CurGravity, CurHorVelocity.z);
                //com.CharacterRig.velocity = CurHorVelocity + CurVirVelocity;
            }
            else
            {
                CurHorVelocity = new Vector3(WorldMove.x, 0.0f, WorldMove.z);
                CurHorVelocity = Quaternion.AngleAxis(-CurGroundSlopAngle, CurGroundCross) * CurHorVelocity;//���ο� ���� y�� �̵�����
                //com.CharacterRig.velocity = new Vector3(WorldMove.x, CurGravity, WorldMove.z);//������ ����ߴ� ����
                //com.CharacterRig.velocity = new Vector3(CurHorVelocity.x*MoveAccel, CurGravity, CurHorVelocity.z* MoveAccel);//�̰� ���������϶��� �̿��ϵ���
            }
            Debug.DrawLine(this.transform.position, this.transform.position + (CurHorVelocity + CurVirVelocity));
            com.CharacterRig.velocity = CurHorVelocity + CurVirVelocity;
        }
        else
        {
            com.CharacterRig.velocity = new Vector3(WorldMove.x, CurGravity, WorldMove.z);
        }
        //com.CharacterRig.velocity = new Vector3(WorldMove.x, CurGravity, WorldMove.z);
        //com.CharacterRig.velocity = new Vector3(WorldMove.x, CurGravity, WorldMove.z);


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
        IsFowordBlock = false;
        Vector3 temp = new Vector3(WorldMove.x, 0, WorldMove.z);
        temp = com.FpRoot.forward /*+ Vector3.down*/;
        bool cast = Physics.CapsuleCast(Capsuletopcenter, Capsulebottomcenter, com.CapsuleCol.radius - 0.2f, temp.normalized, out hit, 0.3f);
        if (cast)
        {
            CurFowardSlopAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (CurFowardSlopAngle >= 70.0f)
            {
                IsFowordBlock = true;
            }


        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Capsuletopcenter, com.CapsuleCol.radius);
        Gizmos.DrawWireSphere(Capsulebottomcenter, com.CapsuleCol.radius);

        //
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(testtart, testend);

        //��������
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(this.transform.position, this.transform.position + CurGroundCross);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(this.transform.position, this.transform.position + (CurHorVelocity + CurVirVelocity));
        if (CurFowardSlopAngle != 0)
        {

        }
    }

    public void CheckGround()
    {
        IsGrounded = false;
        IsSlip = false;
        IsOnTheSlop = false;
        CurGroundSlopAngle = 0;
        if (Time.time >= LastJump + 0.2f)//�����ϰ� 0.2�� ������ ����˻縦 ���� �ʴ´�.
        {
            //Debug.Log("�ٴ�");
            RaycastHit hit;
            testtart = com.CapsuleCol.transform.position;
            testend = testtart + Vector3.down * 0.2f;
            testend = testtart;
            //Debug.DrawRay(testtart, Vector3.down, Color.yellow);
            bool cast = Physics.SphereCast(Capsulebottomcenter, com.CapsuleCol.radius - 0.2f, Vector3.down, out hit, com.CapsuleCol.radius - 0.1f);

            if (cast)
            {
                testend = hit.point;
                IsGrounded = true;
                CurGroundNomal = hit.normal;
                CurGroundSlopAngle = Vector3.Angle(hit.normal, Vector3.up);

                CurFowardSlopAngle = Vector3.Angle(hit.normal, MoveDir) - 90f;

                if (CurGroundSlopAngle > 1.0f)
                {
                    IsOnTheSlop = true;
                    if (CurGroundSlopAngle >= MaxSlop)
                    {
                        IsSlip = true;
                    }
                }
                CurGroundCross = Vector3.Cross(CurGroundNomal, Vector3.up);

            }
        }

    }


    //��� ȸ���� �Ϸ�� ������ �����ؾ� �Ѵ�.
    //x,z���� �������� ��� y���� �������� ���� ����
    public void HorVelocity()
    {
        //CurHorVelocity = com.FpCamRig.forward;


        if (IsSlip)
        {
            //�������� ���� �ٴ� ��簢�� -�� �ؼ� ȸ���� ��Ŵ
        }
        CurHorVelocity = Quaternion.AngleAxis(-CurGroundSlopAngle, CurGroundCross) * CurHorVelocity;//�̷������� ���͸� ȸ����ų �� �ִ�. ���� �������� �ʴ´�.

    }

    public void Falling()
    {
        float deltacof = Time.deltaTime * 10f;

        if (IsGrounded)
        {
            if (IsJumping)
                IsJumping = false;
            CurGravity = 0;
            Gravity = 1;
        }
        else
        {
            Gravity += 0.098f;
            CurGravity -= deltacof * Gravity;
        }
    }

    public void Jump()
    {
        if (Time.time >= LastJump + JumpcoolTime)
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
        if (IsFPP)
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
        float nextRotY = 0;
        WorldMove = com.TpCamRig.TransformDirection(MoveDir);
        float curRotY = com.FpRoot.localEulerAngles.y;
        if (WorldMove.sqrMagnitude != 0)
            nextRotY = Quaternion.LookRotation(WorldMove, Vector3.up).eulerAngles.y;

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
        com.animator = ComponentManager.GetI.GetMyComponent(EnumTypes.eComponentTypes.AnimatorCom) as CAnimationComponent;
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
