using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shooter : MonoBehaviour
{
    private const int MaxShotPower = 5;
    private const int RecoverySeconds = 3;

    private int shotPower = MaxShotPower;
    private AudioSource shotSound;
    
    public GameObject[] candyPrefabs;
    public Transform candyParentTransform;
    public CandyManager candyManager;

    public float shotForce;

    public float shotTorque;

    public float baseWidth;
    // Start is called before the first frame update
    void Start()
    {
        shotSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            Shot();
    }
    
    //キャンディのプレハブからランダムに1つ選ぶ
    GameObject SampleCandy()
    {
        int index = Random.Range(0, candyPrefabs.Length);
        return candyPrefabs[index];
    }

    Vector3 GetInstantiatePosition()
    {
        //画面のサイズとInputの割合からキャンディ生成のポジションを計算
        float x = baseWidth * (Input.mousePosition.x / Screen.width) - (baseWidth / 2);
        return transform.position + new Vector3(x, 0, 0);
    }

    public void Shot()
    {
        //ストック不足など、キャンディを生成できない場合Shotしない
        if (candyManager.GetCandyAmount()<=0)
        {
            return;
        }

        if (shotPower <= 0)
        {
            return;
        }
        //プレハブからCandyオブジェクトを生成
        GameObject candy = (GameObject)Instantiate(
            SampleCandy(),
            GetInstantiatePosition(),
            Quaternion.identity
        );
        
        //生成したCandyオブジェクトの親をcandyParentTransformに設定する
        candy.transform.parent = candyParentTransform;
        
        //CandyオブジェクトのRigidbodyを取得して力と回転を加える
        Rigidbody candyRigidbody = candy.GetComponent<Rigidbody>();
        candyRigidbody.AddForce(transform.forward * shotForce);
        candyRigidbody.AddTorque(new Vector3(0,shotTorque,0));
        
        //Candyのストックを消費
        candyManager.ConsumeCandy();
        //ShotPowerを消費
        ConsumePower();
        
        //発射音を再生
        shotSound.Play();
    }

    private void OnGUI()
    {
        GUI.color = Color.black;
        
        //ShotPowerの残量を+で表示
        string label = "";
        for (int i = 0; i < shotPower; i++)
        {
            label = label + "+";
        }
        
        GUI.Label(new Rect(50,65,100,30),label );
    }

    void ConsumePower()
    {
        //ShotPowerを消費すると同時に回復のカウントをスタート
        shotPower--;
        StartCoroutine(RecoverPower());
    }

    IEnumerator RecoverPower()
    {
        //一定秒数後にshotPowerを回復
        yield return new WaitForSeconds(RecoverySeconds);
        shotPower++;
    }
}
