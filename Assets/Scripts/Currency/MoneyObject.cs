using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MoneyObject : MonoBehaviour
{
    [SerializeField] int amount;
    public int Amount
    {
        get { return amount; }
        set { amount = value; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Money Object Created with amount: " + amount);
        StartCoroutine(OnSpawn());
        DOTween.Init();
        transform.DORotate(new Vector3(0, 0, 180), 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MoneyManager.Instance.AddMoney(amount);
            int number = DOTween.Kill(transform);
            Debug.Log("Killed tweens: " + number);
            Destroy(gameObject);
        }
    }

    IEnumerator OnSpawn()
    {
        yield return new WaitForSeconds(0.5f);
        BoxCollider2D bx2d = gameObject.AddComponent<BoxCollider2D>();
        bx2d.isTrigger = true;
        bx2d.size = new Vector2(2.2f, 1f);
    }
}
