using BS;
using Harmony;
using System.Collections.Generic;
using UnityEngine;

namespace Halt
{
  public partial class TimeController
  {
    [HarmonyPatch(typeof(BrainData))]
    [HarmonyPatch("Update")]
    private static class CreatureBrainFreezePatch
    {
      [HarmonyPrefix]
      private static bool Prefix(BrainData __instance)
      {
        return !Instance.IsTimeFrozen;
      }
    }

    [HarmonyPatch(typeof(Creature))]
    [HarmonyPatch("UpdateActionCycle")]
    private static class CreatureActionFreezePatch
    {
      [HarmonyPrefix]
      private static bool Prefix(Creature __instance)
      {
        return !Instance.IsTimeFrozen;
      }
    }

    [HarmonyPatch(typeof(Creature))]
    [HarmonyPatch("TryAction")]
    private static class CreatureNewActionFreezePatch
    {
      [HarmonyPrefix]
      private static bool Prefix(Creature __instance)
      {
        return !Instance.IsTimeFrozen;
      }
    }

    [HarmonyPatch(typeof(Creature))]
    [HarmonyPatch("DelayedInit")]
    private static class NewCreatureFreezePatch
    {
      [HarmonyPostfix]
      private static void Postfix(Creature __instance)
      {
        if (Instance.IsTimeFrozen)
        {
          __instance.gameObject.AddComponent<DelayFreeze>();
        }
      }
    }

    [HarmonyPatch(typeof(RagdollHandle))]
    [HarmonyPatch("OnGrab")]
    private static class GrabbedRagdollUnFreezePatch
    {
      [HarmonyPostfix]
      private static void Postfix(RagdollHandle __instance)
      {
        if (Instance.IsTimeFrozen)
        {
          foreach (var part in __instance.ragdollPart.ragdoll.parts)
          {
            Instance.UnFreezeRigidbody(part.rb);
            Instance.UnFreezeRigidbody(part.targetRb);
          }
        }
      }
    }

    [HarmonyPatch(typeof(RagdollHandle))]
    [HarmonyPatch("OnUnGrab")]
    private static class UnGrabbedRagdollFreezePatch
    {
      [HarmonyPostfix]
      private static void Postfix(RagdollHandle __instance)
      {
        if (Instance.IsTimeFrozen
          && __instance.ragdollPart.ragdoll.grabbedHandleL == null
          && __instance.ragdollPart.ragdoll.grabbedHandleR == null)
        {
          foreach (var part in __instance.ragdollPart.ragdoll.parts)
          {
            Instance.FreezeRigidbody(part.rb);
            Instance.FreezeRigidbody(part.targetRb);
          }
        }
      }
    }

    [HarmonyPatch(typeof(CreatureAudio))]
    [HarmonyPatch("Update")]
    private static class CreatureVoiceFreezePatch
    {
      [HarmonyPrefix]
      private static bool Prefix(CreatureAudio __instance, AudioSource ___audioSource)
      {
        if (Instance.IsTimeFrozen)
        {
          ___audioSource.Pause();
          return false;
        }
        return true;
      }
    }

    [HarmonyPatch(typeof(Creature))]
    [HarmonyPatch("Despawn")]
    [HarmonyPatch(new System.Type[] { })]
    private static class CreatureDespawnUnfreezePatch
    {
      [HarmonyPrefix]
      private static bool Prefix(Creature __instance)
      {
        if (Instance.IsTimeFrozen)
        {
          Instance.UnFreezeCreature(__instance);
        }
        return true;
      }
    }

    [HarmonyPatch(typeof(Damager))]
    [HarmonyPatch("AddForceCoroutine")]
    internal static class UnfreezeOnDamagePatch
    {
      [HarmonyPrefix]
      internal static bool Prefix(Damager __instance, Collider targetCollider, Vector3 impulseVelocity, Vector3 contactPoint, BS.Ragdoll ragdoll)
      {
        if (TimeController.Instance.IsTimeFrozen)
        {
          if (ragdoll != null)
          {
            Vector3 localContactPoint = targetCollider.attachedRigidbody.transform.InverseTransformPoint(contactPoint);
            Vector3 velocity = __instance.data.addForceNormalize ? impulseVelocity.normalized : impulseVelocity;
            using (List<RagdollPart>.Enumerator enumerator = ragdoll.parts.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                RagdollPart ragdollPart = enumerator.Current;
                if (ragdollPart.HasCollider(targetCollider) && __instance.data.addForceRagdollPartMultiplier > 0f)
                {
                  Vector3 force = velocity * __instance.data.addForce * __instance.data.addForceRagdollPartMultiplier * Time.fixedDeltaTime;
                  var store = ragdollPart.rb.gameObject.GetComponent<StoredPhysicsData>();
                  if (store)
                  {
                    store.velocity += force / ragdollPart.rb.mass;
                  }
                }
                else if (__instance.data.addForceRagdollOtherMultiplier > 0f)
                {
                  Vector3 force = velocity * __instance.data.addForce * __instance.data.addForceRagdollOtherMultiplier * Time.fixedDeltaTime;
                  var store = ragdollPart.rb.gameObject.GetComponent<StoredPhysicsData>();
                  if (store)
                  {
                    store.velocity += force / ragdollPart.rb.mass;
                  }
                }
              }
            }
          }
        }

        return true;
      }
    }
  }
}
