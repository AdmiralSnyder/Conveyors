namespace CoreLib;

public interface IIdentity
{
    string ID { get; }
}

public interface IIdentitySet : IIdentity
{
    new string ID { get; set; }

}
