using UnityEngine;

namespace Halt
{
  internal class DelayFreeze : MonoBehaviour
  {
    private float timeToFreeze = 0.2f;

    void Update()
    {
      timeToFreeze -= Time.deltaTime;
      if (timeToFreeze <= 0f)
      {
        Freeze();
      }
    }

    private void Freeze()
    {
      TimeController.Instance.FreezeGameObject(gameObject);
      GameObject.Destroy(this);
    }
  }
}
