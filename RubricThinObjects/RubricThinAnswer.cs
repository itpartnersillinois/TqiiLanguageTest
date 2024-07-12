namespace TqiiLanguageTest.RubricThinObjects {

    public class RubricThinAnswer {
        public string Description { get; set; } = "";
        public List<string> Descriptors { get; set; } = default!;
        public string Id { get; set; } = "";
        public bool IsSelected { get; set; }
        public string Title { get; set; } = "";
        public int Value { get; set; }
    }
}