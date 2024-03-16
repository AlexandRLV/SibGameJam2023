using GameCore.Enemies.NewEnemy.Parameters;
using GameCore.StateMachine;

namespace GameCore.Enemies.NewEnemy.StateMachine.States
{
    public abstract class EnemyStateBase : StateBase<EnemyStateType>
    {
        protected readonly NewEnemyMovement movement;
        protected readonly EnemyMainPreset preset;

        protected EnemyStateBase(NewEnemyMovement enemyMovement)
        {
            movement = enemyMovement;
            preset = enemyMovement.Preset;
        }
    }
}