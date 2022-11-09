using System.Windows;

namespace WpfUtil
{
    public static class Util
    {
        /// <summary>
        /// Setup Window properties MinWidth and MinHeight based on current Width and Height.
        /// 
        /// A scaling factor is applied to Width and Height in order to reduce (assumed) visually
        /// dense packing of SizeToContent.
        /// 
        /// Intended use: To be applied, for instance, after SizeToContent, e.g., as handler
        /// for a Window's Loaded event.
        /// </summary>
        /// <param name="sender">A Window, which should be configured. No effect for other types.</param>
        /// <param name="e">(not used)</param>
        public static void Setup_MinWidth_MinHeight(object sender, RoutedEventArgs e)
        {
            if (sender is Window)
            {
                // Relax layout packing calculated by SizeToContent 
                double scale = 1.2;

                ((Window)sender).MinWidth = ((Window)sender).Width * scale;
                ((Window)sender).MinHeight = ((Window)sender).Height * scale;
            }
        }
    }
}