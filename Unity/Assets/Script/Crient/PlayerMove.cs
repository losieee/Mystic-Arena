using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMove : MonoBehaviour
{
    public float curHealth; // 현재 체력
    public float maxHealth; // 최대 체력

    public Transform spot;
    public Transform player;
    public Transform miniPlayer;
    public Transform dead;
    public TextMeshProUGUI countdownText;   // 카운트다운 UI 텍스트
    private float countdownTime = 5f;       // 5초 카운트다운

    [SerializeField]
    private Image infoBarImage;
    [SerializeField]
    private Image characterBarImage;
    [SerializeField]
    private FieldOfView fieldOfView;
    [SerializeField]
    private SmallFieldOfView smallFieldOfView;

    private new Camera camera;
    private CapsuleCollider capsule;
    private NavMeshAgent agent;
    private Vector3 destination;
    private bool isMove = false;

    public Transform respawnPoint; // 리스폰 위치
    private bool isDead = false; // 사망 여부 확인

    public void SetHp(float amount) // Hp설정
    {
        maxHealth = amount;
        curHealth = maxHealth;
    }

    private void Awake()
    {
        camera = Camera.main;
        capsule = GetComponentInChildren<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;   // NavMeshAgent의 회전을 비활성화
        curHealth = 100;
        dead.gameObject.SetActive(false);
    }

    private void ChangeHealthBarAmount(float amount) //* HP 게이지 변경 
    {
        infoBarImage.fillAmount = amount;
        characterBarImage.fillAmount = amount;
    }

    public void Damage(float damage) // 데미지 받는 함수
    {
        if (maxHealth == 0 || curHealth <= 0 || isDead) // 이미 체력이 0 이하이거나 죽었으면 패스
            return;

        curHealth -= damage;

        // 체력 비율 계산
        float healthRatio = curHealth / maxHealth;

        // HP 바 업데이트 (비율로 감소)
        infoBarImage.fillAmount = healthRatio;
        characterBarImage.fillAmount = healthRatio;


        if (curHealth <= 0)
        {
            if (!isDead) // 코루틴이 실행 중이 아닐 때만 실행
            {
                StartCoroutine(RespawnCoroutine()); // 리스폰 코루틴 실행
            }
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        isDead = true; // 사망 상태 설정
        capsule.enabled = false; // capsule collider 비활성화 (+죽는 이모션 추가)
        
        agent.isStopped = true; // 이동 중지
        agent.enabled = false; // 네비게이션 비활성화
        dead.gameObject.SetActive(true);

        // 플레이어가 죽었을 때 viewAngle을 0으로 설정
        if (fieldOfView != null)
        {
            fieldOfView.viewAngle = 0;
            smallFieldOfView.viewAngle = 0;
        }

        countdownTime = 5f; // 카운트다운 시간
        while (countdownTime > 0)
        {
            countdownText.text = Mathf.Round(countdownTime).ToString(); // 카운트 다운 UI에 표시
            yield return new WaitForSeconds(1f);
            countdownTime--; // 1초씩 감소
        }

        Respawn(); // 리스폰 함수 실행
        if (fieldOfView != null)
        {
            fieldOfView.viewAngle = 69.47f;   //원래 값으로 변경
            smallFieldOfView.viewAngle = 360;
        }
    }

    private void Respawn()
    {
        // capsule collider 다시 활성화
        capsule.enabled = true;
        agent.enabled = true; // 네비게이션 다시 활성화
        agent.isStopped = false; // 다시 움직임
        isDead = false;

        dead.gameObject.SetActive(false);
        //countdownText.gameObject.SetActive(false); // 카운트다운 UI 숨기기

        // 체력 회복
        curHealth = maxHealth;

        // HP 바 갱신
        infoBarImage.fillAmount = 1f;
        characterBarImage.fillAmount = 1f;

        // 리스폰 위치 설정
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isDead)
        {
            RaycastHit hit;
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                spot.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                spot.position = hit.point;
                spot.gameObject.SetActive(true);
                SetDestination(hit.point);
            }
        }
        LookMoveDirection();
    }

    private void SetDestination(Vector3 dest)
    {
        agent.SetDestination(dest);
        destination = dest;
        isMove = true;

        Vector3 direction = (destination - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            direction.y = 0; // y 값을 0으로 고정하여 수평 방향으로만 회전하도록 설정
            capsule.transform.forward = direction;
        }
    }

    private void LookMoveDirection()
    {
        if (isMove)
        {
            // spot 크기 줄여주기
            spot.transform.localScale = new Vector3(
                Mathf.Max(0, spot.transform.localScale.x - 0.05f),
                Mathf.Max(0, spot.transform.localScale.y - 0.05f),
                Mathf.Max(0, spot.transform.localScale.z - 0.05f));

            // spot 크기가 0이 되면 비활성화
            if (spot.transform.localScale.x <= 0)
            {
                spot.gameObject.SetActive(false);
            }

            Vector3 moveDirection = agent.velocity.normalized;
            if (moveDirection != Vector3.zero) // velocity가 0이 아닐 때만 회전
            {
                moveDirection.y = 0;    // y 값을 0으로 고정하여 수평 방향으로만 회전
                capsule.transform.forward = moveDirection;
            }
        }

        if (agent.velocity.magnitude == 0f)
        {
            isMove = false;
            return;
        }

        if (!isMove)
        {
            spot.gameObject.SetActive(false);
        }
    }
}
