using System.Collections.Generic;

namespace ServerWordAddIn
{
    public class FormSchema
    {
        public string Title { get; set; }
        public bool Rtl { get; set; }
        public List<FieldSchema> Fields { get; set; } = new List<FieldSchema>();
    }

    public class FieldSchema
    {
        public string Name { get; set; }
        public string Type { get; set; }      // text | select | checkbox
        public string Label { get; set; }
        public bool Required { get; set; }
        public List<FieldOption> Options { get; set; } = new List<FieldOption>();
    }

    public class FieldOption
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }
}