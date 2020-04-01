using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using Newtonsoft.Json;
using Plugin.Connectivity;
using SkiaSharp;
using Xamarin.Forms;

namespace DinoLingo
{
    public class ReportPage_ViewModel: INotifyPropertyChanged
    {
        static int ERROR_PREFIX = 110;

        BackButtonView backButtonView;
        INavigation navigation;
        ScrollView centerListView;
        ReportResponse.Report dataForScrollView_cached;
        ReportResponse.Report dataForScrollView_;
        ReportResponse.Report dataForScrollView;
        bool isLoading = false;
        MyViews.SKChart[] sKCharts = new MyViews.SKChart[3];

        string[] notDoneColors =    new string[] { "#FFE5D0", "#A6EAFF", "#D8FFC8" };
        string[] doneColors =       new string[] { "#FD7E14", "#26CBFD", "#58FD14" };
        string[] doneColorsXamarin = new string[] { "FD7E14", "26CBFD", "58FD14" };

        bool isAnimating;
        Object animLock = new Object();
        public bool IsAnimating
        {
            get
            {
                if (animLock != null) lock (animLock)
                    {
                        return isAnimating;
                    }
                else return true;
            }
            set
            {
                if (animLock != null) lock (animLock)
                {
                    isAnimating = value;
                }

            }
        }

        //===================================
        StackLayout totalStack;
        Color grayTextColor = Color.Gray;
        double fontSize;
        double bigfontSize;
        string cacheKey;

        bool showBottomStarted, showBottomEnded, downloadStarted, downloadEnded;

        // === main info ===
        static string[] main_info_fields;// = { "User name or email: ", "Language course: ", "Dinosaur Score: ", "Star score: ", "Quiz score: ", "Total Books: " };
        StackLayout[] row_main_info; //= new StackLayout[main_info_fields.Length];
        Label[] main_info_labels; //= new Label[main_info_fields.Length];

        // === chart info ===
        static string[] chart_titles;// = { "Lessons & games", "Books", "Overall" };
        static string[] comleted_strings = { "completed", "read", "completed" };
        static string[] not_comleted_strings = { "not completed", "not read", "not completed" };
        static string[] summary; // = { "Lessons & games completed:", "Books read:", "Overall Completed:" };
        //ChartView[] chartViews = new ChartView[3];
        Label[] summary_labels = new Label[3];

        // === daily rep ===
        StackLayout daily_rep_stack;

        // === lessons rep ===
        StackLayout lessons_rep_stack;
        private int MAX_RECORDS_IN_REPORT_TABLE = 15;

        private Color _mainTextColor = Color.FromHex("0275c4"); // blue 


        public ReportPage_ViewModel(INavigation navigation)
        {
            this.navigation = navigation;

            showBottomStarted = showBottomEnded = downloadStarted = downloadEnded = false;
            main_info_fields = new string[] {   Translate.GetString("report_user_name_or_email"),    Translate.GetString("report_lang_course"),    Translate.GetString("report_dinosaur_score"),
                                                Translate.GetString("report_star_score"),    Translate.GetString("report_quiz_score"),    Translate.GetString("report_total_books"),};
            row_main_info = new StackLayout[main_info_fields.Length];
            main_info_labels = new Label[main_info_fields.Length];

            chart_titles = new string[] { Translate.GetString("report_lessons_and_games"), Translate.GetString("report_books"), Translate.GetString("report_overall"), };
            //comleted_strings = new string[] { Translate.GetString("report_completed"), Translate.GetString("report_read"), Translate.GetString("report_completed") };
            //not_comleted_strings = new string[] { Translate.GetString("report_not_completed"), Translate.GetString("report_not_read"), Translate.GetString("report_not_completed"), };
            summary = new string[] { Translate.GetString("report_lessons_and_games_completed"), Translate.GetString("report_books_read"), Translate.GetString("report_overall_completed"), };

            fontSize = UI_Sizes.SmallTextSize * 0.75;
            bigfontSize = UI_Sizes.MediumTextSize * 0.75;
        }

