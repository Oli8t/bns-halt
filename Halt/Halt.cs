using BS;
using Harmony;
using System.Reflection;
using UnityEngine;

namespace Halt
{
  public class Halt : LevelModule
  {
    private HarmonyInstance harmony = null;

    public override void OnLevelLoaded(LevelDefinition levelDefinition)
    {
      base.OnLevelLoaded(levelDefinition);

      try
      {
        harmony = HarmonyInstance.Create("Halt");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        Debug.Log("Halt successfully loaded!");
      }
      catch (System.Exception e)
      {
        Debug.LogException(e);
      }
    }
  }
}
