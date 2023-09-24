namespace Thoth.Tokenization;

public enum WhiteSpaceType
{
    Space = ' ',
    Tab = '\t',
    CarriageReturn = '\r',
    LineFeed = '\n'
}

public static class WhiteSpaceExtensions
{
    public static char ToCharacter(this WhiteSpaceType type) => (char) type;
}
