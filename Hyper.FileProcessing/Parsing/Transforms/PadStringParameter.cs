﻿using System;

namespace Hyper.FileProcessing.Parsing.Transforms
{
    public class PadStringParameter {
        #region Properties

        public char PadChar {
            get;
            set;
        }

        private int _maxLength = 0;
        public int MaxLength {
            get {
                return _maxLength;
            }
            set {
                if (value < 0) {
                    throw new IndexOutOfRangeException("MaxLength cannot be less than zero.");
                }

                _maxLength = value;
            }
        }

        #endregion Properties

        #region Public Methods

        public PadStringParameter() {
        }

        public PadStringParameter(char padChar, int maxLength)
            : this() {
            this.PadChar = padChar;
            this.MaxLength = maxLength;
        }

        #endregion Public Methods
    }
}
