using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CMSXtream.Control
{
    public class MaskedTextBox : TextBox
    {
        public TextBoxMask Mask { get; set; }

        public MaskedTextBox()
        {
            this.TextChanged += new TextChangedEventHandler(MaskedTextBox_TextChanged);
        }

        void MaskedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.CaretIndex = this.Text.Length;
            var tbEntry = sender as MaskedTextBox;

            if (tbEntry.Text.Trim() == string.Empty)
            {
                tbEntry.Text = "0";
            }

            if (tbEntry != null && tbEntry.Text.Length > 0)
            {
                tbEntry.Text = formatNumber(tbEntry.Text, tbEntry.Mask);
            }
        }

        public static string formatNumber(string MaskedNum, TextBoxMask phoneFormat)
        {
            int x;
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();

            if (MaskedNum != null)
            {
                for (int i = 0; i < MaskedNum.Length; i++)
                {
                    if (int.TryParse(MaskedNum.Substring(i, 1), out x))
                    {
                        sb.Append(x.ToString());
                    }
                }
                switch (phoneFormat)
                {
                    //case TextBoxMask.Phone7Digit:
                    //    return FormatFor7DigitPhone(sb.ToString());

                    //case TextBoxMask.Phone7DigitWithExt:
                    //    return FormatFor7DigitPhoneWithExt(sb.ToString());

                    case TextBoxMask.Phone10Digit:
                        return FormatFor10DigitPhone(sb.ToString());

                    //case TextBoxMask.Phone10DigitWithExt:
                    //    return FormatFor10DigitPhoneWithExt(sb.ToString());

                    //case TextBoxMask.Phone11Digit:
                    //    return FormatFor11DigitPhone(sb.ToString());

                    //case TextBoxMask.Phone11DigitWithExt:
                    //    return FormatFor11DigitPhoneWithExt(sb.ToString());

                    //case TextBoxMask.SSN:
                    //    return FormatForSSN(sb.ToString());

                    default:
                        break;
                }

            }
            return sb.ToString();
        }

        public static string FormatFor10DigitPhone(string sb)
        {
            StringBuilder sb2 = new StringBuilder();

            if (sb.Length > 0) sb2.Append("(0)-");

            if (sb.Length > 1) sb2.Append(sb.Substring(1, 1));
            if (sb.Length > 2) sb2.Append(sb.Substring(2, 1));
            if (sb.Length > 3) sb2.Append(sb.Substring(3, 1));

            if (sb.Length > 4) sb2.Append("-");
            if (sb.Length > 4) sb2.Append(sb.Substring(4, 1));
            if (sb.Length > 5) sb2.Append(sb.Substring(5, 1));
            if (sb.Length > 6) sb2.Append(sb.Substring(6, 1));

            if (sb.Length > 7) sb2.Append("-");
            if (sb.Length > 7) sb2.Append(sb.Substring(7, 1));
            if (sb.Length > 8) sb2.Append(sb.Substring(8, 1));
            if (sb.Length > 9) sb2.Append(sb.Substring(9, 1));

            return sb2.ToString();
        }
    }
    public enum TextBoxMask
    {
        Phone10Digit
    }
}
