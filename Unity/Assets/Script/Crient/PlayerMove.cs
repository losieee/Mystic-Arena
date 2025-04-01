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
using Unity.VisualScripting;

public class PlayerMove : MonoBehaviour
{
    public Transform spot;
    public Transform player;
    public Transform miniPlayer;
    public Image death;
    public Image LowHp;
    public TextMeshProUGUI respawnCountdownText;   //카운트다운 UI 텍스트
    public Transform deathMark;
    public Image skillQCoolTime;    //스킬 쿨타임
    public Image skillWCoolTime;
    public Image skillECoolTime;
    public Image skillGCoolTime;

    [SerializeField]
    private Image infoBarImage; //화면 왼쪽 아래 HP_Bar
    [SerializeField]
    private Image characterBarImage;  //플레이어 위 HP_Bar
    [SerializeField]
    private FieldOfView fieldOfView;
    [SerializeField]
    private SmallFieldOfView smallFieldOfView;

    private float curHealth = 100; //현재 체력
    private float maxHealth = 100; //최대 체력
    private float healAmount; //회복량 설정
    private float interactionRange = 3f; //피 회복 가능한 거리 설정
    private bool isMove = false;    //움직임 여부
    private bool hasHealed = false; //체력 회복 여부 추적
    private float respawnTime = 5f;       //리스폰 시간 5초
    private float skillCoolTime;        // 스킬 쿨타임
    private float dashDistance = 3f;  // 도약 거리
    private float dashSpeed = 10f;    // 도약 속도
    private LayerMask obstacleMask;   // 장애물 감지

    private new Camera camera;
    private CapsuleCollider capsule;
    private Light spotLight;
    private NavMeshAgent agent;
    private Vector3 destination;

    public Transform respawnPoint; // 리스폰 위치
    private bool isDead = false; // 사망 여부 확인
    private bool isDashing = false; // 도약 중인지 확인
    private bool isGSkillCoolTime = false; // G 스킬 사용 가능 여부
    private bool isInvincible = false; // 무적 상태 여부


    public void SetHp(float amount) // Hp설정
    {
        maxHealth = amount;
        curHealth = maxHealth;
    }

    private void Awake()
    {
        camera = Camera.main;
        capsule = GetComponentInChildren<CapsuleCollider>();
        spotLight = GetComponentInChildren<Light>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;   // NavMeshAgent의 회전을 비활성화
        curHealth = 100;
        death.gameObject.SetActive(false);
        LowHp.gameObject.SetActive(false);
        deathMark.gameObject.SetActive(false);
        skillQCoolTime.gameObject.SetActive(false);
        skillWCoolTime.gameObject.SetActive(false);
        skillECoolTime.gameObject.SetActive(false);
        skillGCoolTime.gameObject.SetActive(false);
        healAmount = maxHealth / 2;
    }

    private void ChangeHealthBarAmount(float amount) //* HP 게이지 변경 
    {
        infoBarImage.fillAmount = amount;
        characterBarImage.fillAmount = amount;
    }

