// PM_MESAJ_19 §6: Haptics.cs -> 0 light impact (jam), 1 heavy impact (bomb), 2 success notification (win).
#import <UIKit/UIKit.h>

extern "C" void RS_Haptic(int kind)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        if (kind == 2)
        {
            UINotificationFeedbackGenerator *g = [[UINotificationFeedbackGenerator alloc] init];
            [g prepare];
            [g notificationOccurred:UINotificationFeedbackTypeSuccess];
            return;
        }
        UIImpactFeedbackGenerator *g = [[UIImpactFeedbackGenerator alloc] initWithStyle:
            kind == 1 ? UIImpactFeedbackStyleHeavy : UIImpactFeedbackStyleLight];
        [g prepare];
        [g impactOccurred];
    });
}
