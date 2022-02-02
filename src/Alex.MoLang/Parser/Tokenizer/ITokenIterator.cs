namespace Alex.MoLang.Parser.Tokenizer
{
    public interface ITokenIterator
    {
        Token Next();
        void Step();
    }
}