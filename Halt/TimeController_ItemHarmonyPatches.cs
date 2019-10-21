using BS;
using Harmony;
using System.Collections.Generic;
using UnityEngine;

namespace Halt
{
  public partial class TimeController
  {
    [HarmonyPatch(typeof(Item))]
    [HarmonyPatch("OnGrab")]
    internal static class UnFreezeGrabbedInteractiveObjectPatch
    {
      [HarmonyPostfix]
      internal static void Postfix(Item __instance, Handle handle, Interactor interactor)
      {
        if (interactor == Player.local.handLeft.bodyHand.interactor
        || interactor == Player.local.handRight.bodyHand.interactor)
        {
          Instance.UnFreezeItem(__instance);

          // Enable yoinking
          List<Interactor> toDrop = new List<Interactor>();
          foreach (var handler in __instance.handlers)
          {
            if (handler != Player.local.handLeft.bodyHand.interactor
            && handler != Player.local.handRight.bodyHand.interactor)
            {
              toDrop.Add(handler);
            }
          }
          foreach (var handler in toDrop)
          {
            handler.UnGrab(false);
          }
        }
      }
    }

    [HarmonyPatch(typeof(Item))]
    [HarmonyPatch("OnTeleGrab")]
    internal static class UnFreezeTeleGrabbedInteractiveObjectPatch
    {
      [HarmonyPostfix]
      internal static void Postfix(Item __instance, Handle handle, Telekinesis teleGrabber)
      {
        Instance.UnFreezeItem(__instance);
      }
    }

    [HarmonyPatch(typeof(Item))]
    [HarmonyPatch("OnTeleUnGrab")]
    internal static class FreezeTeleUnGrabbedInteractiveObjectPatch
    {
      [HarmonyPostfix]
      internal static void Postfix(Item __instance, Handle handle, Telekinesis teleGrabber)
      {
        if (Instance.IsTimeFrozen && handle.handlers.Count == 0)
        {
          __instance.gameObject.AddComponent<DelayFreeze>();
        }
      }
    }

    [HarmonyPatch(typeof(Item))]
    [HarmonyPatch("OnUnGrab")]
    internal static class FreezeUnGrabbedInteractiveObjectPatch
    {
      [HarmonyPostfix]
      internal static void Postfix(Item __instance, Handle handle, Interactor interactor, bool throwing)
      {
        if (Instance.IsTimeFrozen)
        {
          __instance.gameObject.AddComponent<DelayFreeze>();
        }
      }
    }

    [HarmonyPatch(typeof(Item))]
    [HarmonyPatch("Update")]
    internal static class PenetrationFollowFreezePatch
    {
      [HarmonyPostfix]
      internal static void Postfix(Item __instance)
      {
        if (Instance.IsTimeFrozen)
        {
          foreach (var collision in __instance.collisions)
          {
            if (collision.damageStruct.penetrationJoint)
            {
              if (collision.damageStruct.penetrationRb.constraints != RigidbodyConstraints.FreezeAll)
              {
                Instance.UnFreezeItem(__instance);
                return;
              }
            }
          }
          if (__instance.handlers.Count == 0 && __instance.gameObject.GetComponent<DelayFreeze>() == null)
          {
            Instance.FreezeItem(__instance);
          }
          if (__instance.handlers.Count > 0)
          {
            Instance.UnFreezeItem(__instance);
          }
        }
      }
    }

    [HarmonyPatch(typeof(BowString))]
    [HarmonyPatch("FixedUpdate")]
    internal static class UnFreezeBowInUsePatch
    {
      [HarmonyPostfix]
      internal static void Postfix(BowString __instance, Handle ___stringHandle)
      {
        if (Instance.IsTimeFrozen)
        {
          if (___stringHandle.item.rb.constraints == RigidbodyConstraints.FreezeAll)
          {
            if (__instance.restedArrow)
            {
              Instance.FreezeItem(__instance.restedArrow);
            }
            if (__instance.nockedArrow)
            {
              Instance.FreezeItem(__instance.nockedArrow);
            }
          }
          else
          {
            if (__instance.restedArrow)
            {
              Instance.UnFreezeItem(__instance.restedArrow);
            }
            if (__instance.nockedArrow)
            {
              Instance.UnFreezeItem(__instance.nockedArrow);
            }
          }
        }
      }
    }

    [HarmonyPatch(typeof(BowString))]
    [HarmonyPatch("OnShootUnrest")]
    internal static class UnFreezeShotRestArrowPatch
    {
      [HarmonyPrefix]
      internal static void Prefix(BowString __instance)
      {
        if (Instance.IsTimeFrozen)
        {
          if (__instance.restedArrow)
          {
            Instance.UnFreezeItem(__instance.restedArrow);
          }
          if (__instance.nockedArrow)
          {
            Instance.UnFreezeItem(__instance.nockedArrow);
          }
        }
      }
    }

    [HarmonyPatch(typeof(BowString))]
    [HarmonyPatch("OnShootUnnock")]
    internal static class UnFreezeShotNockArrowPatch
    {
      [HarmonyPrefix]
      internal static void Prefix(BowString __instance)
      {
        if (Instance.IsTimeFrozen)
        {
          if (__instance.restedArrow)
          {
            Instance.UnFreezeItem(__instance.restedArrow);
          }
          if (__instance.nockedArrow)
          {
            Instance.UnFreezeItem(__instance.nockedArrow);
          }
        }
      }
    }

    [HarmonyPatch(typeof(BowString))]
    [HarmonyPatch("Unrest")]
    internal static class UnFreezeUnrestArrowPatch
    {
      [HarmonyPrefix]
      internal static void Prefix(BowString __instance)
      {
        if (Instance.IsTimeFrozen)
        {
          if (__instance.restedArrow)
          {
            Instance.UnFreezeItem(__instance.restedArrow);
            __instance.restedArrow.gameObject.AddComponent<DelayFreeze>();
          }
        }
      }
    }

    [HarmonyPatch(typeof(BowString))]
    [HarmonyPatch("Unnock")]
    internal static class UnFreezeUnnockArrowPatch
    {
      [HarmonyPrefix]
      internal static void Prefix(BowString __instance)
      {
        if (Instance.IsTimeFrozen)
        {
          if (__instance.nockedArrow)
          {
            Instance.UnFreezeItem(__instance.nockedArrow);
            __instance.nockedArrow.gameObject.AddComponent<DelayFreeze>();
          }
        }
      }
    }
  }
}
