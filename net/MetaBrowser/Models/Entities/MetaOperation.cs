using System;

namespace MetaBrowser.Models.Entities
{
    public class MetaOperation
    {
        public string Name { get; set; }
        public string InputType { get; set; }
        public string OutputType { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return String.Format("{2} {0}({1});{3}",
                Name, 
                String.IsNullOrEmpty(InputType) ? String.Empty : InputType + " value", 
                String.IsNullOrEmpty(OutputType) ? "void" : OutputType,
                String.IsNullOrEmpty(Description) ? String.Empty : " // " + Description);
        }
    }
}