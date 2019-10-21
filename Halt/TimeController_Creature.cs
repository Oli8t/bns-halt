using BS;

namespace Halt
{
  public partial class TimeController
  {
    private void FreezeCreature(Creature creature)
    {
      if (creature == Creature.player)
      {
        return;
      }

      creature.brain.Stop();
      creature.ClearActions();
      if (creature.animator)
      {
        creature.animator.speed = 0.0f;
      }
      if (creature.locomotion)
      {
        creature.locomotion.MoveStop();
      }
      if (creature.navigation)
      {
        creature.navigation.StopNavigation();
        creature.navigation.StopTurn();
      }
      if (creature.ragdoll.grabbedHandleL == null
        && creature.ragdoll.grabbedHandleR == null)
      {
        foreach (var part in creature.ragdoll.parts)
        {
          FreezeRigidbody(part.rb);
          FreezeRigidbody(part.targetRb);
        }
      }
    }

    private void UnFreezeCreature(Creature creature)
    {
      if (creature == Creature.player)
      {
        return;
      }

      if (creature.health.isKilled == false)
      {
        creature.brain.Start();
      }
      creature.animator.speed = 1.0f;
      foreach (var part in creature.ragdoll.parts)
      {
        UnFreezeRigidbody(part.rb);
        UnFreezeRigidbody(part.targetRb);
      }
    }
  }
}
