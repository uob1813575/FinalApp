﻿using System;
using System.IO;

namespace FinalApp.Commons {
    public static class ImageUtils {
        public static byte[] GetImageAsByteArray(string imageFilePath) {
            using (FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read)) {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}
