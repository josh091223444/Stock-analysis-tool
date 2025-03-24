using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program {
    // static string is included in the code so that program which data should be included in monthly analysis for each month 
    static string[] monthNames = {
        "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };

public static void Main(string[] args) {
 Console.WriteLine("Main menu");
Console.WriteLine("1. Finding Minimum/Maximum "); Console.WriteLine("2. Stock Analysis ");
Console.WriteLine("3. Exit ");
Console.WriteLine("Enter a number:");
int choice = Convert.ToInt32(Console.ReadLine());
 if (choice == 1) {
FindMinMax();
        } else if (choice == 2) {
            Console.WriteLine("Stock Analysis Main Menu");
            Console.WriteLine("1. Yearly Data Analysis");
            Console.WriteLine("2. Monthly Data Analysis");
            Console.WriteLine("3. Exit to main menu ");
            Console.WriteLine("Please enter a number");
            int number = int.Parse(Console.ReadLine());
            if (number == 1) {
                yearlyDataAnalysis();
            } else if (number == 2) {
                monthlyDataAnalysis();
            } else if (number == 3) {
              Console.WriteLine("Exit");

            }
   else if (number != 1 && number != 2 && number != 3)
   {
     Console.WriteLine("Invalid input");
   }

        }
        if (choice == 3) {

            Console.WriteLine("Goodbye");
            Environment.Exit(0);
        } 
  else  if (choice != 1 && choice != 2 && choice != 3)
   {
     Console.WriteLine("Invalid choice please try again");
   }
    }

    static void FindMinMax() {
        // this tells the program that its the start of the finding minimum / maximum function 
        Console.WriteLine("Enter the value of a:");
        double a = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Enter the value of b:");
        double b = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Enter the value of c:");
        double c = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Enter the value of d:");
        double d = Convert.ToDouble(Console.ReadLine());

        double criticalPoint1 = 0;
        double criticalPoint2 = 0;
        // this part of the code is where the discriminant for maximum and minimum are found 
        double discriminant = Math.Pow(b, 2) - 3 * a * c;
        if (discriminant > 0) {
            criticalPoint1 = (-b + Math.Sqrt(discriminant)) / (3 * a);
            criticalPoint2 = (-b - Math.Sqrt(discriminant)) / (3 * a);
        }
        double functionValue1 = CubicFunction(criticalPoint1, a, b, c, d);
        double functionValue2 = CubicFunction(criticalPoint2, a, b, c, d);


        double localMaximum = Math.Max(functionValue1, functionValue2);
        double localMinimum = Math.Min(functionValue1, functionValue2);


        double xForMaximum = localMaximum == functionValue1 ? criticalPoint1 : criticalPoint2;
        double xForMinimum = localMinimum == functionValue1 ? criticalPoint1 : criticalPoint2;

      localMaximum = Math.Round(localMaximum, 2);
      localMinimum = Math.Round(localMinimum, 2);
      xForMaximum = Math.Round(xForMaximum, 2);
      xForMinimum = Math.Round(xForMinimum, 2);

        Console.WriteLine("Local Maximum: f(x) = " + localMaximum + " at x = " + xForMaximum);
        Console.WriteLine("Local Minimum: f(x) = " + localMinimum + " at x = " + xForMinimum);
    }

    static double CubicFunction(double x, double a, double b, double c, double d) {
        return a * Math.Pow(x, 3) + b * Math.Pow(x, 2) + c * x + d;
    }

    static double CubicFunctionDerivative(double x, double a, double b, double c, double d) {
        return 3 * a * Math.Pow(x, 2) + 2 * b * x + c;
    }

    static double FindRoot(double target, double a, double b, double c, double d) {
        double x = 0;
         // this part of the code tells the program how precices the solution has to be while also telling it the max amount of iterations 
        int maxIteration = 1000;
        double tolerance = 1e-6;

        for (int i = 0; i < maxIteration; i++) {
            double fx = CubicFunction(x, a, b, c, d) - target;
            double fpx = CubicFunctionDerivative(x, a, b, c, d);

            double deltaX = fx / fpx;

            x -= deltaX;

            if (Math.Abs(deltaX) < tolerance)
                break;
        }
        return x;
    }

    static void monthlyDataAnalysis() {
        // start of monthlyDataAnalysis function 
        string filePath = @"data_analysis.csv";
        string[] lines = File.ReadAllLines(filePath);
        string[] headers = lines.First().Split(',');
        var relevantData = lines.Skip(1);

        Console.WriteLine("Enter a month (MM): ");
        string selectedMonth = Console.ReadLine();
    // the purpose of the var filtered data is to make sure the variable only contains data from rows and columns of a certain month 
        var filteredData = relevantData.Where(line => {
            string[] columns = line.Split(',');
            string[] dateParts = columns[0].Split('/');
            return dateParts.Length >= 2 && dateParts[1] == selectedMonth;
        });

        if (filteredData.Any()) {
            double openingPrice = 0;
            double closingPrice = 0;
            decimal highestTradingPrice = decimal.MinValue;
            decimal lowestTradingPrice = decimal.MaxValue;
            Dictionary<string, decimal> tradingVolumes = new Dictionary<string, decimal>();

            foreach (var dataRow in filteredData) {
                string[] columns = dataRow.Split(',');

                string date = columns[0];
                decimal open = decimal.Parse(columns[1]);
                decimal high = decimal.Parse(columns[2]);
                decimal low = decimal.Parse(columns[3]);
                decimal close = decimal.Parse(columns[4]);
                decimal volume = decimal.Parse(columns[5]);

                openingPrice = (openingPrice == 0) ? (double)open : openingPrice;
                closingPrice = (double)close;
                highestTradingPrice = Math.Max(highestTradingPrice, high);
                lowestTradingPrice = Math.Min(lowestTradingPrice, low);
                tradingVolumes[date] = volume;
            }


            Console.WriteLine($"Selected Month: {monthNames[int.Parse(selectedMonth)]}");

            Console.WriteLine($"Opening price: {(openingPrice != 0 ? openingPrice.ToString("F2") : "N/A")}");
            Console.WriteLine($"Closing price: {closingPrice:F2}");
            Console.WriteLine($"Highest trading price: {highestTradingPrice:F2}");
            Console.WriteLine($"Lowest trading price: {lowestTradingPrice:F2}");

            // variable checks the top 3 dates and orgainsins them by descending order 
            var top3Dates = tradingVolumes.OrderByDescending(kv => kv.Value).Take(3).Select(kv => kv.Key).ToList();
            Console.WriteLine("The three dates with the highest trading volume (in descending order):");
            foreach (var date in top3Dates) {
                Console.WriteLine(date);
            }
        } else {
            Console.WriteLine("No data found for the selected month.");
        }
    }
       static void yearlyDataAnalysis() {
        string filePath = @"data_analysis.csv";
        string[] lines = File.ReadAllLines(filePath);
        string[] headers = lines.First().Split(',');
              var relevantData = lines.Skip(1);

              int totalTradingDays = 0;
              decimal openingPriceFirstDay = 0;
              decimal closingPriceLastDay = 0;
              decimal highestPrice = decimal.MinValue;
              decimal lowestPrice = decimal.MaxValue;
              string dateHighestVolume = "";
              decimal previousDayClosePrice = 0;
              decimal maxVolume = 0;
              decimal closingPriceChange = 0;

              foreach (var dataRow in relevantData) {
              string[] columns = dataRow.Split(',');
                            totalTradingDays++;

                            string date = columns[0];
                            decimal open = decimal.Parse(columns[1]);
                            decimal high = decimal.Parse(columns[2]);
                            decimal low = decimal.Parse(columns[3]);
                            decimal close = decimal.Parse(columns[4]);
                            decimal volume = decimal.Parse(columns[5]);
                             // checks each variable over the yearly data analysis 
                            if (totalTradingDays == 1) {
                                openingPriceFirstDay = open;
                            }
                            if (totalTradingDays == relevantData.Count()) {
                                closingPriceLastDay = close;
                            }
                            if (high > highestPrice) {
                                highestPrice = high;
                            }
                            if (low < lowestPrice) {
                                lowestPrice = low;
                            }
                            if (volume > maxVolume) {
                                maxVolume = volume;
                                dateHighestVolume = date;
                            }
                            if (totalTradingDays > 1) {
                                closingPriceChange = ((close - previousDayClosePrice) / previousDayClosePrice) * 100;
                            }
                            previousDayClosePrice = close;
                        }
                        Console.WriteLine($"Total number of trading days: {totalTradingDays}");
                        Console.WriteLine($"Opening price of the first trading day: {openingPriceFirstDay}");
                        Console.WriteLine($"Closing price of the last trading day: {closingPriceLastDay}");
                        Console.WriteLine($"Highest trading price of the year: {highestPrice}");
                        Console.WriteLine($"Lowest trading price of the year: {lowestPrice}");
                        Console.WriteLine($"Date with the highest trading volume: {dateHighestVolume}, closing price change from the previous trading day: {closingPriceChange}%");
                    }
                }

