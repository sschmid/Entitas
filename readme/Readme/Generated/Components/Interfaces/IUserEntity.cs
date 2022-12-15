public partial interface IUserEntity
{
    UserComponent User { get; }
    bool HasUser { get; }

    IUserEntity AddUser(string newName, int newAge);
    IUserEntity ReplaceUser(string newName, int newAge);
    IUserEntity RemoveUser();
}
