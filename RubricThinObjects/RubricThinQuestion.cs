using TqiiLanguageTest.Models;

namespace TqiiLanguageTest.RubricThinObjects {

    public class RubricThinQuestion {
        public List<RubricThinAnswer> Answers { get; set; } = default!;
        public string Description { get; set; } = "";
        public string HtmlName { get; set; } = "";
        public string Title { get; set; } = "";
        public double Weight { get; set; }
        public string WeightId { get; set; } = "";

        public static List<RubricThinQuestion> GenerateFromDatabase(List<RaterScale> raterScales, string scoreText) {
            var returnValue = raterScales.Where(rs => rs.QuestionInformationId == null).OrderBy(rs => rs.Order).ThenBy(rs => rs.Title).Select((rs, i) => new RubricThinQuestion {
                Description = rs.Description,
                Title = rs.Title,
                Weight = rs.Weight,
                HtmlName = "raterScale_" + i,
                WeightId = "raterScaleWeight_" + i,
                Answers = raterScales.Where(rsa => rsa.QuestionInformationId == rs.Id).OrderByDescending(rs => rs.Value).Select(rsa => new RubricThinAnswer {
                    Description = rsa.Description,
                    Title = rsa.Title,
                    IsSelected = false,
                    Value = rsa.Value,
                    Descriptors = rsa.Descriptors.Split('\n').Select(s => s.Trim()).ToList(),
                    Id = "raterScale_" + i + "_answer_" + rsa.Value
                }).ToList()
            }).ToList();

            if (!string.IsNullOrWhiteSpace(scoreText)) {
                foreach (var item in scoreText.Split(';').Select(a => a.Trim())) {
                    if (!string.IsNullOrWhiteSpace(item)) {
                        var parts = item.Split(':').Select(s => s.Trim()).ToList();
                        returnValue[int.Parse(parts[0])].Answers.Single(a => a.Value == int.Parse(parts[1])).IsSelected = true;
                    }
                }
            }
            return returnValue;
        }
    }
}