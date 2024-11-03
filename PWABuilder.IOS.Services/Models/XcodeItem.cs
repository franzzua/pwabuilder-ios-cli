namespace PWABuilder.IOS.Services.Models
{
    public abstract class XcodeItem
    {
        protected XcodeItem(string path)
        {
            this.ItemPath = path;
        }

        public string ItemPath { get; protected init; }

        public abstract string Name { get; protected set; }
    }
}
