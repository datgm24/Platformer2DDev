using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using DAT;
using UnityEngine.SceneManagement;
using System.Linq;

public class CharacterCasterTests
{
    [UnityTest]
    public IEnumerator CharacterCasterTestsWithEnumeratorPasses()
    {
        SceneManager.LoadScene("CharacterMoveTest");
        yield return null;

        // 初期状態の設定
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        var playerRb = playerObject.GetComponent<Rigidbody2D>();
        playerRb.position = new Vector3(-15, -4, 0);
        var playerBox = playerObject.GetComponent<BoxCollider2D>();

        // Playerを使って、落下のチェック
        int count = CharacterCaster.Cast(playerBox.bounds.center,
            playerBox, new Vector2(0, -1), LayerMask.GetMask("Floor"));
        Assert.That(count, Is.EqualTo(1), "Playerの落下チェック");
        Assert.That(CharacterCaster.CanMoveDistance(), Is.InRange(0.29f, 0.31f), "地面までの距離");

        // 右移動のチェック
        count = CharacterCaster.Cast(playerBox.bounds.center,
            playerBox, new Vector2(1, 0), LayerMask.GetMask("Floor"));
        Assert.That(count, Is.EqualTo(1), "Playerの右移動");

        // 斜面の角度を求める
        var normal = CharacterCaster.GetNormal();
        float angle = Vector2.SignedAngle(Vector2.up, normal);
        Assert.That(angle, Is.InRange(44, 46), "斜面の取得");
        Assert.That(CharacterCaster.CanMoveDistance(), Is.InRange(0.7f, 0.9f), "斜面までの距離");

        // 足元から上面への高さ
        playerRb.position = new Vector2(-8, -4.3f);
        count = CharacterCaster.Cast(playerBox.bounds.center, playerBox, new Vector2(1, 0), LayerMask.GetMask("Floor"));
        Assert.That(count, Is.EqualTo(1), "段差");
        Assert.That(CharacterCaster.CanMoveDistance, Is.InRange(0.45f, 0.55f), "段差までの距離");
        Assert.That(CharacterCaster.GetStepHeight(), Is.InRange(0.19f, 0.21f), "段差の高さ");

        // 最寄りのオブジェクトを求める
        playerRb.position = new Vector2(3, -4.3f);
        count = CharacterCaster.Cast(playerBox.bounds.center, playerBox, new Vector2(1, 0), LayerMask.GetMask("Floor", "GimmickCollision"));
        Assert.That(count, Is.EqualTo(2), "斜面とキューブに接触");
        Assert.That(CharacterCaster.CanMoveDistance, Is.InRange(0.48f, 0.52f), "斜面までの距離");
        var slope = GameObject.Find("Slope");
        Assert.That(slope, Is.Not.Null, "斜面取得");
        Assert.That(CharacterCaster.GetNearestObject(), Is.EqualTo(slope), "手前の斜面");

        var cube = GameObject.Find("Cube");
        Assert.That(cube, Is.Not.Null, "キューブ取得");
        bool isContains = false;
        foreach (var hit in CharacterCaster.RaycastHit2Ds)
        {
            if (hit.collider.gameObject == cube)
            {
                isContains = true; break;
            }
        }
        Assert.That(isContains, Is.True, "キューブもとれてる");
    }
}
