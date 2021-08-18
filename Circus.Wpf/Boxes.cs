#pragma warning disable IDE0002

namespace Circus.Wpf {
    public sealed class Boxes {
        public static object False => false;
        public static object Null => null;
        public static object True => true;
        public static object Box(bool value) {
            return value ? Boxes.True : Boxes.False;
        }
    }
}
