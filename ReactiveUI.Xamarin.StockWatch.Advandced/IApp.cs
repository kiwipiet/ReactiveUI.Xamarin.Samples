namespace ReactiveUI.Sample.Routing
{
    public interface IApp
    {
        AppDb AppDb { get; }
        AppService AppService { get; }
    }
}