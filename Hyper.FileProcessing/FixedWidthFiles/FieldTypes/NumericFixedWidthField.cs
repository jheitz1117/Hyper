namespace Hyper.FileProcessing.FixedWidthFiles
{
    public abstract class NumericFixedWidthField : FixedWidthFieldBase
    {
        public override FixedWidthJustifyType JustifyType => FixedWidthJustifyType.Right;

        public override char PaddingChar => '0';
    }
}
