namespace ExifParser.Services
{
    public class PageTitleService
    {
        public event Action? OnChange;
        private string _title = "Exif Parser";

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnChange?.Invoke();
                }
            }
        }
    }
}
