namespace Churva.Interpreter.BluePrints
{
    public class ItemScope
    {
        public string ScopeName { get; set; }
        /// <summary>
        /// If a value is temporary, remove after usage. 
        /// </summary>
        public bool AutoRemove { get; set; }

        public ItemScope(string scope, bool autoRemove = false)
        {
            this.ScopeName = scope;
            this.AutoRemove = autoRemove;
        }
    }
}