using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace JaebeMusicStudio3.Front.Controls;

public class LabeledControl2: HeaderedContentControl
{
    static LabeledControl2()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(LabeledControl2),
            new FrameworkPropertyMetadata(typeof(LabeledControl2))
        );
    }
}
    