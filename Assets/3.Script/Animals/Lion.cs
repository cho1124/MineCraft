using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//  ★★프리팹과 이름을 맞추기위해 사자라 써놨지만 맘모스(초식동물)입니다!!!★★
public class Lion : Animal
{
    protected override void Update() {
        base.Update();
    }

    protected override void OnPlayerDetected() {
        StartCoroutine(FleeSequence());
    }

    IEnumerator FleeSequence() {
        // Jump
        ChangeState(State.Jump);
        yield return new WaitForSeconds(1.1f); // Jump duration

        // Rotate 180 degrees
        transform.Rotate(0, 180, 0);

        // Run twice
        ChangeState(State.Run);
        yield return new WaitForSeconds(2f); // Duration for the first run
        ChangeState(State.Run);
        yield return new WaitForSeconds(2f); // Duration for the second run

        // Return to a random state
        ChangeState(GetRandomState());
    }
}
