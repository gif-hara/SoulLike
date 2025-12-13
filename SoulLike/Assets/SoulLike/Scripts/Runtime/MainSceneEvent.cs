using SoulLike.ActorControllers;

namespace SoulLike
{
    public sealed class MainSceneEvent
    {
        public enum JudgementType
        {
            PlayerLose,
            PlayerWin
        }

        public readonly struct GameJudgement
        {
            public readonly JudgementType Judgement;

            public GameJudgement(JudgementType judgement)
            {
                Judgement = judgement;
            }
        }

        public readonly struct RestartGame
        {
            public readonly Actor Player;

            public readonly Actor Enemy;

            public RestartGame(Actor player, Actor enemy)
            {
                Player = player;
                Enemy = enemy;
            }
        }
    }
}
