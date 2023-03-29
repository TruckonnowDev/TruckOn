namespace MDispatch.NewElement
{
    public interface IOrientationHandler
    {
        void ForceLandscape();
        bool IsForceLandscape();
        void ForceSensor();
        void ForcePortrait();
    }
}