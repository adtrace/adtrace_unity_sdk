#import <objc/runtime.h>
#import "AdTrace.h"
#import "AdTraceUnityAppDelegate.h"

typedef BOOL (*openURL_t)(id, SEL, UIApplication *, NSURL *, NSDictionary *);
typedef BOOL (*continueUserActivity_t)(id, SEL, UIApplication *, NSUserActivity *, void (^)(NSArray *restorableObjects));
static openURL_t original_openURL = NULL;
static continueUserActivity_t original_continueUserActivity = NULL;

@implementation AdTraceUnityAppDelegate

+ (void)swizzleAppDelegateCallbacks {
    NSLog(@"[AdTrace] Swizzling AppDelegate callbacks...");
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        original_openURL = (openURL_t)[self swizzleOriginalSelector:@selector(application:openURL:options:)
                                                       withSelector:@selector(adtrace_application:openURL:options:)];
        original_continueUserActivity = (continueUserActivity_t)[self swizzleOriginalSelector:@selector(application:continueUserActivity:restorationHandler:)
                                                                                 withSelector:@selector(adtrace_application:continueUserActivity:restorationHandler:)];
    });
}

+ (IMP)swizzleOriginalSelector:(SEL)originalSelector
                  withSelector:(SEL)swizzledSelector {
    // The Unity base app controller class name.
    extern const char* AppControllerClassName;
    Class originalClass = NSClassFromString([NSString stringWithUTF8String:AppControllerClassName]);
    Class swizzledClass = [self class];
    IMP originalImp = NULL;
    IMP swizzledImp = class_getMethodImplementation(swizzledClass, swizzledSelector);

    // Replace original implementation by the custom one.
    Method originalMethod = class_getInstanceMethod(originalClass, originalSelector);
    if (originalMethod) {
        originalImp = method_setImplementation(originalMethod, swizzledImp);
    } else if (![originalClass instancesRespondToSelector:originalSelector]) {
        // Add the method to the original class if it doesn't implement the selector.
        Method swizzledMethod = class_getInstanceMethod(swizzledClass, swizzledSelector);
        BOOL methodAdded = class_addMethod(originalClass, originalSelector, swizzledImp, method_getTypeEncoding(swizzledMethod));
        if (!methodAdded) {
            NSLog(@"[AdTrace] Cannot swizzle selector '%@' of class '%@'.", NSStringFromSelector(originalSelector), originalClass);
            return NULL;
        }
    }
    NSLog(@"[AdTrace] Selector '%@' of class '%@' is swizzled.", NSStringFromSelector(originalSelector), originalClass);
    return originalImp;
}

- (BOOL)adtrace_application:(UIApplication *)application
                   openURL:(NSURL *)url
                   options:(NSDictionary *)options {
    [AdTrace appWillOpenUrl:url];
    return original_openURL ? original_openURL(self, _cmd, application, url, options) : YES;
}

- (BOOL)adtrace_application:(UIApplication *)application
      continueUserActivity:(NSUserActivity *)userActivity
        restorationHandler:(void (^)(NSArray *restorableObjects))restorationHandler {
    if ([[userActivity activityType] isEqualToString:NSUserActivityTypeBrowsingWeb]) {
        NSURL *url = [userActivity webpageURL];
        [AdTrace appWillOpenUrl:url];
    }
    return original_continueUserActivity ? original_continueUserActivity(self, _cmd, application, userActivity, restorationHandler) : YES;
}

@end