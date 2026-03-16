namespace MWUtilityScripts
{
    public interface IConvertibleTo<out T>
    {
        T Convert();
    }
}
