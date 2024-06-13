using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using DAT;
using static UnityEngine.GraphicsBuffer;

public class CharacterMoveTests
{
    [UnityTest]
    public IEnumerator JumpTest()
    {
        // テストに必要な要素の初期化
        SceneManager.LoadScene("CharacterMoveTest");
        yield return null;

        var playerObject = GameObject.FindGameObjectWithTag("Player");
        Assert.That(playerObject, Is.Not.Null);

        // 入力更新を切る
        playerObject.GetComponent<PlayerInputToAction>().enabled = false;

        var fallable = playerObject.GetComponent<IFallable>();
        Assert.That(fallable, Is.Not.Null);

        // 着地待ち
        while (!fallable.IsGrounded)
        {
            yield return null;
        }

        // ジャンプの確認
        float jumpTime = Time.time;
        var moveable = playerObject.GetComponent<ICharacterMoveable>();
        Assert.That(moveable, Is.Not.Null);
        moveable.Jump();

        yield return new WaitForFixedUpdate();

        float vy = fallable.VelocityY;
        Assert.That(vy, Is.GreaterThan(0), "上昇");
        Assert.That(fallable.IsGrounded, Is.False, "空中");

        yield return new WaitForSeconds(0.1f);
        Assert.That(fallable.VelocityY, Is.LessThan(vy), "減速している");
        vy = fallable.VelocityY;

        // 空中ジャンプができない
        moveable.Jump();
        yield return new WaitForFixedUpdate();
        Assert.That(fallable.VelocityY, Is.LessThan(vy), "空中ジャンプしてない");

        // 着地したか
        float jumpH = 2.5f;
        // 初速を求める:jumpH = t * t * Physics2D.gravity.y / 2
        // t^2 = (Physics2D.gravity.y * 0.5f) / jumpH
        // 時間に負の値は不要なので、プラスのみ。
        // t = sqrt((Phyics2D.gravity.y * 0.5f) / jumpH)
        float t = Mathf.Sqrt(jumpH / (-Physics2D.gravity.y * 0.5f));
        yield return new WaitForSeconds(2f * t + 0.1f);
        Assert.That(fallable.IsGrounded, Is.True, "着地");
    }

    [UnityTest]
    public IEnumerator CharacterMoveTestsWithEnumeratorPasses()
    {
        // テストに必要な要素の初期化
        SceneManager.LoadScene("CharacterMoveTest");
        yield return null;

        var playerObject = GameObject.FindGameObjectWithTag("Player");
        Assert.That(playerObject, Is.Not.Null);

        // 入力更新を切る
        playerObject.GetComponent<PlayerInputToAction>().enabled = false;

        var fallable = playerObject.GetComponent<IFallable>();
        Assert.That(fallable, Is.Not.Null);

        // 着地を待つ
        while (!fallable.IsGrounded)
        {
            yield return null;  // 1フレーム待つ
        }

        // 足元の角度を確認
        Assert.That(Vector2.Dot(Vector2.up, fallable.FloorNormal.normalized), Is.GreaterThan(0.99f), "地面上向き");

        // 右へ移動開始
        var moveable = playerObject.GetComponent<ICharacterMoveable>();
        Assert.That(moveable, Is.Not.Null);
        moveable.Walk(1);   // 右へ移動

        // -15から-3.75まで、速度4で移動するには？
        // -3.75まで移動
        float targetX = -3.75f;
        float playerSpeed = 4f;
        float overTime = (targetX - playerObject.transform.position.x) / playerSpeed + 1f;
        float startTime = Time.time;
        while (playerObject.transform.position.x < targetX)
        {
            yield return null;

            float delta = Time.time - startTime;
            if (delta > overTime)
            {
                Assert.Fail("目的地につかなかった");
            }
        }

        // -3.5で停止するはず
        targetX = -3.5f;
        startTime = Time.time;
        overTime = 1f/4f;
        while ((Time.time - startTime) < overTime)
        {
            yield return null;

            // 座標をオーバーしたらアウト
            Assert.That(playerObject.transform.position.x, Is.LessThan(targetX));
        }

        yield return new WaitForSeconds(1);
    }
}
