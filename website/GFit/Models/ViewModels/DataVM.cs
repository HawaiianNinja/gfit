namespace gFit.Models.ViewModels
{
    public class DataVM
    {
        public DataVM(string data, string caption)
        {
            Data = data;
            Caption = caption;
        }

        public DataVM(object data, string caption)
        {
            Data = data.ToString();
            Caption = caption;
        }

        public string Data { get; set; }
        public string Caption { get; set; }
    }
}