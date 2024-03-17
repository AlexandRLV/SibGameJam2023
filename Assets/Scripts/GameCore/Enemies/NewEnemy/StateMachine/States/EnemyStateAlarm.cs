using GameCore.Character.Animation;

namespace GameCore.Enemies.NewEnemy.StateMachine.States
{
    public class EnemyStateAlarm : EnemyStateBase
    {
        public override AnimationType AnimationType => AnimationType.Walk;
        public override EnemyStateType Type => EnemyStateType.Alarm;

        public EnemyStateAlarm(NewEnemyMovement enemyMovement) : base(enemyMovement)
        {
        }

        public override bool CanEnter(EnemyStateType prevState)
        {
            return movement.Vision.IsAlarmed;
        }

        public override bool CanExit(EnemyStateType nextState)
        {
            return false;
        }

        public override void OnEnter(EnemyStateType prevState)
        {
            movement.Agent.isStopped = false;
            movement.Agent.SetDestination(movement.Player.LastMovement.transform.position);
        }
    }
}