using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Player stats
    public float Hp;
    public float Damage;
    public float AttackRange = 2;
    public float MoveSpeed = 10f;
    public float RotationSpeed = 2f;
    public float AttackCooldown = 1f;
    public float SuperAttackCooldown = 2f;
    private float lastSuperAttackTime = 0;

    // Internal variables
    private float lastAttackTime = 0;
    private bool isDead = false;
    public Animator AnimatorController;
    public Button superAttackButton;
    public TextMeshProUGUI superAttackText;
    public Slider HpSlider;

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (Hp <= 0)
        {
            Die();
            return;
        }

        HandlePlayerMovement();
        HpSlider.value = Hp;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        var enemies = SceneManager.Instance.Enemies;
        Enemie closestEnemie = FindClosestEnemy(enemies);

        if (closestEnemie != null)
        {
            // Перевірка кулдауну атаки
            if (Time.time - lastAttackTime > AttackCooldown)
            {
                AttackEnemy(closestEnemie);
            }
        }
        

    }

    private void AttackEnemy(Enemie enemy)
    {
        var distance = Vector3.Distance(transform.position, enemy.transform.position);

        if (distance <= AttackRange)
        {
            Debug.Log("attack");
            Quaternion toRotation = Quaternion.LookRotation(enemy.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, RotationSpeed * Time.deltaTime);
            lastAttackTime = Time.time;
            StartCoroutine(ApplyDamageWithAnimation());
            AnimatorController.SetTrigger("Attack");
        }
        else
        {
            AnimatorController.SetTrigger("Attack");
            Debug.Log("miss");
        }

    }

    private IEnumerator ApplyDamageWithAnimation()
    {
        // Визначте урон відповідно до анімації
        float animationDamageMultiplier = 0.5f;  // Наприклад, урон буде половиною від звичайного

        // Очікування часу, коли анімація доходить до точки удару
        yield return new WaitForSeconds(0.5f);  // Наприклад, через половину тривалості анімації

        // Нанесення урону
        SceneManager.Instance.Enemies[0].Hp -= Damage * animationDamageMultiplier;  // Припустимо, що у вас є тільки один ворог

        // Якщо є багато ворогів, вам потрібно визначити, як надати урон кожному з них відповідно до анімації
    }

    private void TrySuperAttack()
    {
        var enemies = SceneManager.Instance.Enemies;
        Enemie closestEnemie = FindClosestEnemy(enemies);

        // Перевірка часу кулдауну
        if (Time.time - lastSuperAttackTime > SuperAttackCooldown)
        {
            // Перевірка наявності ворогів у радіусі
            if (closestEnemie != null)
            {
                SuperAttack(closestEnemie);
            }
        }
    }

    private void SuperAttack(Enemie enemy)
    {

        
        var distance = Vector3.Distance(transform.position, enemy.transform.position);

        if (distance <= AttackRange)
        {
            enemy.Hp -= Damage * 2;
            Quaternion toRotation = Quaternion.LookRotation(enemy.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, RotationSpeed * Time.deltaTime);
            AnimatorController.SetTrigger("SuperAttack");
        }
        else
        {
            AnimatorController.SetTrigger("SuperAttack");
        }
        StartCoroutine(SuperAttackCooldownRoutine());
    }

    private IEnumerator SuperAttackCooldownRoutine()
    {
        var timer = SuperAttackCooldown;
        while (timer > 0f)
        {
            // Відображення залишкового часу на текстовому елементі
            superAttackText.text = timer.ToString("F1") + " с";

            superAttackButton.interactable = false;
            // Зменшення таймера на час кадру
            timer -= Time.deltaTime;

            // Чекаємо один кадр
            yield return null;
        }
        lastSuperAttackTime = Time.time;
        superAttackText.text = "Double Attack";
        //yield return new WaitForSeconds(SuperAttackCooldown);
        superAttackButton.interactable = true;
    }

    // Метод, який викликається при натисканні кнопки супер атаки
    public void SuperAttackOnClick()
    {
        TrySuperAttack();
    }

    // Решта коду залишається без змін
    private void Die()
    {
        isDead = true;
        AnimatorController.SetTrigger("Die");
        SceneManager.Instance.GameOver();
    }

    private void HandlePlayerMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(h, 0, v);

        if (movementDirection != Vector3.zero)
        {
            CheckEnemyAround();
            Quaternion toRotation = Quaternion.LookRotation(movementDirection);
           // transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, RotationSpeed * Time.deltaTime);
            AnimatorController.SetFloat("Speed", movementDirection.magnitude);
            AnimatorController.SetBool("IsWalking", true);
        }
        else
        {
            AnimatorController.SetBool("IsWalking", false);
        }

        transform.Translate(movementDirection * MoveSpeed * Time.deltaTime);
    }

    private void CheckEnemyAround()
    {
        var enemies = SceneManager.Instance.Enemies;
        Enemie closestEnemie = FindClosestEnemy(enemies);

        if (closestEnemie != null)
        {
            var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
            if (distance <= AttackRange)
            {
                superAttackButton.interactable = true;
            }
            else
            {
                superAttackButton.interactable = false;
            }
        }
    }
    private Enemie FindClosestEnemy(List<Enemie> enemies)
    {
        Enemie closestEnemie = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemie = enemies[i];

            if (enemie == null)
            {
                continue;
            }

            if (closestEnemie == null)
            {
                closestEnemie = enemie;
                continue;
            }

            var distance = Vector3.Distance(transform.position, enemie.transform.position);
            var closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);

            if (distance < closestDistance)
            {
                closestEnemie = enemie;
            }
        }

        return closestEnemie;
    }
}