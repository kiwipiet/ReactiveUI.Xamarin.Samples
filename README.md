ReactiveUI.Xamarin.Samples
==========================

### ReactiveUI.Xamarin.StockWatch.Advandced (support for Android 2.1)
The sample is an adaption of the "MobileSample-Android" from rxUI.

Use my Branch "with_android_2.1_support" of Reactive UI because
i have unsynced changes here. 
I linked the Projects in the ReactiveUI.Xamarin.Samples-Solution so 
you will have to pull these branch too.

The sample includes the following features:

- use AppBarCombat for tab navigation
- use rxUI-Routing for navigation
- supports the back-button for navigation
- handle configuration changes e.g. change of the orientation
- reuse of existing views

#### Naming conventions

Layouts (only lowercase):
a_ layout for Activity
d_ layout for Dialog
f_ layout for Fragment
t_ layout for Toast
v_ layout for View

Ressource IDs (only lowercase):
In Layouts: [layoutname]__(two underscores)[controltype prefix e.g. txt or btn][id/name]	