using System.Threading;
using HK;
using SoulLike.ActorControllers.Abilities;
using SoulLike.ActorControllers.AISystems;
using SoulLike.MasterDataSystem;

namespace SoulLike.ActorControllers.Brains
{
    public sealed class Enemy : IActorBrain
    {
        private readonly EnemySpec enemySpec;

        public Enemy(EnemySpec enemySpec)
        {
            this.enemySpec = enemySpec;
        }

        public void Attach(Actor actor, CancellationToken cancellationToken)
        {
            actor.gameObject.SetLayerRecursive(Layer.Enemy);
            actor.AddAbility<ActorTime>();
            actor.AddAbility<ActorMovement>();
            actor.AddAbility<ActorSceneViewHandler>();
            actor.AddAbility<ActorAnimation>();
            actor.AddAbility<ActorTargetHandler>();
            var actorWalk = actor.AddAbility<ActorWalk>();
            var actorWeaponHandler = actor.AddAbility<ActorWeaponHandler>();
            var actorStatus = actor.AddAbility<ActorStatus>();
            actor.ActivateAbilities();

            actorWeaponHandler.CreateWeapon(enemySpec.WeaponPrefab, Layer.EnemyWeapon);
            actorWalk.MoveSpeed = enemySpec.MoveSpeed;
            actorStatus.ApplySpec(enemySpec.ActorStatusSpec);
            var actorAIController = new ActorAIController(actor);
            actorAIController.Change(enemySpec.ActorAI);
        }
    }
}
