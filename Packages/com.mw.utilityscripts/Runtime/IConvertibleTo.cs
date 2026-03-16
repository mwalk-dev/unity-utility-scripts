namespace Runtime
{
    public interface IConvertibleTo<out T>
    {
        T Convert();
    }
}
