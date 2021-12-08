namespace DimenshipBase
{
    public abstract class InstructionBase
    {
        public GameTime TimeIssued;
        public abstract ProcessBase GenerateProcess();
    }
}