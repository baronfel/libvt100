using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using libvt100;
using SkiaSharp;

namespace libvt100.Tests {
    [TestFixture]
    public class TestPrograms {
        [Test]
        public void TestProgram() {
            // ReadAndRenderFile("../../../simple.txt");
            // ReadAndRenderFile("../../../Program.cs.ans");
        }

        public void ReadAndRenderFile(string _filename) {
            IAnsiDecoder vt100 = new AnsiDecoder();
            //vt100.Encoding = encodingInfo.GetEncoding (); // encodingInfo.Name, new EncoderExceptionFallback(), new DecoderReplacementFallback ("U") );
            Screen screen = new Screen(80, 160);
            vt100.Subscribe(screen);

            using (BinaryReader reader = new BinaryReader(File.Open(_filename, FileMode.Open))) {
                try {
                    int read = 0;
                    while ((read = reader.Read()) != -1) {
                        vt100.Input(new byte[] { (byte)read });
                    }
                }
                catch (EndOfStreamException) {
                }
            }
            System.Console.Write(screen.ToString());
            var bitmap = screen.ToBitmap(SKTypeface.FromFamilyName("Courier New", SKFontStyle.Normal));
            using var fileStream = new FileStream(_filename + ".png", FileMode.OpenOrCreate);
            bitmap.Encode(fileStream, SKEncodedImageFormat.Png, 100);
        }
    }
}
