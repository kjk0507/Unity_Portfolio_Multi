using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitStruct;
using EnumStruct;
using TMPro;
using UnityEditor;

public class UnitState : MonoBehaviour
{
    public Status status;
    public int x;
    public int y;

    private Vector3 originalPosition;
    private Vector3 offset;
    private bool isDragging = false;
    private int activeTouchId = -1;

    public Animator animator;
    private bool isMoving = false;
    private Position targetPosition;
    public float moveSpeed = 5.0f;

    void Start()
    {
        //if (status == null)
        //{
        //    status = new Status();
        //    status.SetPosition(0, 0);
        //}

        originalPosition = transform.position;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (status != null)
        {
            x = status.positionX; 
            y = status.positionY;            
        }

        CheckInputType();
        CheckMoving();
    }

    // ----------------------------------  입력  ---------------------------------- //
    public void CheckInputType()
    {
        if (Input.touchCount > 0)
        {
            // 모바일 터치 입력 처리
            //foreach (Touch touch in Input.touches)
            //{
            //    switch (touch.phase)
            //    {
            //        case TouchPhase.Began:
            //            OnTouchBegin(touch);
            //            break;
            //        case TouchPhase.Moved:
            //            OnTouchMove(touch);
            //            break;
            //        case TouchPhase.Ended:
            //        case TouchPhase.Canceled:
            //            OnTouchEnd(touch);
            //            break;
            //    }
            //}
        }
        else if (Input.GetMouseButtonDown(0))
        {
            // PC 마우스 입력 처리
            OnMouseDown();
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            OnMouseDrag();
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            OnMouseUp();
        }
    }

    // 초기 말 배치
    //void OnTouchBegin(Touch touch)
    //{
    //    // 터치 시작: 터치한 위치에 오브젝트가 있는지 확인
    //    Ray ray = Camera.main.ScreenPointToRay(touch.position);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit) && hit.transform == transform)
    //    {
    //        isDragging = true;
    //        activeTouchId = touch.fingerId;
    //        originalPosition = transform.position;

    //        Plane plane = new Plane(Vector3.up, 0);
    //        if (plane.Raycast(ray, out float distance))
    //        {
    //            Vector3 touchPosition = ray.GetPoint(distance);
    //            offset = originalPosition - touchPosition;
    //        }
    //    }
    //}

    //void OnTouchMove(Touch touch)
    //{
    //    if (isDragging && touch.fingerId == activeTouchId)
    //    {
    //        Plane plane = new Plane(Vector3.up, 0);
    //        Ray ray = Camera.main.ScreenPointToRay(touch.position);
    //        if (plane.Raycast(ray, out float distance))
    //        {
    //            Vector3 touchPosition = ray.GetPoint(distance);
    //            transform.position = touchPosition + offset;
    //        }
    //    }
    //}

    //void OnTouchEnd(Touch touch)
    //{
    //    if (isDragging && touch.fingerId == activeTouchId)
    //    {
    //        isDragging = false;
    //        activeTouchId = -1;

    //        Vector3 tempPosition = UnitManager.um_instance.GetPosition(transform.position);

    //        if (UnitManager.um_instance.IsRightPosition(tempPosition))
    //        {
    //            transform.position = tempPosition;
    //            status.SetPosition((int)tempPosition.x, (int)tempPosition.z);
    //        }
    //        else
    //        {
    //            transform.position = originalPosition;
    //            //status.SetPosition(0, 0);
    //        }
    //    }
    //}

