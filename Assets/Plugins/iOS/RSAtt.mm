// PM_101: the App Tracking Transparency question and the IDFA for the AdMob adapter (ReverseSolver.Platform.AdMob):
// asked after Google's consent form (UMP), as Google recommends. Status: 0 not determined, 1 restricted, 2 denied,
// 3 authorized (iOS under 14: authorized). IosBuild links AppTrackingTransparency and AdSupport.
#import <Foundation/Foundation.h>
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <AdSupport/AdSupport.h>

static volatile int rsAttAnswered = 0;

extern "C" {
    int RS_AttStatus() {
        if (@available(iOS 14, *)) return (int)[ATTrackingManager trackingAuthorizationStatus];
        return 3;
    }

    void RS_AttRequest() {
        rsAttAnswered = 0;
        if (@available(iOS 14, *)) {
            dispatch_async(dispatch_get_main_queue(), ^{
                [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
                    rsAttAnswered = 1;
                }];
            });
        } else rsAttAnswered = 1;
    }

    int RS_AttAnswered() { return rsAttAnswered; }

    // the string is freed by Unity's marshaller
    const char* RS_Idfa() {
        NSString* s = [[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString];
        const char* u = [s UTF8String];
        char* out = (char*)malloc(strlen(u) + 1);
        strcpy(out, u);
        return out;
    }
}