        public void AddCloseButton(RelativeLayout totalLayout)
        {
            backButtonView = new BackButtonView();
            backButtonView.AddToTopRight(totalLayout, 0.1, 5, OnClickedClose);
        }

        public void AddCenterView(RelativeLayout totalLayout)
        {
            centerListView = new ScrollView();

            Device.StartTimer(TimeSpan.FromMilliseconds(5), FillTheViewOnStartAsync);


            double padding = 5;

            totalLayout.Children.Add(centerListView,
                Constraint.RelativeToParent((parent) =>
                {
                    return 0 + padding;    // установка координаты X
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return 0;   // установка координаты Y
                }),

                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width - 2 * padding;
                }), // установка ширины
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height;
                })
                );
        }

        bool FillTheViewOnStartAsync()
        {
            Debug.WriteLine("FillTheViewOnStartAsync() ... ");
            //ListItems = null;
            TryToShowContent();
            return false;
        }

        async void TryToShowContent()
        {
            Debug.WriteLine("TryToShowContent...");
            totalStack = new StackLayout() {Spacing = 0, };
            centerListView.Content = totalStack;

            // check, if we have cached content FOR THE current central view
            cacheKey = CacheHelper.REPORT + UserHelper.Lang_cat + UserHelper.Login.user_id;
            if (await CacheHelper.Exists(cacheKey))
            { // we Do have the content - just show it
                Debug.WriteLine("we have cached data for report");  

                ShowTopOfList();
                
            }
            else {
                Debug.WriteLine("try to download data for report (force)...");
                TryToDownloadContent();
            }
        }

        bool ShowBottomOfList () {
            
            ShowList_AddDailyReport();
            ShowList_AddLessonsReport();
            showBottomEnded = true;
            return false;
        }

        async void ShowTopOfList() {
            Debug.WriteLine("ShowTopOfList");
           // string cached = (await CacheHelper.GetAsync(CacheHelper.REPORT + UserHelper.Lang_cat + UserHelper.Login.user_id)).Data;

            dataForScrollView_cached = dataForScrollView = await CacheHelper.GetAsync<ReportResponse.Report>(CacheHelper.REPORT + UserHelper.Lang_cat + UserHelper.Login.user_id);



            Debug.WriteLine("totalStack == null ? : " + (totalStack == null));
            

            ShowList_AddMainInfo();
            ShowList_AddChartInfo();

            showBottomStarted = true;
            ShowBottomOfList();

            TryToDownloadContent();
        }

        void ShowTopAndBottomOfList() {
            Debug.WriteLine("ShowTopAndBottomOfList");            

            ShowList_AddMainInfo();
            ShowList_AddChartInfo();
            ShowList_AddDailyReport();
            ShowList_AddLessonsReport();
        }

        void ShowList_AddMainInfo(){
            Debug.WriteLine("ShowList_AddMainInfo()");
            totalStack.Children.Add(new Label());
            totalStack.Children.Add(new Label());
            totalStack.Children.Add(new MyLabel() { Text = Translate.GetString("report_my_report"), FontSize = bigfontSize, TextColor = grayTextColor, HorizontalOptions = LayoutOptions.Center });
            string[] main_info_values = { dataForScrollView.main_info.email, dataForScrollView.main_info.course, dataForScrollView.main_info.dinos.ToString(), dataForScrollView.main_info.stars.ToString(), dataForScrollView.main_info.quizes.ToString(), dataForScrollView.main_info.books.ToString() };
            for (int i = 0; i < main_info_fields.Length; i++)
            {
                row_main_info[i] = new StackLayout() { Orientation = StackOrientation.Horizontal };
                row_main_info[i].Children.Add(new MyLabel { Text = main_info_fields[i], FontSize = fontSize, TextColor = grayTextColor, });
                main_info_labels[i] = new MyLabel { Text = main_info_values[i], FontSize = fontSize, TextColor = _mainTextColor };
                row_main_info[i].Children.Add(main_info_labels[i]);
                totalStack.Children.Add(row_main_info[i]);
            }

            totalStack.Children.Add(new Label());
        }

        void ShowList_UpdateMainInfo()
        {
            Debug.WriteLine("ShowList_UpdateMainInfo()");
            string[] main_info_values = { dataForScrollView.main_info.email, dataForScrollView.main_info.course, dataForScrollView.main_info.dinos.ToString(), dataForScrollView.main_info.stars.ToString(), dataForScrollView.main_info.quizes.ToString(), dataForScrollView.main_info.books.ToString() };
            for (int i = 0; i < main_info_fields.Length; i++)
            {
                main_info_labels[i].Text = main_info_values[i];  
            }
        }

        void ShowList_AddChartInfo()
        {

            Debug.WriteLine("ShowList_AddChartInfo()");
            // add chart_info
            var grid_chart_info1 = new Grid() { };
            grid_chart_info1.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid_chart_info1.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid_chart_info1.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid_chart_info1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid_chart_info1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(UI_Sizes.MediumTextSize * 7.5, GridUnitType.Absolute) });
            grid_chart_info1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid_chart_info1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            /*
            var grid_chart_info2 = new Grid() { };
            grid_chart_info2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid_chart_info2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            grid_chart_info2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid_chart_info2.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid_chart_info2.RowDefinitions.Add(new RowDefinition { Height = new GridLength(UI_Sizes.MediumTextSize * 7.5, GridUnitType.Absolute) });
            grid_chart_info2.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid_chart_info2.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            */

            int[] comleted_items = { dataForScrollView.chart_info.lessons_compl, dataForScrollView.chart_info.books_compl, dataForScrollView.chart_info.overall_compl };
            int[] total_items = { dataForScrollView.chart_info.lessons_total, dataForScrollView.chart_info.books_total, dataForScrollView.chart_info.overall_total };
            for (int i = 0; i < 3; i++)
            {
                // add title
                if (i < 3)
                grid_chart_info1.Children.Add(new MyLabel() { Text = chart_titles[i], FontSize = bigfontSize,
                    TextColor = Color.FromHex(doneColorsXamarin[i]), 
                    HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center }, i, 0);
                //else grid_chart_info2.Children.Add(new MyLabel() { Text = chart_titles[i], FontSize = bigfontSize, TextColor = Color.White, 
                //    HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center }, 1, 0);

                // add chart
                /*
                var entries = new[]
                {
                    new Microcharts.Entry(comleted_items[i])
                    {
                        Label = comleted_strings[i],
                        TextColor = SKColor.Parse("#ffffff"),
                        ValueLabel = comleted_items[i].ToString(),
                        Color = SKColor.Parse("#00ff57")

                    },
                    new Microcharts.Entry(total_items[i] - comleted_items[i])
                    {
                        Label = not_comleted_strings[i],
                        TextColor = SKColor.Parse("#ffffff"),
                        ValueLabel = (total_items[i] - comleted_items[i]).ToString(),
                        Color = SKColor.Parse("#ff6666")
                    },
                };
                var chart = new DonutChart() { Entries = entries, HoleRadius = 0, Margin = 10, LabelTextSize = (float)(fontSize * 1.5), LabelColor = SKColor.Parse("#ffffff"), BackgroundColor = SKColor.Empty, IsAnimated = false };

                chartViews[i] = new ChartView() { Chart = chart, BackgroundColor = Color.Transparent};

                RelativeLayout relLayout = new RelativeLayout() {};
                double heightSize = 0.6;

                relLayout.Children.Add(chartViews[i],
                //relLayout.Children.Add(new MyViews.SKChart(),
                Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - parent.Height * heightSize) * 0.5;    // установка координаты X
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return 0;   // установка координаты Y
                }),

                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height * heightSize;
                }), // установка ширины
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height * 1.0;
                })
                );
                */
                sKCharts[i] = new MyViews.SKChart();
                sKCharts[i].SetColors(doneColors[i], notDoneColors[i]);
                sKCharts[i].value = (float)comleted_items[i] / total_items[i];

                if (i < 3)
                    //grid_chart_info1.Children.Add(relLayout, i, 1);
                    grid_chart_info1.Children.Add(sKCharts[i], i, 1);
                
                //else
                    //grid_chart_info2.Children.Add(relLayout, 1, 1);
                    //grid_chart_info2.Children.Add(new MyViews.SKChart(), 1, 1);

                // add summary title
                if (i < 3)
                grid_chart_info1.Children.Add(new MyLabel() { Text = summary[i], FontSize = fontSize, TextColor = Color.FromHex(doneColorsXamarin[i]), }, i, 2);
                //else
                //grid_chart_info2.Children.Add(new MyLabel() { Text = summary[i], FontSize = fontSize, TextColor = Color.White, }, 1, 2);

                // add summary value
                summary_labels[i] = new MyLabel()
                {
                    Text = comleted_items[i] + "/" + total_items[i] + "(" + (((double)comleted_items[i] * 100) / total_items[i]).ToString("F1") + "%)",
                    FontSize = fontSize,
                    //TextColor = (Color) Application.Current.Resources["ReportOrangeColor"], 
                    TextColor = Color.FromHex(doneColorsXamarin[i]),
                };

                if (i < 3)
                grid_chart_info1.Children.Add(summary_labels[i], i, 3);
                //else grid_chart_info2.Children.Add(summary_labels[i], 1, 3);

            }

            totalStack.Children.Add(grid_chart_info1);

            //totalStack.Children.Add(grid_chart_info2);

            totalStack.Children.Add(new Label());
        }

        void ShowList_UpdateChartInfo()
        {
            Debug.WriteLine("ShowList_UpdateChartInfo()");
            // add chart_info
            int[] comleted_items_prev = { dataForScrollView_cached.chart_info.lessons_compl, dataForScrollView_cached.chart_info.books_compl, dataForScrollView_cached.chart_info.overall_compl };
            int[] total_items_prev = { dataForScrollView_cached.chart_info.lessons_total, dataForScrollView_cached.chart_info.books_total, dataForScrollView_cached.chart_info.overall_total };

            int[] comleted_items = { dataForScrollView.chart_info.lessons_compl, dataForScrollView.chart_info.books_compl, dataForScrollView.chart_info.overall_compl };
            int[] total_items = { dataForScrollView.chart_info.lessons_total, dataForScrollView.chart_info.books_total, dataForScrollView.chart_info.overall_total };
            for (int i = 0; i < 3; i++)
            {
                if (comleted_items_prev[i] == comleted_items[i] && total_items_prev[i] == total_items[i]) continue;
                // add chart
                /*
                var entries = new[]
                {
                    new Microcharts.Entry(comleted_items[i])
                    {
                        Label = comleted_strings[i],
                        TextColor = SKColor.Parse("#ffffff"),
                        ValueLabel = comleted_items[i].ToString(),
                        Color = SKColor.Parse("#00ff57")

                    },
                    new Microcharts.Entry(total_items[i] - comleted_items[i])
                    {
                        Label = not_comleted_strings[i],
                        TextColor = SKColor.Parse("#ffffff"),
                        ValueLabel = (total_items[i] - comleted_items[i]).ToString(),
                        Color = SKColor.Parse("#ff6666")
                    },
                };

                var chart = new DonutChart() { Entries = entries, HoleRadius = 0, Margin = 10, LabelTextSize = (float)(fontSize * 1.5), LabelColor = SKColor.Parse("#ffffff"),  BackgroundColor = SKColor.Empty };
                chartViews[i].Chart = chart;
                */
                sKCharts[i].value = (float)comleted_items[i] / total_items[i];
                sKCharts[i].InvalidateSurface();
                // add summary value
                summary_labels[i].Text = comleted_items[i] + "/" + total_items[i] + "(" + (((double)comleted_items[i] * 100) / total_items[i]).ToString("F1") + "%)";
            }           
        }

        void ShowList_AddDailyReport() {
            Debug.WriteLine("ShowList_AddDailyReport() --> is totalStack == null ? = " + (totalStack == null));
            // add daily report
            totalStack.Children.Add(new MyLabel() { Text = Translate.GetString("report_daily_report"), FontSize = bigfontSize, TextColor = _mainTextColor, HorizontalOptions = LayoutOptions.Center });
            // add header
            var grid_daily_rep_header = new Grid() { BackgroundColor = (Color) Application.Current.Resources["ReportRedColor"] };
            grid_daily_rep_header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid_daily_rep_header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            grid_daily_rep_header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            grid_daily_rep_header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            grid_daily_rep_header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });

            string[] daily_rep_header = { string.Empty, Translate.GetString("report_date"),
                Translate.GetString("report_lesson"), Translate.GetString("report_completion"),  Translate.GetString("report_time_spent"), };
            for (int i = 0; i < daily_rep_header.Length; i++)
            {
                grid_daily_rep_header.Children.Add(new MyLabel { Text = daily_rep_header[i], FontSize = fontSize, TextColor = Color.White}, i, 0);
            }
            totalStack.Children.Add(grid_daily_rep_header);

            daily_rep_stack = new StackLayout() {Spacing = 0};

            FillDailyReportBody();

            totalStack.Children.Add(daily_rep_stack);

            totalStack.Children.Add(new Label());
        }

        void FillDailyReportBody()
        {
            int curRow = 0;

            for (int d = 0; d < dataForScrollView.daily_report.Length; d++) // processs each day
            {
                // add day
                Color curColor = curRow % 2 == 0 ? Color.White : Color.LightGray;
                var grid_daily_rep = new Grid() { BackgroundColor = curColor };
                grid_daily_rep.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid_daily_rep.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                grid_daily_rep.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                grid_daily_rep.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                grid_daily_rep.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });

                grid_daily_rep.Children.Add(new MyLabel { Text = dataForScrollView.daily_report[d].date, FontSize = fontSize, TextColor = grayTextColor, HorizontalTextAlignment = TextAlignment.Center },
                                                1, 0);
                curRow++;
                daily_rep_stack.Children.Add(grid_daily_rep);

                // add all the records

                for (int i = 0; i < dataForScrollView.daily_report[d].report.Length; i++)
                {
                    Color curColor_ = curRow % 2 == 0 ? Color.White : Color.LightGray;
                    var grid_daily_rep_ = new Grid() { BackgroundColor = curColor_ };
                    grid_daily_rep_.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid_daily_rep_.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                    grid_daily_rep_.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                    grid_daily_rep_.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                    grid_daily_rep_.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });

                    string[] rep_record = { (i + 1).ToString(), dataForScrollView.daily_report[d].report[i].time,
                    System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(dataForScrollView.daily_report[d].report[i].name)), dataForScrollView.daily_report[d].report[i].progress + "%", dataForScrollView.daily_report[d].report[i].spent };

                    // add record
                    for (int j = 0; j < rep_record.Length; j++)
                    {
                        if (j == 0)
                        {
                            grid_daily_rep_.Children.Add(new MyLabel { Text = (i + 1).ToString(), FontSize = fontSize, TextColor = grayTextColor, },
                                                j, 0);
                        }
                        else
                        {
                            grid_daily_rep_.Children.Add(new MyLabel { Text = rep_record[j], FontSize = fontSize, TextColor = grayTextColor, }, j, 0);
                        }
                    }
                    curRow++;

                    daily_rep_stack.Children.Add(grid_daily_rep_);
                    if (curRow > MAX_RECORDS_IN_REPORT_TABLE) break;
                }
                if (curRow > MAX_RECORDS_IN_REPORT_TABLE) break;
            }            
        }

        void ShowList_UpdateDailyReport()
        {
            Debug.WriteLine("ShowList_UpdateDailyReport()");
            daily_rep_stack.Children.Clear();

            FillDailyReportBody();
        }
       

        void ShowList_AddLessonsReport() {
            Debug.WriteLine("ShowList_AddLessonsReport()");
            // add lessons report  
            totalStack.Children.Add(new MyLabel() { Text = Translate.GetString("report_lessons_report"), FontSize = bigfontSize, TextColor = _mainTextColor, HorizontalOptions = LayoutOptions.Center });
            // add header
            var grid_lessons_rep_header = new Grid() { BackgroundColor = (Color) Application.Current.Resources["ReportRedColor"] };
            grid_lessons_rep_header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid_lessons_rep_header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            grid_lessons_rep_header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            grid_lessons_rep_header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });

            string[] lesson_rep_header = { string.Empty, Translate.GetString("report_lesson"), Translate.GetString("report_last_activity"), Translate.GetString("report_completion") };
            for (int i = 0; i < lesson_rep_header.Length; i++)
            {
                grid_lessons_rep_header.Children.Add(new MyLabel { Text = lesson_rep_header[i], FontSize = fontSize, TextColor = Color.White, }, i, 0);
            }
            totalStack.Children.Add(grid_lessons_rep_header);

            lessons_rep_stack = new StackLayout() {Spacing = 0};

            FillLessonsReportBody();

            totalStack.Children.Add(lessons_rep_stack);
        }

        void FillLessonsReportBody()
        {
            int i;
            for (i = 0; i < dataForScrollView.lessons_report.Length; i++)
            {
                Color curColor = i % 2 == 0 ? Color.White : Color.LightGray;
                var grid_lessons_rep = new Grid() { BackgroundColor = curColor };
                grid_lessons_rep.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid_lessons_rep.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                grid_lessons_rep.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                grid_lessons_rep.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });

                string[] rep_record = { (i + 1).ToString(),
                    System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(dataForScrollView.lessons_report[i].name)), dataForScrollView.lessons_report[i].last_activity, dataForScrollView.lessons_report[i].progress + "%" };

                // add record
                for (int j = 0; j < rep_record.Length; j++)
                {

                    grid_lessons_rep.Children.Add(new MyLabel { Text = rep_record[j], FontSize = fontSize, TextColor = grayTextColor, }, j, 0);

                }
                lessons_rep_stack.Children.Add(grid_lessons_rep);

                if (i > MAX_RECORDS_IN_REPORT_TABLE)
                    break;
            }

            if (i <= MAX_RECORDS_IN_REPORT_TABLE)
            for (i = i; i < dataForScrollView.books_report.Length; i++)
            {
                Color curColor = i % 2 == 0 ? Color.White : Color.LightGray;
                var grid_lessons_rep = new Grid() { BackgroundColor = curColor };
                grid_lessons_rep.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid_lessons_rep.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                grid_lessons_rep.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                grid_lessons_rep.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });

                string[] rep_record = { (i + 1).ToString(),
                    System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(dataForScrollView.books_report[i].name)), dataForScrollView.books_report[i].last_activity, dataForScrollView.books_report[i].progress + "%" };

                // add record
                for (int j = 0; j < rep_record.Length; j++)
                {

                    grid_lessons_rep.Children.Add(new MyLabel { Text = rep_record[j], FontSize = fontSize, TextColor = grayTextColor, }, j, 0);

                }
                lessons_rep_stack.Children.Add(grid_lessons_rep);

                if (i > 5) break;
            }

        }

        void ShowList_UpdateLessonsReport()
        {
            Debug.WriteLine("ShowList_UpdateLessonsReport()");
            lessons_rep_stack.Children.Clear();

            FillLessonsReportBody();
        }

        Task TryToDownloadContent(int delay = 0)
        {
            downloadStarted = true;
            return Task.Run(async()=> {
                try
                {
                    isLoading = true;
                    await Task.Delay(delay);
                    // check connection here
                    if (!CrossConnectivity.Current.IsConnected || !DownloadHelper.DownloadHelper.CheckInternetConnectionProgressive())
                    { // check connectivity
                        if (AsyncMessages.CheckConnectionTimeout())
                        {
                            AsyncMessages.DisplayAlert(POP_UP.OOPS, POP_UP.NO_CONNECTION
                                //+ POP_UP.GetCode(null, ERROR_PREFIX + 1)
                                , POP_UP.OK);
                        }
                        
                        //("Error!", "No internet connection!", "ОK");
                        isLoading = false;
                        return;
                    }

                    // get ReportResponse
                    var postData = $"cat={UserHelper.Lang_cat}";
                    postData += $"&user_id={UserHelper.Login.user_id}";
                    postData += $"&daily_limit={100}";
                    ReportResponse reportResponse = await ServerApi.PostRequestProgressive<ReportResponse>(ServerApi.REPORT_URL, postData, null);

                    // process response
                    if (reportResponse == null)
                    {
                        Analytics.SendResultsRegular("ReportPage_ViewModel", reportResponse, reportResponse?.error, ServerApi.REPORT_URL, postData);
                        if (AsyncMessages.CheckDisplayAlertTimeout())
                        {
                            AsyncMessages.DisplayAlert(POP_UP.OOPS,
                            POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(reportResponse?.error, ERROR_PREFIX + 2), POP_UP.OK);
                            //("Error!", "Error in response from server!", "ОK");
                        }

                        isLoading = false;
                        return;
                    }
                    if (reportResponse.error != null)
                    {
                        Analytics.SendResultsRegular("ReportPage_ViewModel", reportResponse, reportResponse?.error, ServerApi.REPORT_URL, postData);
                        if (AsyncMessages.CheckDisplayAlertTimeout())
                        {
                            AsyncMessages.DisplayAlert(POP_UP.OOPS,
                            POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(reportResponse?.error, ERROR_PREFIX + 3), POP_UP.OK);
                            //("Error!", "Message: " + reportResponse.error.message, "ОK");
                        }

                        isLoading = false;
                        return;
                    }
                    if (reportResponse.result == null)
                    {
                        Analytics.SendResultsRegular("ReportPage_ViewModel", reportResponse, reportResponse?.error, ServerApi.REPORT_URL, postData);
                        if (AsyncMessages.CheckDisplayAlertTimeout())
                        {
                            AsyncMessages.DisplayAlert(POP_UP.OOPS,
                           POP_UP.SOME_ERROR_IN_RESPONSE + POP_UP.GetCode(reportResponse?.error, ERROR_PREFIX + 4), POP_UP.OK);
                            //("Error!", "Result from server is null!", "ОK");
                        }

                        isLoading = false;
                        return;
                    }

                    // add to cache
                    Debug.WriteLine("reportResponse downloaded ok");
                    await CacheHelper.Add<ReportResponse.Report>(CacheHelper.REPORT + UserHelper.Lang_cat + UserHelper.Login.user_id, reportResponse.result);
                    dataForScrollView_ = reportResponse.result;
                    isLoading = false;
                    downloadEnded = true;
                    if (showBottomStarted) while (!showBottomEnded)
                        {
                            Debug.WriteLine("showBottomStarted, wait for it...");
                            await Task.Delay(100);
                        }

                    Device.BeginInvokeOnMainThread(ShowListWhenDownloaded);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ReportPage_ViewModel -> TryToDownloadContent -> ex: " + ex.Message);
                }                

                return;
            });

        }

        void ShowListWhenDownloaded () {
            dataForScrollView = (dataForScrollView_ != null) ? dataForScrollView_ : dataForScrollView_cached;
            if (dataForScrollView == null) return;
            if (showBottomStarted)  { // update
                Debug.WriteLine("ShowListWhenDownloaded  --> update");
                ShowList_UpdateMainInfo();
                ShowList_UpdateChartInfo();
                ShowList_UpdateDailyReport();
                ShowList_UpdateLessonsReport();
            }
            else { // create from zero 
                Debug.WriteLine("ShowListWhenDownloaded  --> create");
                ShowTopAndBottomOfList();
            }
        }

        async void OnClickedClose(View view)
        {
            if (IsAnimating) return;
            IsAnimating = true;
            await AnimateImage(view, 250);
            await navigation.PopModalAsync();

            IsAnimating = false;
        }

        public void OnAppearing()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Task AnimateImage(View view, uint time)
        {
           
            return Task.Run(async () =>
            {
                try
                {
                    await view.ScaleTo(0.8, time / 2);
                    await view.ScaleTo(1.0, time / 2);
                }
                catch
                {

                }
                
                return;
            });
        }

        public void Dispose()
        {
            backButtonView.Dispose();
            navigation = null;

            centerListView.Content = null;
            centerListView = null;

            dataForScrollView_cached =  dataForScrollView_ = dataForScrollView = null;
            animLock = null;
            totalStack = null;
            row_main_info = null; 
            main_info_labels = null;

            //chartViews = null;
            sKCharts = null;

            summary_labels = null;
            
            daily_rep_stack = null;            
            lessons_rep_stack = null;
        }
    }
}
