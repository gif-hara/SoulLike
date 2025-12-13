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
    }
}
