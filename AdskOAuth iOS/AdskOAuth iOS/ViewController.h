/////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Adam Nagy 2013 - ADN/Developer Technical Services
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////////////////

#import <UIKit/UIKit.h>

@interface ViewController : UIViewController <UIWebViewDelegate, UIAlertViewDelegate>

- (IBAction)LogInClick:(id)sender;
- (IBAction)RefreshClick:(id)sender;
- (IBAction)OobClick:(id)sender;

@property (weak, nonatomic) IBOutlet UIBarButtonItem *LogInButton;
@property (weak, nonatomic) IBOutlet UIBarButtonItem *RefreshButton;
@property (weak, nonatomic) IBOutlet UIBarButtonItem *OobButton;
@property (weak, nonatomic) IBOutlet UIWebView * webView;

@end
