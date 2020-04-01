using System;
using System.Collections.Generic;
using System.Text;
using Plugin.LatestVersion;
using Xamarin.Forms;
using System.Diagnostics;
using System.Globalization;

namespace DinoLingo
{
    public static class NewVersionWidget
    {
        static TimeSpan TIME_TO_CHECK_NEW_VERSION = TimeSpan.FromDays(1);
        static TimeSpan TIME_TO_CHECK_NEW_VERSION_DONT_SHOW = TimeSpan.FromDays(30);


        public static async void CheckNewVersion(Page page)
        {
            Debug.WriteLine($"NewVersionWidget -> ");
            // check date
            bool NeedToCheckNewVersion = true;
            if (await CacheHelper.Exists(CacheHelper.DATE_TO_CHECK_NEW_VERSION))
            {
                string DateToCheck = (await CacheHelper.GetAsync(CacheHelper.DATE_TO_CHECK_NEW_VERSION)).Data;
                DateTime DateTimeToCheck = DateTime.ParseExact(DateToCheck, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                Debug.WriteLine($"NewVersionWidget -> DateTime.Now = {DateTime.Now}, DateTimeToCheck = {DateTimeToCheck}");
                if (DateTimeToCheck.CompareTo(DateTime.Now) > 0)
                {
                    NeedToCheckNewVersion = false;                   
                }                
            }
            Debug.WriteLine("NewVersionWidget -> NeedToCheckNewVersion = " + NeedToCheckNewVersion);

            if (NeedToCheckNewVersion)
            {
                try {
                    var isLatest = await CrossLatestVersion.Current.IsUsingLatestVersion();
                    string latest = await CrossLatestVersion.Current.GetLatestVersionNumber();
                    Debug.WriteLine("NewVersionWidget -> latest = " + latest);

                    if (!isLatest)
                    {
                        string result = await page.DisplayActionSheet(Translate.GetString("new_version_text"), RateWidget.NO_THANKS, null, Translate.GetString("new_version_update_now"), RateWidget.REMIND_LATER);

                        if (result == RateWidget.NO_THANKS)
                        {
                            Debug.WriteLine("NewVersionWidget -> No thanks");
                            DateTime dateToCheck = DateTime.Now + TIME_TO_CHECK_NEW_VERSION_DONT_SHOW;
                            await CacheHelper.Add(CacheHelper.DATE_TO_CHECK_NEW_VERSION, dateToCheck.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                        }
                        else if (result == Translate.GetString("new_version_update_now"))
                        {
                            Debug.WriteLine("NewVersionWidget -> OpenAppInStore");

                            // only open parent check
                            // check navigation here
                            Debug.WriteLine("NewVersionWidget -> page.Navigation.ModalStack.Count = " + page.Navigation.ModalStack.Count);
                            bool isParentChecked = false;
                            if (page.Navigation.ModalStack.Count > 0)
                            {
                                foreach (Page p in page.Navigation.ModalStack)
                                {
                                    Debug.WriteLine("NewVersionWidget -> page.Navigation.ModalStack.GetType()= " + page.Navigation.ModalStack.GetType());
                                    if (p.GetType() == typeof(ParentMenu_Page))
                                    {
                                        isParentChecked = true;
                                        break;
                                    }
                                }
                            }
                            Debug.WriteLine("NewVersionWidget -> isParentChecked = " + isParentChecked);

                            if (isParentChecked)
                            {
                                if (Device.RuntimePlatform == Device.iOS) Device.OpenUri(new System.Uri(RateWidget.RATE_LINK_IOS));
                                else if (Device.RuntimePlatform == Device.Android) Device.OpenUri(new System.Uri(RateWidget.RATE_LINK_ANDROID));
                            }
                            else
                            {
                                await page.Navigation.PushModalAsync(new ParentCheck_Page(ParentCheck_Page.WhatToDoNext.UPDATE_VERSION));
                            }

                            //await CrossLatestVersion.Current.OpenAppInStore();
                        }
                        else if (result == RateWidget.REMIND_LATER)
                        {
                            Debug.WriteLine("NewVersionWidget -> Remind Later");
                            DateTime dateToCheck = DateTime.Now + TIME_TO_CHECK_NEW_VERSION;
                            await CacheHelper.Add(CacheHelper.DATE_TO_CHECK_NEW_VERSION, dateToCheck.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                        }
                    }
                    else
                    {
                        Debug.WriteLine("NewVersionWidget -> It's the latest version!");
                    }
                }

                catch (Exception ex)
                {
                    Debug.WriteLine("NewVersionWidget -> Caught exception - ex.message:" + ex.Message);
                }
                
            }
        }
    }
}
