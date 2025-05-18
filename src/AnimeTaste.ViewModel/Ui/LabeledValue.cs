namespace AnimeTaste.ViewModel.Ui
{
    public class LabeledValue(string value, string label, bool disabled)
    {
        public string Value { get; set; } = value;
        public string Label { get; set; } = label;
        public bool Disabled { get; set; } = disabled;
    }
}
