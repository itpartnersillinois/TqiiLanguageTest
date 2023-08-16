using System.Text;

namespace TqiiLanguageTest.BusinessLogic {

    public static class InteractiveReadingParser {

        public static string ConverToHtml(string s, IEnumerable<string> options) {
            var returnValue = new StringBuilder();
            var isInInputString = false;
            var inputInStringCount = 0;
            var inputCount = 0;
            foreach (var c in s) {
                if (c == '\r') {
                    returnValue.Append("<br>");
                } else if (c == '^') {
                    inputCount++;
                    returnValue.Append($"<span><select id='info_select_{inputCount}'><option value=''></option>");
                    foreach (var option in options) {
                        returnValue.Append($"<option value='{option}'>{option}</option>");
                    }
                    returnValue.Append("</select></span>");
                } else if (c == '*') {
                    if (!isInInputString) {
                        returnValue.Append($"<span>");
                        inputCount++;
                        isInInputString = true;
                    }
                    inputInStringCount++;
                    returnValue.Append($"<input type='text' id='info_{inputCount}_{inputInStringCount}'>");
                } else if (c != '\n') {
                    if (isInInputString) {
                        returnValue.Append($"</span>");
                        isInInputString = false;
                        inputInStringCount = 0;
                    }
                    returnValue.Append(c);
                }
            }
            return returnValue.ToString();
        }
    }
}