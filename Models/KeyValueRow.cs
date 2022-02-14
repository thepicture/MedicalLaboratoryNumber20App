namespace MedicalLaboratoryNumber20App.Models
{
    /// <summary>
    /// Представляет собой строку вида параметр-значение.
    /// </summary>
    public class KeyValueRow
    {
        public KeyValueRow(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
