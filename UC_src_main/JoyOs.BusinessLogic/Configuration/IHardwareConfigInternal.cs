namespace JoyOs.BusinessLogic.Configuration
{
    public struct CpuConfig
    {
    }

    public struct GpuConfig
    {
    }

    public struct PhysicalMemoryConfig
    {
    }

    public struct HIDConfig
    {
    }

    public interface IHardwareConfigInternal : IBaseInterface
    {
        bool Init();
        bool Shutdown();



        
    }
}