    void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.transform == transform)
        {
            PlayerRole player = PlayManager.pm_instance.CheckCurRole();
            PlayPhase curPhase = PlayManager.pm_instance.CheckCurPhase();
            int blueUnitLayer = LayerMask.NameToLayer("BlueUnit");
            int redUnitLayer = LayerMask.NameToLayer("RedUnit");

            if (curPhase == PlayPhase.UnitPlacement && player == PlayerRole.Master && hit.transform.gameObject.layer == blueUnitLayer)
            {
                isDragging = true;
                originalPosition = transform.position;

                Plane plane = new Plane(Vector3.up, 0);
                if (plane.Raycast(ray, out float distance))
                {
                    Vector3 mousePosition = ray.GetPoint(distance);
                    offset = originalPosition - mousePosition;
                }
            } else if(curPhase == PlayPhase.UnitPlacement && player == PlayerRole.Participant && hit.transform.gameObject.layer == redUnitLayer)
            {
                isDragging = true;
                originalPosition = transform.position;

                Plane plane = new Plane(Vector3.up, 0);
                if (plane.Raycast(ray, out float distance))
                {
                    Vector3 mousePosition = ray.GetPoint(distance);
                    offset = originalPosition - mousePosition;
                }
            }
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Plane plane = new Plane(Vector3.up, 0);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 mousePosition = ray.GetPoint(distance);
                transform.position = mousePosition + offset;
            }
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;

            Vector3 tempPosition = UnitManager.um_instance.GetPosition(transform.position);
            Debug.Log("임시 위치 = " + tempPosition.x + " / " + tempPosition.z);        

            if (UnitManager.um_instance.IsRightPosition(tempPosition))
            {
                transform.position = tempPosition;
                status.SetPosition((int)tempPosition.x, (int)(tempPosition.z / 1.5f));
            }
            else
            {
                transform.position = originalPosition;
                //status.SetPosition(0, 0);
            }
        }
    }

    // ----------------------------------  함수  ---------------------------------- //
    // 유닛 이동
    public void CheckMoving()
    {
        if (isMoving)
        {
            Vector3 tempPosition = new Vector3(targetPosition.x, 0, targetPosition.y * 1.5f);
            transform.position = Vector3.MoveTowards(transform.position, tempPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, tempPosition) < 0.1f)
            {
                isMoving = false;
                ChangeAnimation(UnitAnimation.Idle);
                this.status.SetPosition(targetPosition.x, targetPosition.y);

                // 엔딩 분기인지 확인
                if (UnitManager.um_instance.CheckEnding())
                {
                    return;
                }

                // 기사가 탈출한 경우
                if (true)
                {

                }

                PlayManager.pm_instance.ChangeTurn();
            }
        }
    }

    public void MoveUnit(Position position)
    {
        PlayManager.pm_instance.IsActionTrue();

        ChangeAnimation(UnitAnimation.Walk); // 걷는 애니메이션 시작
        UnitManager.um_instance.ClearPositionLapping(); // 이동 타일 삭제

        targetPosition = position;
        Vector3 temp = new Vector3(position.x, 0, position.y * 1.5f);
        transform.LookAt(temp); // 목표 지점을 바라봄
        transform.Rotate(0, 180, 0); // 필요에 따라 회전 조정

        isMoving = true;
    }

    // 유닛 애니메이션 변경
    public void ChangeAnimation(UnitAnimation unitState)
    {
        animator.SetInteger("UnitState", (int)unitState);
    }

    public void AttackUnit(Position position)
    {
        PlayManager.pm_instance.IsActionTrue();

        Vector3 temp = new Vector3(position.x, 0, position.y * 1.5f);
        transform.LookAt(temp); // 목표 지점을 바라봄
        transform.Rotate(0, 180, 0); // 필요에 따라 회전 조정
        targetPosition = position;

        GameObject targetObj = UnitManager.um_instance.CheckObjByPosition(position);
        
        if(targetObj.GetComponent<UnitState>().status.GetIsReflection()) 
        {
            // 해당 유닛이 반사 아이템을 사용한 경우

        }
        else
        {
            // 반사 아이템이 없는 경우
            ChangeAnimation(UnitAnimation.Attack);

            // 당하는 쪽이 자신을 보도록
            Position curPosition = this.status.GetPosition();
            Vector3 tempPosition = new Vector3(curPosition.x, 0, curPosition.y * 1.5f);
            targetObj.transform.LookAt(tempPosition);
            targetObj.transform.Rotate(0, 180, 0);
        }        
    }

    public void DeathUnit(GameObject obj)
    {
        obj.GetComponent<UnitState>().ChangeAnimation(UnitAnimation.Death);        
        UnitManager.um_instance.ChangeUnitList(obj);

        StartCoroutine(WaitingDeath(4f));
    }

    private IEnumerator WaitingDeath(float delay)
    {
        ChangeAnimation(UnitAnimation.Idle);

        yield return new WaitForSeconds(delay);
        MoveUnit(targetPosition);
    }

    // ----------------------------------  애니메이션  ---------------------------------- //
    // 공격모션이 끝난 후(애니메이션에 추가)
    public void AttackEnd()
    {
        GameObject targetObj = UnitManager.um_instance.CheckObjByPosition(targetPosition);        

        DeathUnit(targetObj);
    }

    // 사망모션이 끝난 후(애니메이션에 추가)
    public void DeathEnd()
    {
        gameObject.SetActive(false);
    }
}
