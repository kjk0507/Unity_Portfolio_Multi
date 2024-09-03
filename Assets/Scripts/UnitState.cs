using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitStruct;
using EnumStruct;

public class UnitState : MonoBehaviour
{
    public Status status;
    public int x;
    public int y;

    private Vector3 originalPosition;
    private Vector3 offset;
    private bool isDragging = false;
    private int activeTouchId = -1;

    void Start()
    {
        //if (status == null)
        //{
        //    status = new Status();
        //    status.SetPosition(0, 0);
        //}

        originalPosition = transform.position;
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
    }

    public void CheckInputType()
    {
        if (Input.touchCount > 0)
        {
            // 모바일 터치 입력 처리
            foreach (Touch touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        OnTouchBegin(touch);
                        break;
                    case TouchPhase.Moved:
                        OnTouchMove(touch);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        OnTouchEnd(touch);
                        break;
                }
            }
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

    public void MovePosition()
    {
        Position position = this.status.GetPosition();
        Vector3 target = new Vector3(position.x, position.y, 0);
        transform.LookAt(target);
        transform.position = target;
    }

    // 초기 말 배치
    void OnTouchBegin(Touch touch)
    {
        // 터치 시작: 터치한 위치에 오브젝트가 있는지 확인
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.transform == transform)
        {
            isDragging = true;
            activeTouchId = touch.fingerId;
            originalPosition = transform.position;

            Plane plane = new Plane(Vector3.up, 0);
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 touchPosition = ray.GetPoint(distance);
                offset = originalPosition - touchPosition;
            }
        }
    }

    void OnTouchMove(Touch touch)
    {
        if (isDragging && touch.fingerId == activeTouchId)
        {
            Plane plane = new Plane(Vector3.up, 0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 touchPosition = ray.GetPoint(distance);
                transform.position = touchPosition + offset;
            }
        }
    }

    void OnTouchEnd(Touch touch)
    {
        if (isDragging && touch.fingerId == activeTouchId)
        {
            isDragging = false;
            activeTouchId = -1;

            Vector3 tempPosition = UnitManager.um_instance.GetPosition(transform.position);

            if (UnitManager.um_instance.IsRightPosition(tempPosition))
            {
                transform.position = tempPosition;
                status.SetPosition((int)tempPosition.x, (int)tempPosition.z);
            }
            else
            {
                transform.position = originalPosition;
                //status.SetPosition(0, 0);
            }
        }
    }

    void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.transform == transform)
        {
            PlayerRole player = PlayManager.pm_instance.CheckCurRole();
            int blueUnitLayer = LayerMask.NameToLayer("BlueUnit");
            int redUnitLayer = LayerMask.NameToLayer("RedUnit");

            if (player == PlayerRole.Master && hit.transform.gameObject.layer == blueUnitLayer)
            {
                isDragging = true;
                originalPosition = transform.position;

                Plane plane = new Plane(Vector3.up, 0);
                if (plane.Raycast(ray, out float distance))
                {
                    Vector3 mousePosition = ray.GetPoint(distance);
                    offset = originalPosition - mousePosition;
                }
            } else if(player == PlayerRole.Participant && hit.transform.gameObject.layer == redUnitLayer)
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
}
