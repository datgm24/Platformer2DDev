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
    public IEnumerator CharacterMoveTestsWithEnumeratorPasses()
    {
        // テストに必要な要素の初期化
        SceneManager.LoadScene("CharacterMoveTest");
        yield return null;

        var playerObject = GameObject.FindGameObjectWithTag("Player");
        Assert.That(playerObject, Is.Not.Null);

        var fallable = playerObject.GetComponent<IFallable>();
        Assert.That(fallable, Is.Not.Null);

        // 着地を待つ
        while (!fallable.IsGrounded)
        {
            yield return null;  // 1フレーム待つ
        }

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
            Assert.That(delta, Is.GreaterThan(overTime), "目的地につかなかった");
        }

        // -3.5で停止するはず
        targetX = -3.5f;
        startTime = Time.time;
        overTime = 1f/4f;
        while ((Time.time - startTime) < overTime)
        {
            yield return null;

            // 座標をオーバーしたらアウト
            Assert.That(playerObject.transform.position.x, Is.GreaterThan(targetX));
        }

        Assert.That(playerObject.transform.position.x, Is.LessThan(targetX));
        yield return new WaitForSeconds(1);
    }
}
