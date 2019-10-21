using BS;
using UnityEngine;

namespace Halt
{
  public partial class TimeController
  {
    private void FreezeItem(Item item)
    {
      if (item.data.category == ItemData.Category.Body ||     // Don't freeze the punchers/feet or weird stuff will probably happen
      item.tkHandler != null ||                               // Don't freeze telegrabbed stuff
      item.rb.constraints == RigidbodyConstraints.FreezeAll)  // Already frozen
      {
        return;
      }

      // Don't freeze things held by the player
      if ((Player.local.handLeft.bodyHand.interactor.grabbedHandle && Player.local.handLeft.bodyHand.interactor.grabbedHandle.item == item)
      || (Player.local.handRight.bodyHand.interactor.grabbedHandle && Player.local.handRight.bodyHand.interactor.grabbedHandle.item == item))
      {
        return;
      }

      FreezeRigidbody(item.rb);
    }

    private void UnFreezeItem(Item item)
    {
      var delay = item.gameObject.GetComponent<DelayFreeze>();
      if (delay)
      {
        GameObject.Destroy(delay);
      }
      UnFreezeRigidbody(item.rb);
      // Let all (moving) weapons do damage on time resuming
      if (item.rb.velocity.sqrMagnitude > 1 && item.handlers.Count == 0 && item.tkHandler == null)
      {
        item.RefreshCollision(true);
      }
    }
  }
}
