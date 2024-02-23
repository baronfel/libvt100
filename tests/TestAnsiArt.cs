using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NUnit.Framework;
using libvt100;
using SkiaSharp;

namespace libvt100.Tests {
    [TestFixture]
    public class TestAnsiArt {
        [Test]
        public void TestWendy() {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ReadAndRenderFile("../../../70-twilight.ans", CodePagesEncodingProvider.Instance.GetEncoding("ibm437"), new Size(80, 80));
            ReadAndRenderFile("../../../n4-wendy.ans", CodePagesEncodingProvider.Instance.GetEncoding("ibm437"), new Size(80, 80));
            ReadAndRenderFile("../../../zv-v01d.ans", CodePagesEncodingProvider.Instance.GetEncoding("ibm437"), new Size(80, 180));
        }

        [Test]
        public void TestSimpleTxt() {
            System.Console.Write(ReadAndRenderFile("../../../simple.txt", Encoding.UTF8, new Size(50, 6)).ToString());
        }

        [Test]
        public void TestUnixProgramOutput() {
            ReadAndRenderFile("../../../mc.output", Encoding.UTF8, new Size(180, 65));
            ReadAndRenderFile("../../../ls.output", Encoding.UTF8, new Size(65, 10));
        }

        public void ReadAndRenderFileAll(string _filename, Size _size) {
            foreach (EncodingInfo encodingInfo in Encoding.GetEncodings()) {
                ReadAndRenderFile(_filename, encodingInfo.GetEncoding(), _size);
            }
        }

        public Screen ReadAndRenderFile(string _filename, Encoding _encoding, Size _size) {
            IAnsiDecoder vt100 = new AnsiDecoder();
            //vt100.Encoding = Encoding.GetEncoding ( encodingInfo.Name, new EncoderExceptionFallback(), new DecoderReplacementFallback ("U") );
            vt100.Encoding = _encoding;
            Screen screen = new Screen(_size.Width, _size.Height);
            vt100.Subscribe(screen);

            using (Stream stream = File.Open(_filename, FileMode.Open)) {
                try {
                    int read = 0;
                    while ((read = stream.ReadByte()) != -1) {
                        vt100.Input(new byte[] { (byte)read });
                    }
                }
                catch (EndOfStreamException) {
                }
            }
            //System.Console.Write ( screen.ToString() );
            var bitmap = screen.ToBitmap(SKTypeface.FromFamilyName("Courier New"));
            using var fileStream = new FileStream(Path.GetFileNameWithoutExtension(_filename) + "_" + _encoding.EncodingName + ".png", FileMode.OpenOrCreate);
            bitmap.Encode(fileStream, SKEncodedImageFormat.Png, 100);

            /*
              foreach ( Screen.Character ch in screen )
              {
              if ( ch.Char != 0x20 )
              {
              System.Console.WriteLine ( "Non-space character: {0} 0x{1:X4}", ch.Char, (int) ch.Char );
              }
              }
            */
            return screen;
        }
    }
}
