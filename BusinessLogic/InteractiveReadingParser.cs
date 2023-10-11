using System.Text;

namespace TqiiLanguageTest.BusinessLogic {

    public static class InteractiveReadingParser {

        public static string ConverToHtml(string s, IEnumerable<string> options) {
            var returnValue = new StringBuilder();
            var paragraph = new StringBuilder();
            var isInInputString = false;
            var isEmphasized = false;
            var inputInStringCount = 0;
            var inputCount = 0;
            var hasInputValues = false;
            var noInputValues = !s.Contains('*') && !s.Contains('^');
            foreach (var c in s) {
                if (c == '\r') {
                    if (hasInputValues || noInputValues) {
                        returnValue.Append(paragraph.ToString());
                    } else {
                        returnValue.Append("<span class='faded'>");
                        returnValue.Append(paragraph.ToString());
                        returnValue.Append("</span>");
                    }
                    returnValue.Append("<br>");
                    paragraph.Clear();
                    hasInputValues = false;
                } else if (c == '^') {
                    hasInputValues = true;
                    inputCount++;
                    paragraph.Append($"<span class='question'><select id='info_select_{inputCount}'><option value=''></option>");
                    foreach (var option in options) {
                        paragraph.Append($"<option value='{option}'>{option}</option>");
                    }
                    paragraph.Append("</select></span>");
                } else if (c == '*') {
                    hasInputValues = true;
                    if (!isInInputString) {
                        paragraph.Append($"<span class='question'>");
                        inputCount++;
                        isInInputString = true;
                    }
                    inputInStringCount++;
                    paragraph.Append($"<input type='text' id='info_{inputCount}_{inputInStringCount}'>");
                } else if (c == '_') {
                    if (isEmphasized) {
                        isEmphasized = false;
                        paragraph.Append($"</em>");
                    } else {
                        isEmphasized = true;
                        paragraph.Append("<em>");
                    }
                } else if (c == '[') {
                    paragraph.Append("<span class='nowrap'>");
                } else if (c == ']') { // note that this may close the 'question' span; that's OK
                    paragraph.Append("</span>");
                } else if (c != '\n') {
                    if (isInInputString) {
                        paragraph.Append($"</span>");
                        isInInputString = false;
                        inputInStringCount = 0;
                    }
                    paragraph.Append(c);
                }
            }
            returnValue.Append(paragraph.ToString());
            return returnValue.ToString();
        }
    }
}