// PM_MESAJ_19/45: Haptics.cs -> 0 light (piece out), 1 heavy (bomb), 2 success (win), 3 selection (drag start, buttons),
// 4 medium (jam). The generators are kept and re-prepared so the tap has no start-up delay.
#import <UIKit/UIKit.h>

static UIImpactFeedbackGenerator *rsLight, *rsMedium, *rsHeavy;
static UINotificationFeedbackGenerator *rsNotify;
static UISelectionFeedbackGenerator *rsSelect;

extern "C" void RS_Haptic(int kind)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        if (rsLight == nil)
        {
            rsLight = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
            rsMedium = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
            rsHeavy = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleHeavy];
            rsNotify = [[UINotificationFeedbackGenerator alloc] init];
            rsSelect = [[UISelectionFeedbackGenerator alloc] init];
        }
        switch (kind)
        {
            case 1: [rsHeavy impactOccurred]; [rsHeavy prepare]; break;
            case 2: [rsNotify notificationOccurred:UINotificationFeedbackTypeSuccess]; [rsNotify prepare]; break;
            case 3: [rsSelect selectionChanged]; [rsSelect prepare]; break;
            case 4: [rsMedium impactOccurred]; [rsMedium prepare]; break;
            default: [rsLight impactOccurred]; [rsLight prepare]; break;
        }
    });
}
