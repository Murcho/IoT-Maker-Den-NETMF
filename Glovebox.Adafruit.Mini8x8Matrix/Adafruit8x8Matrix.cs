﻿using Glovebox.Adafruit.Mini8x8Matrix.Driver;
using System;
using System.Threading;

namespace Glovebox.Adafruit.Mini8x8Matrix {
    public class Adafruit8x8Matrix : IDisposable {

        const uint bufferSize = 17;
        private byte[] Frame = new byte[bufferSize];

        protected const ushort Rows = 8;
        protected const ushort Columns = 8;
        public readonly ushort Panels = 1;
        protected ushort Length = Rows * Columns;

        Ht16K33I2cConnection i2Cdriver;

        public enum BlinkRate : byte {
            Off = 0x00,
            Fast = 0x02, //2hz
            Medium = 0x04, //1hz
            Slow = 0x06, //0.5 hz
        }

        #region Font
        public ulong[] fontSimple = new ulong[] { 
			0x0000000000000000, // space
			0x0008000808080808, // !            
			0x0000000000001414, // "
			0x0014143E143E1414, // #
			0x00143E543E153E14, // $
			0x0032220408102226, // %  
			0x002C122C0C12120C, // &
			0x0000000000000808, // '            
			0x0008040202020408, // (
			0x0002040808080402, // )
			0x00412A142A142A41, // *
			0x000808083E080808, // +
			0x0408080000000000, // ,
			0x000000003E000000, // -
			0x0002000000000000, // .
			0x0001020408102040, // / 
			0x001c22262a32221c, //0
			0x003e080808080a0c, //1
			0x003e04081020221c, //2
			0x001c22201820221c, //3
			0x0008083e0a020202, //4
			0x001c2220201e023e, //5
			0x001c22221e02023c, //6
			0x000202040810203e, //7
			0x001c22221c22221c, //8
			0x002020203c22221c, //9
			0x00000C0C000C0C00, // :
			0x0004080800080800, // ;
			0x0010080402040810, // <
			0x0000003E003E0000, // =
			0x0002040810080402, // >
			0x000808081020221C, // ?
			0x001C02122A32221C, // @
			0x002222223e22221c, //A
			0x001e22221e22221e, //B
			0x001c22020202221c, //C
			0x001e22222222221e, //D
			0x003e02021e02023e, //E
			0x000202021e02023e, //F
			0x001c22223a02221c, //G
			0x002222223e222222, //H
			0x003e08080808083e, //I
			0x000608080808083e, //J
			0x0022120a060a1222, //K
			0x003e020202020202, //L
			0x00222222222a3622, //M
			0x002222322a262222, //N
			0x001c22222222221c, //O
			0x000202021e22221e, //P
			0x002c122a2222221c, //Q
			0x0022120a1e22221e, //R
			0x001c22100804221c, //S
			0x000808080808083e, //T
			0x001c222222222222, //U
			0x0008142222222222, //V
			0x0022362a22222222, //W
			0x0022221408142222, //X
			0x0008080808142222, //Y
			0x003e02040810203e, //Z
			0x000E02020202020E, //[
			0x0020201008040202, // \
			0x000E08080808080E, //]
			0x0000000000221408, // ^
			0x003E000000000000, // _
			0x0000000000001008, // '
			0x0012121E120C0000, // a
			0x000E120E120E0000, // b
			0x000C1202120C0000, // c
			0x000E1212120E0000, // d
			0x001E020E021E0000, // e
			0x0002020E021E0000, // f
			0x000C121A021C0000, // g
			0x0012121E12120000, // h
			0x001C0808081C0000, // i
			0x000C121010100000, // j
			0x00120A060A120000, // k
			0x001E020202020000, // l
			0x0022222A36220000, // m
			0x0022322A26220000, // n
			0x000C1212120C0000, // o
			0x0002020E120E0000, // p
			0x201C1212120C0000, // q
			0x0012120E120E0000, // r
			0x000E100C021C0000, // s
			0x00080808081C0000, // t
			0x000C121212120000, // u
			0x0008142222220000, // v
			0x0022362A22220000, // w
			0x0022140814220000, // x
			0x0008080814220000, // y
			0x003E0408103E0000, // z

		};

        #endregion

        #region Symbols
        public enum Symbols : ulong {
            Heart = 0x00081C3E7F7F3600,
            HappyFace = 0x3C4299A581A5423C,
            SadFace = 0x3C42A59981A5423C,
            Block = 0xffffffffffffffff,
            HourGlass = 0xFF7E3C18183C7EFF,
            UpArrow = 0x18181818FF7E3C18,
            DownArrow = 0x183C7EFF18181818,
            RightArrow = 0x103070FFFF703010,
            LeftArrow = 0x080C0EFFFF0E0C08,

        }

        #endregion

        public Adafruit8x8Matrix(Ht16K33I2cConnection i2Cdriver, ushort panels = 1) {
            this.Panels = panels;
            this.i2Cdriver = i2Cdriver;
        }

