using BS;
using Harmony;
using UnityEngine;

namespace Halt
{
  public partial class TimeController
  {
    private static TimeController instance = new TimeController();

    public static TimeController Instance
    {
      get
      {
        return instance;
      }
    }

    private TimeController()
    {
    }

    private bool isTimeFrozen = false;
    public bool IsTimeFrozen
    {
      get
      {
        return isTimeFrozen;
      }
      set
      {
        if (value != isTimeFrozen)
        {
          if (value)
          {
            FreezeTime();
          }
          else
          {
            UnFreezeTime();
          }
        }
      }
    }

    public void FreezeTime()
    {
      if (isTimeFrozen)
      {
        return;
      }
      isTimeFrozen = true;

      foreach (Item item in Item.list)
      {
        FreezeItem(item);
      }

      foreach (Creature creature in Creature.list)
      {
        FreezeCreature(creature);
      }
    }

    public void UnFreezeTime()
    {
      if (!isTimeFrozen)
      {
        return;
      }
      isTimeFrozen = false;

      foreach (Item item in Item.list)
      {
        UnFreezeItem(item);
      }

      foreach (Creature creature in Creature.list)
      {
        UnFreezeCreature(creature);
      }
    }

    internal void FreezeGameObject(GameObject gameObject)
    {
      Item interactive = gameObject.GetComponent<Item>();
      if (interactive != null)
      {
        FreezeItem(interactive);
      }
      Creature creature = gameObject.GetComponent<Creature>();
      if (creature != null)
      {
        FreezeCreature(creature);
      }
    }

    [HarmonyPatch(typeof(SpellCaster))]
    [HarmonyPatch("SlowTime")]
    internal static class TimeFreezePatch
    {
      [HarmonyPrefix]
      internal static bool Prefix(SpellCaster __instance)
      {
        if (Instance.IsTimeFrozen)
        {
          Instance.UnFreezeTime();
        }
        else
        {
          Instance.FreezeTime();
        }
        return false;
      }
    }

    [HarmonyPatch(typeof(GameManager))]
    [HarmonyPatch("StopSlowMotion")]
    internal static class StopFreezePatch
    {
      [HarmonyPostfix]
      internal static void Postfix(GameManager __instance)
      {
        Instance.UnFreezeTime();
      }
    }
  }
}
