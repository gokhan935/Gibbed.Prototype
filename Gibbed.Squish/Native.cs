/* Copyright (c) 2017 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Runtime.InteropServices;

namespace Gibbed.Squish
{
    public sealed class Native
    {
        public enum Flags
        {
            // ReSharper disable InconsistentNaming
            None = 0,

            /// <summary>
            /// Use DXT1 compression.
            /// </summary>
            DXT1 = 1 << 0,

            /// <summary>
            /// Use DXT3 compression.
            /// </summary>
            DXT3 = 1 << 1,

            /// <summary>
            /// Use DXT5 compression.
            /// </summary>
            DXT5 = 1 << 2,

            /// <summary>
            /// Use BC4 compression.
            /// </summary>
            BC4 = 1 << 3,

            /// <summary>
            /// Use BC5 compression.
            /// </summary>
            BC5 = 1 << 4,

            /// <summary>
            /// Use a slow but high quality colour compressor (the default).
            /// </summary>
            ColourClusterFit = 1 << 5,

            /// <summary>
            /// Use a fast but low quality colour compressor.
            /// </summary>
            ColourRangeFit = 1 << 6,

            /// <summary>
            /// Weight the colour by alpha during cluster fit (disabled by default).
            /// </summary>
            WeightColourByAlpha = 1 << 7,

            /// <summary>
            /// Use a very slow but very high quality colour compressor.
            /// </summary>
            ColourIterativeClusterFit = 1 << 8,

            /// <summary>
            /// Source is BGRA rather than RGBA.
            /// </summary>
            SourceBGRA = 1 << 9,
            // ReSharper restore InconsistentNaming
        }

        private static bool Is64Bit()
        {
            return Marshal.SizeOf(IntPtr.Zero) == 8;
        }

        private sealed class Native32
        {
            public const string DllName = "squish32";

            [DllImport(DllName, EntryPoint = "CompressImage", CallingConvention = CallingConvention.Cdecl)]
            internal static extern void CompressImage([MarshalAs(UnmanagedType.LPArray)] byte[] rgba,
                                                      int width,
                                                      int height,
                                                      [MarshalAs(UnmanagedType.LPArray)] byte[] blocks,
                                                      int flags,
                                                      [MarshalAs(UnmanagedType.LPArray)] float[] metric);

            [DllImport(DllName, EntryPoint = "DecompressImage", CallingConvention = CallingConvention.Cdecl)]
            internal static extern void DecompressImage([MarshalAs(UnmanagedType.LPArray)] byte[] rgba,
                                                        int width,
                                                        int height,
                                                        [MarshalAs(UnmanagedType.LPArray)] byte[] blocks,
                                                        int flags);
        }

        private sealed class Native64
        {
            public const string DllName = "squish64";

            [DllImport(DllName, EntryPoint = "CompressImage", CallingConvention = CallingConvention.Cdecl)]
            internal static extern void CompressImage([MarshalAs(UnmanagedType.LPArray)] byte[] rgba,
                                                      int width,
                                                      int height,
                                                      [MarshalAs(UnmanagedType.LPArray)] byte[] blocks,
                                                      int flags,
                                                      [MarshalAs(UnmanagedType.LPArray)] float[] metric);

            [DllImport(DllName, EntryPoint = "DecompressImage", CallingConvention = CallingConvention.Cdecl)]
            internal static extern void DecompressImage([MarshalAs(UnmanagedType.LPArray)] byte[] rgba,
                                                        int width,
                                                        int height,
                                                        [MarshalAs(UnmanagedType.LPArray)] byte[] blocks,
                                                        int flags);
        }

        private static void CallDecompressImage(byte[] rgba, int width, int height, byte[] blocks, int flags)
        {
            if (Is64Bit() == true)
            {
                Native64.DecompressImage(rgba, width, height, blocks, flags);
            }
            else
            {
                Native32.DecompressImage(rgba, width, height, blocks, flags);
            }
        }

        public static byte[] DecompressImage(byte[] blocks, int width, int height, Flags flags)
        {
            var pixelOutput = new byte[width * height * 4];
            CallDecompressImage(pixelOutput, width, height, blocks, (int)flags);
            return pixelOutput;
        }
    }
}
