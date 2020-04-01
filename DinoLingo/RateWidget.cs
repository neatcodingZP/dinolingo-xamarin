using Plugin.DeviceInfo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace DinoLingo
{
    public static class RateWidget
    {
        static int START_OPEN_PARENT_MENU_TO_SHOW_POP_UP = 2;
        static int OPEN_PARENT_MENU_TO_SHOW_POP_UP = START_OPEN_PARENT_MENU_TO_SHOW_POP_UP;
        static int OPEN_PARENT_MENU_INCREMENT = 2;

        public static string HEADER_IOS;// = "Enjoying DinoLingo? \nRate it on the App Store!";
        public static string HEADER_ANDROID;// = "Enjoying DinoLingo? \nRate it on the Google Play!";

        public static string RATE_DINOLINGO;// = "Rate DinoLingo";
        public static string REMIND_LATER;// = "Remind me later";
        public static string NO_THANKS;// = "No, Thanks";

        public static string RATE_LINK_IOS = "https://itunes.apple.com/us/app/dinolingo/id1444105210?mt=8";
                                              
        public static string RATE_LINK_ANDROID = "https://play.google.com/store/apps/details?id=com.dinolingo.DinoLingo";
                                                 

        
        public static bool IsFreshIOS = false;

        public static RateState State;

        public class RateState
        {
            public int openedParentMenu { get; set; }
            public bool remindLater { get; set; } 
            public int justPurchased { get; set; } 
        }

        public static void SetStrings()
        {
            HEADER_IOS = Translate.GetString("rate_enjoy_dinolingo_ios");
            HEADER_ANDROID = Translate.GetString("rate_enjoy_dinolingo_android");
            RATE_DINOLINGO = Translate.GetString("rate_rate_dinolingo");
            REMIND_LATER = Translate.GetString("rate_remind_later");
            NO_THANKS = Translate.GetString("rate_no_thanks");
        }

        /*
        public static Task CheckRateWidget(INavigation navigation, Page page)
        {
            return Task.Run(async ()=> {
                if (State.remindLater)
                {
                    State.openedParentMenu++;
                    Debug.WriteLine("RateWidget -> State.openedParentMenu = " + State.openedParentMenu);
                    if (State.openedParentMenu >= OPEN_PARENT_MENU_TO_SHOW_POP_UP)
                    { // show pop-up here
                        //*** if (IsFreshIOS) // we have fresh iOs - try to 
                        if (false)
                        {
                            Debug.WriteLine("RateWidget -> DependencyService.Get<IRequestReview>().RequestReview();");
                            DependencyService.Get<IRequestReview>().RequestReview();
                            //State.remindLater = false;
                            //await CacheHelper.Add<RateWidget.RateState>(CacheHelper.RATE_STATE, State);
                        }
                        else
                        {
                            Debug.WriteLine("RateWidget -> !IsFreshIOS");
                            string header = (Device.RuntimePlatform == Device.iOS) ? HEADER_IOS : HEADER_ANDROID;
                            string result = await page.DisplayActionSheet(header, NO_THANKS, null, RATE_DINOLINGO, REMIND_LATER);
                            Debug.WriteLine("RateWidget -> result = " + result);
                            // process the result
                            if (result == NO_THANKS)
                            {
                                Debug.WriteLine("RateWidget -> No thanks");
                                RateWidget.State.remindLater = false;
                            }
                            else if (result == RATE_DINOLINGO)
                            {
                                Debug.WriteLine("RateWidget -> rate dinolingo");
                                State.remindLater = false;
                                await CacheHelper.Add<RateWidget.RateState>(CacheHelper.RATE_STATE, State);
                                await navigation.PopModalAsync();
                                if (Device.RuntimePlatform == Device.iOS) Device.OpenUri(new System.Uri(RATE_LINK_IOS));
                                else if (Device.RuntimePlatform == Device.Android) Device.OpenUri(new System.Uri(RATE_LINK_ANDROID));

                                return;
                            }
                            else if (result == REMIND_LATER)
                            {
                                Debug.WriteLine("RateWidget -> Remind me later");
                                // do nothing here
                            }
                            
                        }
                        // cache the widget
                        await CacheHelper.Add<RateWidget.RateState>(CacheHelper.RATE_STATE, State);
                    }                    
                }
            });
        }
        */

        public static async void CheckRateWidget(INavigation navigation, Page page)
        {
            
               if (State.remindLater)
               {
                    State.openedParentMenu++;
                    Debug.WriteLine("RateWidget -> State.openedParentMenu = " + State.openedParentMenu);
                    if (State.openedParentMenu >= OPEN_PARENT_MENU_TO_SHOW_POP_UP)
                    { // show pop-up here
                        OPEN_PARENT_MENU_TO_SHOW_POP_UP += OPEN_PARENT_MENU_INCREMENT;
                        State.justPurchased = 0;
                        if (IsFreshIOS) // we have fresh iOs - try to                         
                        {
                            Debug.WriteLine("RateWidget -> DependencyService.Get<IRequestReview>().RequestReview();");
                            DependencyService.Get<IRequestReview>().RequestReview();
                            //State.remindLater = false;
                            //await CacheHelper.Add<RateWidget.RateState>(CacheHelper.RATE_STATE, State);
                        }
                        else
                        {
                            Debug.WriteLine("RateWidget -> !IsFreshIOS");
                            string header = (Device.RuntimePlatform == Device.iOS) ? HEADER_IOS : HEADER_ANDROID;
                            string result = await page.DisplayActionSheet(header, NO_THANKS, null, RATE_DINOLINGO, REMIND_LATER);
                            Debug.WriteLine("RateWidget -> result = " + result);
                            // process the result
                            if (result == NO_THANKS)
                            {
                                Debug.WriteLine("RateWidget -> No thanks");
                                State.remindLater = false;
                                
                            }
                            else if (result == RATE_DINOLINGO)
                            {
                                Debug.WriteLine("RateWidget -> rate dinolingo");
                                State.remindLater = false;
                                

                                await CacheHelper.Add<RateWidget.RateState>(CacheHelper.RATE_STATE, State);
                                await navigation.PopModalAsync();

                                if (Device.RuntimePlatform == Device.iOS) Device.OpenUri(new System.Uri(RATE_LINK_IOS));
                                else if (Device.RuntimePlatform == Device.Android) Device.OpenUri(new System.Uri(RATE_LINK_ANDROID));

                                return;
                            }
                            else if (result == REMIND_LATER)
                            {
                                
                                Debug.WriteLine("RateWidget -> Remind me later");
                                // do nothing here
                            }
                            
                        }

                        // cache the widget
                        await CacheHelper.Add<RateWidget.RateState>(CacheHelper.RATE_STATE, State);
                    }
                }
            
        }
    }
}
