using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using DAT;
using UnityEngine.SceneManagement;

public class CharacterCasterTests
{
    [UnityTest]
    public IEnumerator CharacterCasterTestsWithEnumeratorPasses()
    {
        SceneManager.LoadScene("CharacterMoveTest");
        yield return null;

        // 初期状態の設定
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        playerObject.transform.position = new Vector3(-15, -4, 0);



    }
}