    public void Damage(float damage) // 데미지 받는 함수
    {
        if (maxHealth == 0 || curHealth <= 0 || isDead || isInvincible) // 무적 상태일 때 데미지 무시
            return;

        curHealth -= damage;
        if (curHealth <= 0)
        {
            StartCoroutine(RespawnCoroutine()); // 코루틴 시작
        }
        UpdateHealthUI(); // UI 업데이트
    }
    private void HealPlayer()
    {
        curHealth += healAmount;
        if (curHealth > maxHealth)
        {
            curHealth = maxHealth;
        }
        UpdateHealthUI();
        hasHealed = true; // 회복 완료 설정
    }
    private void UpdateHealthUI()
    {
        // 체력 비율 계산
        float healthRatio = curHealth / maxHealth;

        // HP 바 업데이트 (비율로 감소)
        infoBarImage.fillAmount = healthRatio;
        characterBarImage.fillAmount = healthRatio;

        // 체력 비율에 따른 색깔 변경 및 LowHp 활성화/비활성화
        if (healthRatio <= 0.15f) // 15% 이하
        {
            infoBarImage.color = Color.red;
            LowHp.gameObject.SetActive(true);
        }
        else if (healthRatio <= 0.5f) // 15% 초과 50% 이하
        {
            infoBarImage.color = new Color(1f, 0.5f, 0f); // 주황색 (RGB: 255, 128, 0)
            LowHp.gameObject.SetActive(false);
        }
        else // 50% 초과
        {
            infoBarImage.color = Color.white; // 원래 색깔 (흰색)
            LowHp.gameObject.SetActive(false);
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        isDead = true; // 사망 상태 설정
        death.gameObject.SetActive(true);
        deathMark.gameObject.SetActive(true); // 데스마크 활성화
        agent.isStopped = true; // 이동 중지
        capsule.enabled = false; // capsule collider 비활성화 (+죽는 이모션 추가)
        spotLight.enabled = false;  // light 비활성화
        LowHp.gameObject.SetActive(false); // 주황색 바탕 비활성화
        spot.gameObject.SetActive(false); // spot 비활성화 
        agent.enabled = false; // 네비게이션 비활성화
        




        // 플레이어가 죽었을 때 viewAngle을 0으로 설정
        if (fieldOfView != null)
        {
            fieldOfView.viewAngle = 0;
            smallFieldOfView.viewAngle = 0;
        }
        respawnTime = 5f; // 카운트다운 시간
        while (respawnTime > 0)
        {
            respawnCountdownText.text = Mathf.Round(respawnTime).ToString(); // 카운트 다운 UI에 표시
            yield return new WaitForSeconds(1f);
            respawnTime--; // 1초씩 감소
        }

        Respawn(); // 리스폰 함수 실행
        UpdateHealthUI(); // HP 바 색상 및 LowHp 상태 초기화

        if (fieldOfView != null)
        {
            fieldOfView.viewAngle = 69.47f;   //원래 값으로 변경
            smallFieldOfView.viewAngle = 360;
        }
    }

    private void Respawn()
    {
        capsule.enabled = true; // capsule collider 다시 활성화
        spotLight.enabled = true; // light 다시 활성화
        agent.enabled = true; // 네비게이션 다시 활성화
        agent.isStopped = false; // 다시 움직임
        isDead = false;

        UpdateHealthUI(); // HP 바 색상 및 LowHp 상태 초기화

        hasHealed = false; // 리스폰 시 회복 가능 상태로 초기화

        death.gameObject.SetActive(false); // 빨간색 바탕 비활성화
        deathMark.gameObject.SetActive(false); // 데스마크 비활성화

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
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "NPC")
                {
                    // NPC 클릭 시, 아직 회복하지 않았을 때만
                    if (!hasHealed)
                    {
                        float distance = Vector3.Distance(transform.position, hit.point);
                        if (distance <= interactionRange)
                        {
                            HealPlayer();
                        }
                    }
                    spot.gameObject.SetActive(false); // spot 비활성화 (NPC 클릭 시 spot 표시 안 함)
                }
                else
                {
                    spot.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    spot.position = hit.point;
                    spot.gameObject.SetActive(true);
                    SetDestination(hit.point); // NPC가 아닌 경우 자유롭게 이동
                }
            }
        }
        LookMoveDirection();

        if (isDead && LowHp.gameObject.activeSelf)
        {
            LowHp.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            skillCoolTime = 5f;
            StartCoroutine(CooldownRoutine(skillQCoolTime, skillCoolTime));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            skillCoolTime = 5f;
            StartCoroutine(CooldownRoutine(skillWCoolTime, skillCoolTime));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            skillCoolTime = 5f;
            StartCoroutine(CooldownRoutine(skillECoolTime, skillCoolTime));
        }
        else if (Input.GetKeyDown(KeyCode.G) && !isGSkillCoolTime) // 쿨타임 중일 때 실행 X
        {
            isGSkillCoolTime = true; // G 스킬 사용 중으로 설정
            skillCoolTime = 8f;
            StartCoroutine(CooldownRoutine(skillGCoolTime, skillCoolTime)); // 쿨타임 UI 업데이트
            StartCoroutine(DashForward()); // 도약 실행
        }
    }
    private IEnumerator DashForward() // 대쉬 함수
    {
        isDashing = true; // 도약 상태 활성화
        isInvincible = true; // 무적 상태 활성화

        float dashTime = dashDistance / dashSpeed; // 도약 지속 시간 계산
        Vector3 dashDirection = player.forward; // player가 바라보는 방향으로 이동

        // 장애물 체크 (Raycast)
        if (Physics.Raycast(transform.position, dashDirection, out RaycastHit hit, dashDistance, obstacleMask))
        {
            dashDistance = hit.distance - 0.5f; // 장애물과 충돌하지 않도록 조정
        }

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + dashDirection * dashDistance;

        // 부모 오브젝트를 이동
        while (elapsedTime < dashTime)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / dashTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // 최종 위치 조정
        isDashing = false; // 도약 상태 해제
        isInvincible = false; // 무적 상태 해제
    }

    IEnumerator CooldownRoutine(Image skillImage, float cooldownTime)   // 스킬 쿨타임 UI 표시
    {
        skillImage.gameObject.SetActive(true);
        float elapsedTime = 0f;

        while (elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            skillImage.fillAmount = 1 - (elapsedTime / cooldownTime);
            yield return null;
        }

        skillImage.gameObject.SetActive(false);

        if (skillImage == skillGCoolTime) // G 스킬이면 쿨타임 해제
        {
            isGSkillCoolTime = false;
        }
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
