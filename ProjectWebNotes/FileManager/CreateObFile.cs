namespace ProjectWebNotes.FileManager
{
    public static class CreateObFile
    {
        public static T Create<T>() where T : class, new()
        {
            return new T();       
        }
    }
}
