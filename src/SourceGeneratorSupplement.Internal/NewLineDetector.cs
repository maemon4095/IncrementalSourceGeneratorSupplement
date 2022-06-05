namespace SourceGeneratorSupplement.Internal;

public struct NewLineDetector
{
    public NewLineDetector()
    {
        this.Category = NewLineCategory.None;
    }

    public NewLineCategory Category { get; private set; }


    public bool Transition(char chara)
    {
        switch (this.Category)
        {
            case NewLineCategory.CR:
                switch (chara)
                {
                    case '\n':
                        this.Category = NewLineCategory.CRLF;
                        return false;
                    default:
                        this.Category = NewLineCategory.CR;
                        return false;
                }
            default:
                switch (chara)
                {
                    case '\r':
                        this.Category = NewLineCategory.CR;
                        return true;
                    case '\n':
                        this.Category = NewLineCategory.LF;
                        return false;
                    default:
                        this.Category = NewLineCategory.None;
                        return false;
                }
        }
    }

    public void Reset()
    {
        this.Category = NewLineCategory.None;
    }
}

public enum NewLineCategory
{
    None, CR, LF, CRLF
}