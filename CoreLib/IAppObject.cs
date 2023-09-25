namespace CoreLib;

public interface IAppObject : IIdentity { } 

public interface IAppObject<TApplication> : IAppObject where TApplication : IApplication { }
