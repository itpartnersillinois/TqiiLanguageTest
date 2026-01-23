using System.Text.RegularExpressions;
using TqiiLanguageTest.Data;
using TqiiLanguageTest.ModelsRegistration;

namespace TqiiLanguageTest.BusinessLogic {

    public class InstructionHelper {
        private readonly RegistrationDbContext _context;

        public InstructionHelper(RegistrationDbContext context) {
            _context = context;
        }

        public RegistrationInstruction GetInstruction(InstructionType id) => _context.Instructions?.SingleOrDefault(i => i.TypeOfInstruction == id) ?? new RegistrationInstruction { TypeOfInstruction = id, Description = id.ToString() };

        public string GetInstructionString(InstructionType id) {
            var returnValue = GetInstruction(id)?.InstructionText ?? "";
            returnValue = Regex.Replace(returnValue, @"(https?://[^\s]+)", "<a href=\"$1\">$1</a>", RegexOptions.IgnoreCase);
            return string.IsNullOrWhiteSpace(returnValue) ? "" : "<p>" + returnValue.Replace("\r", "</p><p>").Replace("\n", "") + "</p>";
        }

        public async Task<int> Save(RegistrationInstruction instruction) {
            instruction.InstructionText ??= "";
            instruction.Description ??= "";
            if (instruction.Id == 0) {
                var oldInstruction = _context.Instructions?.SingleOrDefault(i => i.TypeOfInstruction == instruction.TypeOfInstruction);
                if (oldInstruction != null) {
                    _context.Remove(oldInstruction);
                }
                _ = _context.Add(instruction);
            } else {
                _context.Instructions?.Update(instruction);
            }
            return await _context.SaveChangesAsync();
        }
    }
}