        #region Scroll string primatives

        public void ScrollStringInFromRight(string characters, int milliseconds) {
            // loop through each chacter
            for (int ch = 0; ch < characters.Length; ch++) {

                char charactor = characters.Substring(ch, 1)[0];
                if (charactor >= ' ' && charactor <= 'z') {
                    ScrollBitmapInFromRight(fontSimple[charactor - 32], milliseconds);
                }
            }
        }

        public void ScrollStringInFromLeft(string characters, int milliseconds) {

            // loop through each chacter
            for (int ch = characters.Length - 1; ch >= 0; ch--) {

                char charactor = characters.Substring(ch, 1)[0];
                if (charactor >= ' ' && charactor <= 'z') {
                    ScrollBitmapInFromLeft(fontSimple[charactor - 32], milliseconds);
                }
            }
        }

        #endregion


        #region Scroll Character primatives
        public void ScrollCharacterFromRight(char charactor, int milliseconds) {
            if (charactor >= ' ' && charactor <= 'z') {
                ScrollBitmapInFromRight(fontSimple[charactor - 32], milliseconds);
            }
        }

        public void ScrollCharacterFromLeft(char charactor, int milliseconds) {
            if (charactor >= ' ' && charactor <= 'z') {
                ScrollBitmapInFromLeft(fontSimple[charactor - 32], milliseconds);
            }
        }

        #endregion

        #region Scroll symbol primatives

        public void ScrollSymbolInFromRight(Symbols[] sym, int milliseconds) {
            foreach (var s in sym) {
                ScrollBitmapInFromRight((ulong)s, milliseconds);
            }
        }

        public void ScrollSymbolInFromLeft(Symbols[] sym, int milliseconds) {
            foreach (var s in sym) {
                ScrollBitmapInFromLeft((ulong)s, milliseconds);
            }
        }

        public void ScrollSymbolInFromRight(Symbols sym, int milliseconds) {
            ScrollBitmapInFromRight((ulong)sym, milliseconds);
        }

        public void ScrollSymbolInFromLeft(Symbols sym, int milliseconds) {
            ScrollBitmapInFromLeft((ulong)sym, milliseconds);
        }

        #endregion

        #region Scroll Bitmaps left and right

        public void ScrollBitmapInFromRight(ulong bitmap, int milliseconds) {
            ushort pos = 0;
            ulong mask;
            bool pixelFound = false;
            int panelOffset = (Panels - 1) * Columns * Rows;

            // space character ?
            if (bitmap == 0) {
                ShiftFrameLeft();
                FrameDraw();
                //      Thread.Sleep(milliseconds);
                return;
            }

            // fetch vertical slice of character font
            for (int col = 0; col < Columns; col++) {
                pixelFound = false;

                for (int row = 0; row < Rows; row++) {
                    mask = (ulong)1 << row * Columns + col;
                    pos = (ushort)(row * Columns + (Columns - 1) + panelOffset);

                    if ((bitmap & mask) != 0) {
                        FrameSet(pos, true);
                        pixelFound = true;
                    }
                }
                if (pixelFound) {
                    FrameDraw();
                    ShiftFrameLeft();
                    Thread.Sleep(milliseconds);
                }
            }
            //post bitmap space
            ShiftFrameLeft();
        }

        public void ScrollBitmapInFromLeft(ulong bitmap, int milliseconds) {
            ushort pos = 0;
            ulong mask;
            bool pixelFound = false;


            // space character ?
            if (bitmap == 0) {
                ShiftFrameRight();
                FrameDraw();
                //     Thread.Sleep(milliseconds);
                return;
            }

            // fetch vertical slice of character font
            for (int col = Columns - 1; col >= 0; col--) {
                pixelFound = false;

                for (int row = 0; row < Rows; row++) {
                    mask = (ulong)1 << row * Columns + col;
                    pos = (ushort)(row * Columns);

                    if ((bitmap & mask) != 0) {
                        FrameSet(pos, true);
                        pixelFound = true;
                    }
                }
                if (pixelFound) {
                    FrameDraw();
                    ShiftFrameRight();
                    Thread.Sleep(milliseconds);
                }
            }
            //blank character space
            ShiftFrameRight();
        }

        #endregion

        public void DrawString(string characters, int milliseconds, ushort panel = 0) {
            char c;
            for (int i = 0; i < characters.Length; i++) {
                c = characters.Substring(i, 1)[0];
                if (c >= ' ' && c <= 'z') {
                    DrawLetter(c, panel);
                    FrameDraw();
                    Thread.Sleep(milliseconds);
                }
            }
        }

        public void DrawLetter(char character, ushort panel = 0) {
            ulong letter = 0;

            if (character >= ' ' && character <= 'z') {
                //calc ascii offset
                byte charValue = (byte)(character - 32);
                letter = fontSimple[charValue];
            }
            else { return; }

            DrawBitmap(letter, panel);
        }

