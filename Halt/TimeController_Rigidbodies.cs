using UnityEngine;

namespace Halt
{
  public partial class TimeController
  {
    private void FreezeRigidbody(Rigidbody rigidbody)
    {
      StoredPhysicsData data = rigidbody.gameObject.GetComponent<StoredPhysicsData>();
      if (data == null)
      {
        data = rigidbody.gameObject.AddComponent<StoredPhysicsData>();
      }
      if (data != null)
      {
        data.StoreDataFromRigidBody(rigidbody);
      }
      else
      {
      }
      rigidbody.constraints = RigidbodyConstraints.FreezeAll;
      rigidbody.useGravity = false;
    }

    private void UnFreezeRigidbody(Rigidbody rigidbody)
    {
      // If we're already unfrozen, do nothing to avoid setting stale data
      if (rigidbody.constraints == RigidbodyConstraints.None)
      {
        return;
      }

      rigidbody.constraints = RigidbodyConstraints.None;
      rigidbody.useGravity = true;
      rigidbody.ResetInertiaTensor();
      StoredPhysicsData data = rigidbody.gameObject.GetComponent<StoredPhysicsData>();
      if (data == null)
      {
        data = rigidbody.gameObject.AddComponent<StoredPhysicsData>();
      }
      if (data != null)
      {
        data.SetRigidbodyFromStoredData(rigidbody);
      }
    }
  }
}
