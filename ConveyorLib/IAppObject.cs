namespace ConveyorLib;

public interface IAppObject { }

public interface IAppObject<TApplication> : IAppObject where TApplication : IApplication { }