        public void FrameClear() {
            for (int i = 0; i < Frame.Length; i++) {
                Frame[i] = 0;
            }
        }

        public void DrawSymbol(Symbols symbol) {
            DrawBitmap((ulong)symbol);
        }

        public void DrawBitmap(ulong bitmap, ushort panel = 0) {
            FrameSet(0, (byte)(bitmap));
            FrameSet(1, (byte)(bitmap >> 8));
            FrameSet(2, (byte)(bitmap >> 16));
            FrameSet(3, (byte)(bitmap >> 24));
            FrameSet(4, (byte)(bitmap >> 32));
            FrameSet(5, (byte)(bitmap >> 40));
            FrameSet(6, (byte)(bitmap >> 48));
            FrameSet(7, (byte)(bitmap >> 56));
        }

        protected void FrameSet(ushort row, byte value) {
            row = (ushort)(row % Rows);
            Frame[row * 2 + 1] = FixBitOrder(value);
        }

        protected void FrameInvert() {
            for (int b = 1; b < bufferSize; b += 2) {
                Frame[b] = (byte)(255 - Frame[b]);
            }
        }

        // Fix bit order problem with the ht16K33 controller or Adafruit 8x8 matrix
        // Bits offset by 1, roll bits forward by 1, replace 8th bit with the 1st 
        private byte FixBitOrder(byte b) {
            return (byte)(b >> 1 | (b << 7));
        }

        protected void FrameSet(int position, bool state) {
            position = (int)(Math.Abs(position) % (Columns * Rows));

            ushort row = (ushort)(position / Columns);
            ushort bit = (ushort)(position - (row * (Columns)));

            bit += 7;
            bit %= 8;

            byte temp = Frame[row * 2 + 1];

            if (state) { temp = (byte)(temp | (1 << bit)); }
            else {
                byte t = (byte)(temp ^ (1 << (bit)));
                temp = (byte)(temp & t);
            }

            Frame[row * 2 + 1] = temp;
        }

        public void ShiftFrameRight() {
            for (ushort i = 0; i < Rows; i++) {
                ColumnShiftRight(i);
            }
        }

        public void ShiftFrameLeft() {
            for (ushort i = 0; i < Rows; i++) {
                ColumnShiftLeft(i);
            }
        }

        /// <summary>
        /// Panel aware scroll left
        /// </summary>
        /// <param name="rowIndex"></param>
        public void ColumnShiftLeft(ushort rowIndex) {
            //      Console.WriteLine(rowIndex.ToString());
            int b = rowIndex * 2 + 1;
            //       for (int b = 1; b < bufferSize; b += 2) {
            Frame[b] = (byte)((Frame[b] >> 1 | Frame[b] << 7) & 0xbf);
            // }
        }

        /// <summary>
        /// Panel aware scroll right
        /// </summary>
        /// <param name="rowIndex"></param>
        public void ColumnShiftRight(ushort rowIndex) {
            int b = rowIndex * 2 + 1;

            //    for (int b = 1; b < bufferSize; b += 2) {
            Frame[b] = (byte)((Frame[b] << 1 | Frame[b] >> 7) & 0x7f);
            //  }
        }

        public void RowRollUp() {
            byte temp = Frame[1];
            for (int b = 0; b < Rows - 1; b++) {
                Frame[(b * 2) + 1] = Frame[((b + 1) * 2) + 1];
            }
            Frame[Rows * 2 - 1] = temp;
        }

        public void RowRollDown() {
            byte temp = Frame[Rows * 2 - 1];
            for (int b = Rows; b > 1; b--) {
                Frame[(b * 2 - 1)] = Frame[((b - 1) * 2) - 1];
            }
            Frame[1] = temp;
        }

        public void FrameRollRight() {
            for (ushort row = 0; row < Rows; row++) {
                ColumnRollRight(row);
            }
        }

        public void FrameRollLeft() {
            for (ushort row = 0; row < Rows; row++) {
                ColumnRollLeft(row);
            }
        }

        public void ColumnRollRight(ushort rowIndex) {
            int b = rowIndex * 2 + 1;
            //    for (int b = 1; b < bufferSize; b += 2) {
            Frame[b] = (byte)(Frame[b] << 1 | Frame[b] >> 7);
            //     }
        }

        public void ColumnRollLeft(ushort rowIndex) {
            int b = rowIndex * 2 + 1;
            //  for (int b = 1; b < bufferSize; b += 2) {
            Frame[b] = (byte)(Frame[b] >> 1 | Frame[b] << 7);
            //  }
        }


        #region Ht16K33 I2C Control Methods

        public void FrameDraw() {
            i2Cdriver.Write(Frame);
        }

        public void FrameSetBlinkRate(BlinkRate br) {
            i2Cdriver.FrameSetBlinkRate((byte)br);
        }

        public void FrameSetBrightness(byte level) {
            i2Cdriver.FrameSetBrightness(level);
        }

        #endregion


        public void Dispose() {

        }
    }
}
