using System.IO.Compression;
using TqiiLanguageTest.Data;

namespace TqiiLanguageTest.FileManager {

    public class PackageHelper {
        private readonly LanguageDbContext _context;

        public PackageHelper(LanguageDbContext context) {
            _context = context;
        }

        public MemoryStream GetRecordings(int testUserId) {
            using var ms = new MemoryStream();
            using var zip = new ZipArchive(ms, ZipArchiveMode.Create, true);
            _context?.Answers?.Where(a => a.TestUserId == testUserId).OrderBy(a => a.DateTimeStart).ToList().ForEach(file => {
                if (file.Recording.Count() > 0) {
                    var entry = zip.CreateEntry("recording_" + file.Id);
                    using var fileStream = new MemoryStream(file.Recording);
                    using var entryStream = entry.Open();
                    fileStream.CopyTo(entryStream);
                }
            });
            return ms;
        }
    }
}