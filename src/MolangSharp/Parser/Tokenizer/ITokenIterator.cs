namespace ConcreteMC.MolangSharp.Parser.Tokenizer
{
    public interface ITokenIterator
    {
        Token Next();
        void Step();
    }
}